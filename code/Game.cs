using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using copsandrunners.Entities;
using copsandrunners.UI;
using Sandbox;

namespace copsandrunners;

public enum GameStates
{
	Waiting,
	Starting,
	Playing
}

public delegate void GameStateChanged(GameStates newState, GameStates lastState);

public class Game : Sandbox.Game
{
	[Net] private static GameStates _state { get; set; } = GameStates.Waiting;
	public static GameStates State
	{
		get => _state;
		set
		{
			StateChanged?.Invoke(value, _state);
			_state = value;
		}
	}
	public static event GameStateChanged StateChanged;
	[Net] public static Jail Jail { get; set; }

	public Game()
	{
		if ( IsServer )
			_ = new Hud();
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		Player player = new Player { Role = State == GameStates.Waiting ? Roles.None : Roles.Spectator };

		client.Pawn = player;
	}

	[ServerCmd( "cr_start" )]
	public static async Task StartGame()
	{
		Log.Info( "Starting the game..." );
		List<Player> players = All.Where( entity => entity is Player ).Cast<Player>().ToList();
		if ( players.Count < 2 )
		{
			Log.Info( "Not enough players" );
			return;
		}
		
		State = GameStates.Starting;

		for ( int i = 0; i < Math.Round( (double)players.Count / 3 ); i++ )
		{
			var random = new Random().Next( 0, players.Count - 1 );
			players[random].Role = Roles.Cop;
			players.RemoveAt( random );
		}

		players.ForEach( player => player.Role = Roles.Runner );

		await System.Threading.Tasks.Task.Delay( 15000 );
		
		State = GameStates.Playing;
	}

	[ServerCmd( "cr_end" )]
	public static void EndGame()
	{
		Log.Info( "Ending the game..." );
		
		State = GameStates.Waiting;
		
		List<Player> players = All.Where( entity => entity is Player ).Cast<Player>().ToList();
		players.ForEach( player => player.Role = Roles.None );
	}
}
