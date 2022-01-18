using System;
using System.Collections.Generic;
using System.Linq;
using copsandrunners.Entities;
using copsandrunners.UI;
using Sandbox;

namespace copsandrunners;

public enum GameStates
{
	Wait,
	Play
}

public class Game : Sandbox.Game
{
	[Net] public static GameStates State { get; set; } = GameStates.Wait;
	[Net] public static Jail Jail { get; set; }

	public Game()
	{
		if ( IsServer )
			_ = new Hud();
	}
	
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		Player player = new Player();
		player.Role = State == GameStates.Wait ? Roles.None : Roles.Spectator;
		
		client.Pawn = player;
	}

	[ServerCmd( "cr_start" )]
	public static void StartGame()
	{
		Log.Info( "Starting the game..." );
		List<Player> players = All.Where( entity => entity is Player ).Cast<Player>().ToList();
		if ( players.Count < 2 )
		{
			Log.Info( "Not enough players" );
			return;
		}
		
		State = GameStates.Play;

		for ( int i = 0; i < Math.Round( (double)players.Count / 3 ); i++ )
		{
			var random = new Random().Next( 0, players.Count - 1 );
			players[random].Role = Roles.Cop;
			players.RemoveAt( random );
		}

		players.ForEach( player => player.Role = Roles.Runner );
	}

	[ServerCmd( "cr_end" )]
	public static void EndGame()
	{
		Log.Info( "Ending the game..." );
		
		State = GameStates.Wait;
		
		List<Player> players = All.Where( entity => entity is Player ).Cast<Player>().ToList();
		players.ForEach( player => player.Role = Roles.None );
	}
}
