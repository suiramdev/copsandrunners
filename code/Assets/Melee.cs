using Sandbox;

namespace copsandrunners.Assets;

[Library("melee"), AutoGenerate]
public class Melee : Weapon
{
	[Property]
	public Vector2 RandMultiplier { get; set; }

	[Property]
	public float ShakeForce { get; set; }
	
	[Property]
	public FGDCurve ShakeCurve { get; set; }
}
