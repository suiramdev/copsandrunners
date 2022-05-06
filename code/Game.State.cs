using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;

namespace copsandrunners;

public enum GameStates
{
	Wait,
	Start,
	Prepare,
	Play,
	End
}

public delegate void StateChanged( GameStates newState, GameStates lastState );

internal partial class Game
{
	[Net, Change( nameof(OnStateChanged) )]
	public GameStates State { get; set; } = GameStates.Wait;
	[Net] public RealTimeUntil StateTimer { get; set; } = 0f;
	
	public event StateChanged StateChanged;
	
	public void OnStateChanged( GameStates newState, GameStates lastState )
	{
		StateChanged?.Invoke( newState, lastState );
	}

	private async Task GameLoop()
	{
		while ( !CanStart() )
		{
			await Task.DelayRealtimeSeconds( 1.0f );
		}

		State = GameStates.Start;
		StateTimer = 20f;
		await WaitStateTimer();
		
		// Set players roles and positions
		var players = All.OfType<Player>().ToList();

		for ( var i = 0; i < Math.Round( (double)players.Count / 3 ); i++ )
		{
			var random = new Random().Next( 0, players.Count - 1 );
			players[random].Role = Roles.Cop;
			players.RemoveAt( random );
		}

		players.ForEach( player => player.Role = Roles.Runner );
	
		// TODO: Set player positions randomly in the map
		
		State = GameStates.Prepare;
		StateTimer = 15f;
		await WaitStateTimer();
		
		State = GameStates.Play;
		StateTimer = 20f;
		await WaitStateTimer(ShouldEnd);
		
		State = GameStates.End;
		StateTimer = 10f;
		await WaitStateTimer();
		
		players = All.OfType<Player>().ToList();
		players.ForEach( player => {
			player.Arrest(false);	
			player.Role = Roles.None;
		});
		
		await GameLoop();
	}
	
	// stopException is used to stop the loop if it is returning true
	private async Task WaitStateTimer(Func<bool> stopException = null)
	{
		while ( StateTimer > 0 )
		{
			if ( stopException != null && stopException.Invoke() )
			{
				Log.Info( "Game loop stopped" );
				break;
			}

			await Task.DelayRealtimeSeconds( 1.0f );
		}
	}

	private bool CanStart() => Client.All.Count > 1;
	
	private bool ShouldEnd()
	{
		var runners = All.OfType<Player>().Where( player => player.Role == Roles.Runner ).ToList();
		var arrested = runners.Where( player => player.IsArrested ).ToList();
		
		return runners.Count <= arrested.Count;
	}
}
