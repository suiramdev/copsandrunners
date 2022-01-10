using Sandbox;

namespace copsandrunners;

public enum Roles
{
	Spectator,
	Cop,
	Runner
}

public class Player : Sandbox.Player
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
				Inventory.Add( new Weapons.Jailer() );
				break;
			case Roles.Runner:
				Inventory.Add( new Weapons.Slap(), true );
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
	
	[ServerCmd( "switchWeapon" )]
	public static void SwitchWeapon(int slot)
	{
		Entity pawn = ConsoleSystem.Caller?.Pawn;
		pawn.ActiveChild = pawn.Inventory.GetSlot( slot );
	}
}
