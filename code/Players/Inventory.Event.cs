using Sandbox;

namespace copsandrunners.Players;

public static class InventoryEvent
{
	public const string ItemAdded = "ItemAdded";

	public class ItemAddedAttribute : EventAttribute
	{
		public ItemAddedAttribute() : base(ItemAdded) { }
	}
	
	public const string ItemRemoved = "ItemRemoved";

	public class ItemRemovedAttribute : EventAttribute
	{
		public ItemRemovedAttribute() : base(ItemRemoved) { }
	}
	
	public const string SetActive = "SetActive";
	
	public class SetActiveAttribute : EventAttribute
	{
		public SetActiveAttribute() : base(SetActive) { }
	}
}
