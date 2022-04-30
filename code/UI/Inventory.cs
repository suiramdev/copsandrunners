using System;
using Sandbox;
using Sandbox.UI;

namespace copsandrunners.UI;

public class Inventory : Panel
{
	[Event.BuildInput]
	public void ProcessBuildInput( InputBuilder inputBuilder )
	{
		for ( var i = 1; i < 9; i++ )
			if ( inputBuilder.Pressed( (InputButton)Enum.Parse( typeof(InputButton), $"Slot{i}" ) ) )
				SetActiveSlot( inputBuilder, (Local.Pawn as Player)?.Inventory, i - 1 );
	}

	private static void SetActiveSlot( InputBuilder inputBuilder, IBaseInventory inventory, int slot )
	{
		inventory.SetActiveSlot( slot, false );
		inputBuilder.ActiveChild = inventory.GetSlot( inventory.GetActiveSlot() );
	}
}
