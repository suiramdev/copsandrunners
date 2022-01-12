using Sandbox;

namespace copsandrunners;

public enum Roles
{
	None,
	Spectator,
	Cop,
	Runner
}

public partial class PlayerPawn : Player
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

	private bool _isJailed;
	[Net]
	public bool IsJailed
	{
		get => (Role != Roles.Cop) && _isJailed;
		set => Jail( value );
	}

	public PlayerPawn()
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

	public void Jail(bool value)
	{
		_isJailed = value;
		CollisionGroup = value ? CollisionGroup.Debris : CollisionGroup.Player;
		
		if ( value && IsServer )
			Position = Game.JailPosition;
	}
}
