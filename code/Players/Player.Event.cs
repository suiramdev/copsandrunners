using Sandbox;

namespace copsandrunners.Players;

public class PlayerEvent
{
	public const string Arrested = "Arrested";

	public class ArrestedAttribute : EventAttribute
	{
		public ArrestedAttribute() : base(Arrested) { }
	}
	
	public const string Knocked = "Knocked";

	public class KnockedAttribute : EventAttribute
	{
		public KnockedAttribute() : base(Knocked) { }
	}
}
