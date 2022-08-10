﻿using Sandbox;

namespace copsandrunners.Assets;

[GameResource("Melee Weapon", "melee", "A melee weapon.")]
public class Melee : GameResource
{
	[ResourceType("vmdl")]
	public string WorldModel { get; set; }
	
	[ResourceType("vmdl")]
	public string ViewModel { get; set; }
	
	public float Damages { get; set; }
	
	public Vector2 RandMultiplier { get; set; }

	public float ShakeForce { get; set; }
	
	public FGDCurve ShakeCurve { get; set; }
}
