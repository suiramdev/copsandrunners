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
	[Net] public Teams Winners { get; set; } = Teams.None;
	
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
		StateTimer = 8f;
		await WaitStateTimer();
		
		// Set players roles and positions
		var players = All.OfType<Player>().ToList();

		var chiefSelected = false;
		for ( var i = 0; i < Math.Round( (double)players.Count / 3 ); i++ )
		{
			var random = new Random().Next( 0, players.Count - 1 );
			if ( chiefSelected )
			{
				players[random].Role = Roles.Cop;
			}
			else
			{
				chiefSelected = true;
				players[random].Role = Roles.ChiefCop;
			}
			players[random].Team = Teams.Cops;
			players.RemoveAt( random );
		}

		players.ForEach( player =>
		{
			player.Team = Teams.Runners;
			player.Role = Roles.Runner;
		});
	
		// TODO: Set player positions randomly in the map
		
		State = GameStates.Prepare;
		StateTimer = 15f;
		await WaitStateTimer();
		
		State = GameStates.Play;
		StateTimer = 120f;
		await WaitStateTimer(HasCopWon);

		Winners = HasCopWon() ? Teams.Cops : Teams.Runners;

		State = GameStates.End;
		StateTimer = 10f;
		await WaitStateTimer();

		Winners = Teams.None;
		players = All.OfType<Player>().ToList();
		players.ForEach( player => {
			player.Arrest(false);	
			player.Role = Roles.None;
			player.Team = Teams.None;
		});
	
		if (Jail.IsValid)
			Jail.Delete();

		await GameLoop();
	}
	
	// stopException is used to stop the loop if it is returning true
	private async Task WaitStateTimer(Func<bool> stopException = null)
	{
		while ( StateTimer > 0 )
		{
			if ( stopException != null && stopException.Invoke() )
				break;

			await Task.DelayRealtimeSeconds( 1.0f );
		}
	}

	private bool CanStart() => Client.All.Count > 1;
	
	private bool HasCopWon()
	{
		var runners = All.OfType<Player>().Where( player => player.Team == Teams.Runners).ToList();
		var arrested = runners.Where( player => player.IsArrested ).ToList();
		
		return runners.Count <= arrested.Count;
	}
}
