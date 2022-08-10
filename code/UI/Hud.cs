using Sandbox;
using Sandbox.UI;

namespace copsandrunners.UI;

public class Hud : RootPanel
{
	public Hud()
	{
		StyleSheet.Load( "/UI/Hud.scss" );

		AddChild<InventoryBar>();
		AddChild<Phase>();
	}
}
