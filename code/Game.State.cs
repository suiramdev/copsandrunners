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
		
		var players = All.OfType<Player>().ToList();

		for ( var i = 0; i < Math.Round( (double)players.Count / 3 ); i++ )
		{
			var random = new Random().Next( 0, players.Count - 1 );
			players[random].Role = Roles.Cop;
			players.RemoveAt( random );
		}

		players.ForEach( player => player.Role = Roles.Runner );
		
		State = GameStates.Prepare;
		StateTimer = 15f;
		await WaitStateTimer();
		
		State = GameStates.Play;
		StateTimer = 20f;
		await WaitStateTimer();
		
		State = GameStates.End;
		StateTimer = 10f;
		await WaitStateTimer();
		
		players = All.OfType<Player>().ToList();
		players.ForEach( player => player.Role = Roles.None );
	}
	
	private async Task WaitStateTimer()
	{
		while ( StateTimer > 0 )
		{
			await Task.DelayRealtimeSeconds( 1.0f );
		}

		// extra second for fun
		await Task.DelayRealtimeSeconds( 1.0f );
	}

	private bool CanStart() => Client.All.Count > 1;
}
