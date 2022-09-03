using System;
using copsandrunners.Players;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace copsandrunners.UI;

public class Phase : Panel
{
	private readonly Label _phaseTimer;
	private readonly Label _phaseName;
	
	public Phase()
	{
		_phaseTimer = Add.Label("00:00");
		_phaseName = Add.Label("");
		
		Update();
	}

	[GameEvent.WinnersChanged, GameEvent.StateChanged]
	private void Update()
	{
		_phaseTimer.Style.Display = DisplayMode.Flex;
		_phaseName.Style.Display = DisplayMode.Flex;

		switch ( Game.Instance.State )
		{
			case GameStates.Wait:
				_phaseTimer.Style.Display = DisplayMode.None;
				_phaseName.Text = "Waiting for more players";
				break;
			case GameStates.Start:
				_phaseName.Text = "The game is about to start";
				break;
			case GameStates.Prepare:
				_phaseName.Text = "Prepare to chase !";
				break;
			case GameStates.Play:
				_phaseName.Style.Display = DisplayMode.None;
				break;
			case GameStates.End:
				_phaseName.Text = Game.Instance.Winners == Teams.Cops ? "All runners were arrested" : "Runners escaped from the police";
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public override void Tick()
	{
		base.Tick();
		
		_phaseTimer.Text = Game.Instance.StateTimer > 0f ? 
			TimeSpan.FromSeconds( Game.Instance.StateTimer ).ToString( @"mm\:ss" )
			: "00:00";
	}
}
