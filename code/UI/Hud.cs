using Sandbox;
using Sandbox.UI;

namespace copsandrunners.UI;

public class Hud : HudEntity<RootPanel>
{
	public Hud()
	{
		if (IsServer)
			return;

		RootPanel.AddChild<Inventory>();
	}
}
