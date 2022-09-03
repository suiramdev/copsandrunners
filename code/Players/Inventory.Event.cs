using Sandbox;

namespace copsandrunners.Players;

public static class InventoryEvent
{
	public const string ChildAdded = "ChildAdded";

	public class ChildAddedAttribute : EventAttribute
	{
		public ChildAddedAttribute() : base(ChildAdded) { }
	}
	
	public const string ChildRemoved = "ChildRemoved";

	public class ChildRemovedAttribute : EventAttribute
	{
		public ChildRemovedAttribute() : base(ChildRemoved) { }
	}
	
	public const string SetActiveSlot = "SetActiveSlot";
	
	public class SetActiveSlotAttribute : EventAttribute
	{
		public SetActiveSlotAttribute() : base(SetActiveSlot) { }
	}
}
