using System;
using Sandbox;

namespace copsandrunners;

public enum Roles
{
	None,
	Spectator,
	Cop,
	Runner
}

public class OnArrestEventArgs : EventArgs
{
	public Entity Caller;
	public Entity Target;
	public bool IsArrested;
}

public partial class Player : Sandbox.Player
{
	private Roles _role;
	public Roles Role
	{
		get => _role;
		set
		{
			_role = value;
			Respawn();
		}
	}

	private bool _isArrested;
	[Net]
	public bool IsArrested => (Role != Roles.Cop) && _isArrested;
	public event EventHandler OnArrest;

	public Player()
	{
		Inventory = new BaseInventory(this);
	}

	public override void Respawn()
	{
		base.Respawn();
		SetModel( "models/citizen/citizen.vmdl" );
		
		Controller = Role == Roles.Spectator ? new NoclipController() : new WalkController();
		Animator = new StandardPlayerAnimator();
		Camera = new FirstPersonCamera();

		EnableAllCollisions = true; 
		EnableDrawing = Role != Roles.Spectator;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		LoadLoadout();
	}

	private void LoadLoadout()
	{
		Inventory.DeleteContents();
		switch ( Role )
		{
			case Roles.Cop:
				Inventory.Add( new Weapons.JailPlacer(), true );
				Inventory.Add( new Weapons.CopsMelee() );
				break;
			case Roles.Runner:
			case Roles.None:
				Inventory.Add( new Weapons.RobbersMelee(), true );
				break;
		}
	}

	public override void OnKilled()
	{
		base.OnKilled();

		Role = Roles.Spectator;
	}

	public override void Simulate( Client client )
	{
		base.Simulate( client );

		SimulateActiveChild( client, ActiveChild );
	}

	public virtual void Arrest(OnArrestEventArgs e)
	{
		_isArrested = e.IsArrested;
		CollisionGroup = e.IsArrested ? CollisionGroup.Debris : CollisionGroup.Player;
		
		if ( e.IsArrested )
			Position = Game.JailPosition;
		
		OnArrest?.Invoke( this, e );
	}
}
