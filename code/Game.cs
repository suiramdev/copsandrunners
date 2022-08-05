using System;
using System.Collections.Generic;
using System.Linq;
using copsandrunners.Entities;
using copsandrunners.UI;
using Sandbox;

namespace copsandrunners;

partial class Game : Sandbox.Game
{
	public static Game Instance;

	public Game()
	{
		Instance = this;

		if ( !IsServer )
			return;

		_ = new Hud();
		_ = GameLoop();
	}
	
	[Net] public static Jail Jail { get; set; }

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var player = new Player { Role = State == GameStates.Wait ? Roles.None : Roles.Spectator };

		client.Pawn = player;
	}
}
