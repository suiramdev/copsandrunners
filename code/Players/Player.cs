using System;
using copsandrunners.UI;
using Sandbox;

namespace copsandrunners;

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

	[Net, Predicted]
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
	#region Carry
	[Net, Predicted]
	public Entity ActiveChild { get; set; }
	
	/// <summary>
	/// This isn't networked, but it's predicted. If it wasn't then when the prediction system
	/// re-ran the commands LastActiveChild would be the value set in a future tick, so ActiveEnd
	/// and ActiveStart would get called multiple times and out of order, causing all kinds of pain.
	/// </summary>
	[Predicted]
	Entity LastActiveChild { get; set; }

	public Inventory Inventory { get; protected set; }
	#endregion

	public Player()
	{
		Inventory = new Inventory( this );
		
		if (IsClient) // Should it be here?
			_ = new Hud();
	}
	
	public override void Spawn()
	{
		EnableLagCompensation = true;

		Tags.Add( "player" );

		base.Spawn();
	
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

		Controller = Role == Roles.Spectator ? new NoclipController() : new Controllers.WalkController();
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
		Inventory.DeleteContents();
		switch ( Role )
		{
			case Roles.Cop:
				Inventory.Add( new Weapons.CopsMelee() );
				break;
			case Roles.ChiefCop:
				if ( Game.Instance.Jail is null ) // Something is wrong here
					Inventory.Add( new Weapons.JailPlacer() );
				Inventory.Add( new Weapons.CopsMelee() );
				break;
			case Roles.Runner:
				Inventory.Add( new Weapons.RunnersMelee(), true );
				Inventory.Add( new Weapons.ThrowingBall() );
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
		
		if (Input.ActiveChild != null)
			ActiveChild = Input.ActiveChild;
		
		SimulateActiveChild( cl, ActiveChild );
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

		ActiveChild?.BuildInput( input );

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

		ActiveChild?.PostCameraSetup( ref setup );
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

	public override void StartTouch( Entity other )
	{
		if ( IsClient ) return;

		if ( other is PickupTrigger )
		{
			StartTouch( other.Parent );
			return;
		}

		Inventory?.Add( other, Inventory.Active == null );
	}
	
	public override void TakeDamage( DamageInfo info )
	{
		if ( LifeState == LifeState.Dead )
			return;

		base.TakeDamage( info );

		this.ProceduralHitReaction( info );
	}

	/// <summary>
	/// Simulated the active child. This is important because it calls ActiveEnd and ActiveStart.
	/// If you don't call these things, viewmodels and stuff won't work, because the entity won't
	/// know it's become the active entity.
	/// </summary>
	private void SimulateActiveChild( Client cl, Entity child )
	{
		if ( LastActiveChild != child )
		{
			OnActiveChildChanged( LastActiveChild, child );
			LastActiveChild = child;
		}

		if ( !LastActiveChild.IsValid() )
			return;

		if ( LastActiveChild.IsAuthority )
		{
			LastActiveChild.Simulate( cl );
		}
	}

	private void OnActiveChildChanged( Entity previous, Entity next )
	{
		if ( previous is BaseCarriable previousBc ) 
			previousBc.ActiveEnd( this, previousBc.Owner != this );

		if ( next is BaseCarriable nextBc ) 
			nextBc.ActiveStart( this );
	}

	public override void OnChildAdded( Entity child )
	{
		Inventory?.OnChildAdded( child );
	}

	public override void OnChildRemoved( Entity child )
	{
		Inventory?.OnChildRemoved( child );
	}
}
