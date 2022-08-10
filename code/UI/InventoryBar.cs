using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace copsandrunners.UI;

public class InventoryBar : Panel
{
	private readonly List<InventorySlot> _slots = new();

	[InventoryEvent.ChildAdded, InventoryEvent.ChildRemoved]
	public void Rebuild()
	{
		Log.Info( "ddd" );
		_slots.Clear();
		
		var inventory = ((Player)Local.Pawn)?.Inventory;
		if ( inventory is null ) return;

		for ( var i = 0; i < inventory.Count(); i++ )
		{
			var slot = new InventorySlot( i, this );
			slot.SetClass( "active", inventory.GetActiveSlot() == i );
			_slots.Add( slot );
		}
	}
	
	[InventoryEvent.SetActiveSlot]
	public void Update()
	{
		var inventory = ((Player)Local.Pawn)?.Inventory;
		if ( inventory is null ) return;
		
		for ( var i = 0; i < _slots.Count; i++ )
			_slots[i].SetClass( "active", inventory.GetActiveSlot() == i );
	}
}
