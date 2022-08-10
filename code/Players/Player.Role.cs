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

public partial class Player
{
	[Net, Change( nameof(OnRoleChanged) )] 
	public Roles Role { get; set; } = Roles.None;
	
	[Net] public Teams Team { get; set; } = Teams.None;

	public void OnRoleChanged()
	{
		if (IsServer) 
			Respawn();
	}
}
