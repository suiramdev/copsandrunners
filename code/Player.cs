using System;
using copsandrunners.Players;
using Sandbox;

namespace copsandrunners;

public enum Roles
{
	None,
	Spectator,
	Cop,
	ChiefCop,
	Runner
}

public enum Teams
{
	None,
	Runners,
	Cops
}

public class OnArrestEventArgs : EventArgs
{
	public Entity Caller;
	public Entity Target;
	public bool IsArrested;
}

public partial class Player : Sandbox.Player
{
	[Net] private Roles _role { get; set; } = Roles.None;
	public Roles Role
	{
		get => _role;
		set
		{
			_role = value;
			Respawn();
		}
	}

	[Net] public Teams Team { get; set; } = Teams.None;
	[Net] private bool _isArrested { get; set; }
	public bool IsArrested => (Role != Roles.Cop) && _isArrested;
	[Net] public bool IsFrozen { get; set; }

	public Player()
	{
		Inventory = new Inventory( this );
	}

	public override void Respawn()
	{
		base.Respawn();
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = Role == Roles.Spectator ? new NoclipController() : new Controllers.WalkController();
		Animator = new StandardPlayerAnimator();
		CameraMode = new FirstPersonCamera();

		EnableAllCollisions = true;
		EnableDrawing = Role != Roles.Spectator;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Loadout();
	}

	public override void Simulate( Client client )
	{
		base.Simulate( client );

		if (Input.ActiveChild != null)
			ActiveChild = Input.ActiveChild;
		
		SimulateActiveChild( client, ActiveChild );
	}
	
	public override void OnKilled()
	{
		base.OnKilled();

		Role = Roles.Spectator;
	}

	private void Loadout()
	{
		Inventory.DeleteContents();
		switch ( Role )
		{
			case Roles.Cop:
				if (!Game.Jail.IsValid)
					Inventory.Add( new Weapons.JailPlacer() );
				Inventory.Add( new Weapons.CopsMelee() );
				break;
			case Roles.ChiefCop:
				Inventory.Add( new Weapons.CopsMelee() );
				break;
			case Roles.Runner:
				Inventory.Add( new Weapons.RunnersMelee(), true );
				break;
		}
	}

	[Event( "arrest" )]
	public virtual void Arrest( bool isArrested )
	{
		_isArrested = isArrested;
		CollisionGroup = isArrested ? CollisionGroup.Debris : CollisionGroup.Player;

		if ( isArrested && IsServer )
			Position = Game.Jail.Position;

		if ( isArrested )
			if ( IsServer )
				Position = Game.Jail.Position;
	}

	[Event("knock")]
	public virtual async void Knock()
	{
		if ( IsFrozen ) return;
		
		IsFrozen = true;
		await Task.DelaySeconds( 3 );
		IsFrozen = false;
	}
}
