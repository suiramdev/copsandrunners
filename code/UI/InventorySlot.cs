using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace copsandrunners.UI;

public class InventorySlot : Panel
{
	private Label _name;
	private Label _slot;

	public InventorySlot( int slot, Panel parent )
	{
		Parent = parent;
		_name = Add.Label( "Empty" );
		_slot = Add.Label( $"{slot}" );
	}
}
