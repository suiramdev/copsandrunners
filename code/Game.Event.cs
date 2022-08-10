using Sandbox;

namespace copsandrunners;

public class GameEvent
{
	public const string StateChanged = "StateChanged";

	public class StateChangedAttribute : EventAttribute
	{
		public StateChangedAttribute() : base(StateChanged) { }
	}
	
	public const string WinnersChanged = "WinnersChanged";

	public class WinnersChangedAttribute : EventAttribute
	{
		public WinnersChangedAttribute() : base(WinnersChanged) { }
	}
}
