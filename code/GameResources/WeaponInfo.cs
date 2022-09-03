using Sandbox;

namespace copsandrunners.GameResources;

[GameResource( "Weapon", "weapon", "Weapon template", Icon = "🔫" )]
public class WeaponInfo : CarriableInfo
{
	[Category("Sounds"), ResourceType("sound"), Description("Used on some weapons, for some events.")]
	public string CustomSound { get; set; }

	[Category("Stats"), Description("Cooldown between shots on primary fire")]
	public float PrimaryRate { get; set; } = -1f;

	[Category( "Stats" ), Description( "Cooldown between shots on secondary fire" )]
	public float SecondaryRate { get; set; } = -1f;
}
