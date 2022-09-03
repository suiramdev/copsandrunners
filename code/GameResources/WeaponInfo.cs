using Sandbox;

namespace copsandrunners.GameResources;

[GameResource( "Weapon", "weapon", "Weapon template", Icon = "🔫" )]
public class WeaponInfo : CarriableInfo
{
	[Category("Sounds"), ResourceType("sound")]
	public string PrimarySound { get; set; }
	
	[Category("Sounds"), ResourceType("sound")]
	public string SecondarySound { get; set; }
	
	[Category("Stats"), Description("Cooldown between shots on primary fire")]
	public float PrimaryRate { get; set; }
	
	[Category("Stats"), Description("Cooldown between shots on secondary fire")]
	public float SecondaryRate { get; set; }
}
