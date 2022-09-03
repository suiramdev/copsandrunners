using Sandbox;

namespace copsandrunners.Players;

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

public partial class Player
{
	[Net] private Roles _Role { get; set; } = Roles.None;
	public Roles Role
	{
		get => _Role;
		set
		{
			Host.AssertServer();
			
			_Role = value;
			Respawn();
		}
	}
	
	[Net] public Teams Team { get; set; } = Teams.None;
}
