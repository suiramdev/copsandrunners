using Sandbox;

namespace copsandrunners.Assets;

[Library("slap"), AutoGenerate]
public partial class Slap : Weapon
{
	[Property]
	public Vector2 RandMultiplier { get; set; }
	
	[Property]
	public float SlapPower { get; set; }

	[Property]
	public float ShakeForce { get; set; }
	
	[Property]
	public FGDCurve ShakeCurve { get; set; }
}
