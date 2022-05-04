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
		
		Game.StateChanged += OnStateChanged;
	}

	private void Update()
	{
		switch ( Game.State )
		{
			case GameStates.Waiting:
				_phaseTimer.Style.Display = DisplayMode.None;
				_phaseName.Style.Display = DisplayMode.Flex;
				_phaseName.Text = "Waiting for more players";
				break;
			default:
				_phaseTimer.Style.Display = DisplayMode.Flex;
				_phaseName.Style.Display = DisplayMode.Flex;
				break;
		}
	}
	
	private void OnStateChanged( GameStates newstate, GameStates laststate )
	{
		Update();
	}
	
}
