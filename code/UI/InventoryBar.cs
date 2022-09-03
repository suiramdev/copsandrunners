using System.Collections.Generic;
using copsandrunners.Players;
using Sandbox;
using Sandbox.UI;

namespace copsandrunners.UI;

public class InventoryBar : Panel
{
	private static readonly InputButton[] SlotInputs = {
		InputButton.Slot1,
		InputButton.Slot2,
		InputButton.Slot3,
		InputButton.Slot4,
		InputButton.Slot5,
		InputButton.Slot6,
		InputButton.Slot7,
		InputButton.Slot8,
		InputButton.Slot9,
		InputButton.Slot0
	};
	
	private readonly List<InventorySlot> _slots = new();

	[InventoryEvent.ItemAdded, InventoryEvent.ItemRemoved]
	public void Rebuild()
	{
		Log.Info( "ddd" );
		_slots.Clear();
		
		var inventory = ((Players.Player)Local.Pawn)?.Inventory;
		if ( inventory is null ) return;

		for ( var i = 0; i < inventory.Count; i++ )
		{
			var slot = new InventorySlot( i, this );
			slot.SetClass( "active", inventory.ActiveSlot == i );
			_slots.Add( slot );
		}
	}
	
	[InventoryEvent.SetActive]
	public void Update()
	{
		var inventory = ((Players.Player)Local.Pawn)?.Inventory;
		if ( inventory is null ) return;
		
		for ( var i = 0; i < _slots.Count; i++ )
			_slots[i].SetClass( "active", inventory.ActiveSlot == i );
	}

	[Event.BuildInput]
	private void BuildInput( InputBuilder inputBuilder )
	{
		for ( var i = 0; i < SlotInputs.Length; i++ )
			if ( inputBuilder.Pressed( SlotInputs[i] ) )
				((Players.Player)Local.Pawn)?.Inventory.SetActive( i );
	}
}
