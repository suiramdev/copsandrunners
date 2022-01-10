using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace copsandrunners;

public enum GameStates
{
	Wait,
	Play
}

public class Game : Sandbox.Game
{
	private static GameStates _state = GameStates.Wait;

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		Player player = new Player();
		player.Respawn();
		
		client.Pawn = player;
	}

	[ServerCmd( "cr_start" )]
	public static void StartGame()
	{
		Log.Info( "Starting the game..." );
		List<Player> nonCops = All
			.Where( entity => entity is Player player && player.Role != Roles.Cop ).Cast<Player>().ToList();
		if ( nonCops.Count < 2 )
		{
			Log.Info( "Not enough players" );
			return;
		}

		for ( int i = 0; i < Math.Round( (double)nonCops.Count / 3 ); i++ )
		{
			var random = new Random().Next( 0, nonCops.Count - 1 );
			nonCops[random].Role = Roles.Cop;
			nonCops.RemoveAt( random );
		}

		nonCops.ForEach( player => player.Role = Roles.Runner );

		_state = GameStates.Play;
	}

	[ServerCmd( "cr_end" )]
	public static void EndGame()
	{
		Log.Info( "Ending the game..." );
		List<Player> players = All
			.Where( entity => entity is Player ).Cast<Player>().ToList();
		players.ForEach( player => player.Role = Roles.Spectator );

		_state = GameStates.Wait;
	}
}
