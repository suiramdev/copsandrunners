using System;
using copsandrunners.Items;
using copsandrunners.Items.Weapons;
using copsandrunners.UI;
using Sandbox;

namespace copsandrunners.Players;

public class OnArrestEventArgs : EventArgs
{
	public Entity Caller;
	public Entity Target;
	public bool IsArrested;
}

[Title( "Player" ), Icon( "emoji_people" )]
public partial class Player : AnimatedEntity
{
	#region Controls
	[Net, Predicted]
	private PawnController _controller { get; set; }
	public PawnController Controller
	{
		get => DevController ?? _controller;
		set => _controller = value;
	}

	[Net, Predicted]
	public PawnController DevController { get; set; }

	public CameraMode CameraMode
	{
		get => Components.Get<CameraMode>();
		set => Components.Add( value );
	}
	
	[Net, Predicted]
	public PawnAnimator Animator { get; set; }
	#endregion
	
	[Net]
	public Inventory Inventory { get; set; }

	public Player()
	{
		Inventory = new Inventory( this );

		if (IsClient) // Should it be here?
			_ = new Hud();
	}
	
	public override void Spawn()
	{
		base.Spawn();
		
		EnableLagCompensation = true;

		Tags.Add( "player" );

		if (IsServer)
			Respawn();
	}
	
	public override void OnKilled()
	{
		Game.Instance.OnKilled( this );

		_timeSinceDied = 0;
		LifeState = LifeState.Dead;
		StopUsing();

		Role = Roles.Spectator;
	}
	
	public void Respawn()
	{
		Host.AssertServer();
		
		SetModel( "models/citizen/citizen.vmdl" );

		LifeState = LifeState.Alive;
		Health = 100;
		Velocity = Vector3.Zero;
		WaterLevel = 0;

		CreateHull();

		Game.Instance.MoveToSpawnpoint( this );
		ResetInterpolation();

		Controller = Role == Roles.Spectator ? new Controllers.NoclipController() : new Controllers.WalkController();
		Animator = new StandardPlayerAnimator();
		CameraMode = new Cameras.FirstPersonCamera();

		EnableAllCollisions = true;
		EnableDrawing = Role != Roles.Spectator;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Loadout();
	}
	
	/// <summary>
	/// Give the player their default loadout
	/// </summary>
	private void Loadout()
	{
		Inventory?.Clear();
		switch ( Role )
		{
			case Roles.Cop:
				Inventory?.Add( new CopsMelee(), true );
				break;
			case Roles.ChiefCop:
				if ( Game.Instance.Jail is null ) // Something is wrong here
					Inventory?.Add( new JailPlacer() );
				Inventory?.Add( new CopsMelee(), true );
				break;
			case Roles.Runner:
				Inventory?.Add( new RunnersMelee(), true );
				Inventory?.Add( new ThrowingBall() );
				break;
			case Roles.None:
				break;
			case Roles.Spectator:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private TimeSince _timeSinceDied;
	
	public override void Simulate( Client cl )
	{
		if ( LifeState == LifeState.Dead )
		{
			if ( _timeSinceDied > 3 && IsServer ) 
				Respawn();

			return;
		}

		Controller?.Simulate( cl, this, Animator );

		if ( Input.ActiveChild is Carriable carriable )
			Inventory.SetActive( carriable );
		
		SimulateActiveCarriable();
	}
	
	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		Controller?.FrameSimulate( cl, this, Animator );

		if ( WaterLevel > 0.9f )
			Audio.SetEffect( "underwater", 1, velocity: 5.0f );
		else
			Audio.SetEffect( "underwater", 0, velocity: 1.0f );
	}

	/// <summary>
	/// Create a physics hull for this player. The hull stops physics objects and players passing through
	/// the player. It's basically a big solid box. It also what hits triggers and stuff.
	/// The player doesn't use this hull for its movement size.
	/// </summary>
	private void CreateHull()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );
		EnableHitboxes = true;
	}
	
	/// <summary>
	/// Called from the gamemode, clientside only.
	/// </summary>
	public override void BuildInput( InputBuilder input )
	{
		if ( input.StopProcessing )
			return;

		ActiveCarriable?.BuildInput( input );

		Controller?.BuildInput( input );

		if ( input.StopProcessing )
			return;

		Animator?.BuildInput( input );
	}

	/// <summary>
	/// Called after the camera setup logic has run. Allow the player to
	/// do stuff to the camera, or using the camera. Such as positioning entities
	/// relative to it, like viewmodels etc.
	/// </summary>
	public override void PostCameraSetup( ref CameraSetup setup )
	{
		Host.AssertClient();

		ActiveCarriable?.PostCameraSetup( ref setup );
	}


	private TimeSince _timeSinceLastFootstep = 0;
	
	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
		if ( LifeState != LifeState.Alive )
			return;

		if ( !IsClient )
			return;

		if ( _timeSinceLastFootstep < 0.2f )
			return;

		volume *= Velocity.WithZ( 0 ).Length.LerpInverse( 0.0f, 200.0f ) * 5.0f; // Footstep volume

		_timeSinceLastFootstep = 0;

		var tr = Trace.Ray( pos, pos + Vector3.Down * 20 )
			.Radius( 1 )
			.Ignore( this )
			.Run();
		
		if ( !tr.Hit ) return;

		tr.Surface.DoFootstep( this, tr, foot, volume );
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( LifeState == LifeState.Dead )
			return;

		base.TakeDamage( info );

		this.ProceduralHitReaction( info );
	}

	#region Carriables
	[Net, Predicted]
	public Carriable ActiveCarriable { get; set; }
	
	private Carriable _lastActiveCarriable;

	public override void StartTouch( Entity other )
	{
		if ( IsClient ) return;

		if ( other is PickupTrigger )
		{
			StartTouch( other.Parent );
			return;
		}

		if ( other is Carriable carriable )
			Inventory?.Add( carriable, ActiveCarriable is null );
	}
	
	public void SimulateActiveCarriable()
	{
		if ( _lastActiveCarriable != ActiveCarriable )
		{
			OnActiveCarriableChanged( _lastActiveCarriable, ActiveCarriable );
			_lastActiveCarriable = ActiveCarriable;
		}

		if ( !ActiveCarriable.IsValid() || !ActiveCarriable.IsAuthority )
			return;

		ActiveCarriable.Simulate( Client );
	}

	public void OnActiveCarriableChanged( Carriable previous, Carriable next )
	{
		previous?.ActiveEnd( this, previous.Owner != this );
		next?.ActiveStart( this );
	}
	#endregion
}
