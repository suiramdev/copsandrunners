using Sandbox;

namespace copsandrunners.GameResources;

[GameResource( "Melee Weapon", "melee", "Melee weapon template", Icon = "🗡️" )]
public class MeleeInfo : WeaponInfo
{
	[Category("Stats"), Description("Damages caused by the weapon")]
	public float Damages { get; set; }
	
	[Category("Effects")]
	public Vector2 RandMultiplier { get; set; }

	[Category("Effects")]
	public float ShakeForce { get; set; }
	
	[Category("Effects")]
	public FGDCurve ShakeCurve { get; set; }
}
