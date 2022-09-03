using copsandrunners.Entities;
using copsandrunners.Players;
using Sandbox;

namespace copsandrunners;

partial class Game : Sandbox.Game
{
	public static Game Instance => Current as Game;

	public Game()
	{
		if ( !IsServer )
			return;

		_ = GameLoop();
	}
	
	[Net] public Jail Jail { get; set; }

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var player = new Players.Player { Role = State == GameStates.Wait ? Roles.None : Roles.Spectator };

		client.Pawn = player;
	}
}
