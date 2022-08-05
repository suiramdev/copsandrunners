using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace copsandrunners.UI;

public class Phase : Panel
{
	private Label _phaseTimer;
	private Label _phaseName;
	
	public Phase()
	{
		_phaseTimer = Add.Label("00:00");
		_phaseName = Add.Label("");
		
		Update();
		Game.Instance.StateChanged += ( _, _ ) => Update();
		Game.Instance.WinnersChanged += ( _, _ ) => Update(); // I don't think that's the right way
	}

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
