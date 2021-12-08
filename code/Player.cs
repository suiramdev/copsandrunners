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
			Log.Info( $"{Name} set as {value.ToString()}" );
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
				break;
		}
		Inventory.Add( new Weapons.Slap(), true );
	}
	
	[ServerCmd( "switchWeapon" )]
	public static void SwitchWeapon(int slot)
	{
		ConsoleSystem.Caller?.Pawn.Inventory.SetActiveSlot( slot, true );
	}
}
