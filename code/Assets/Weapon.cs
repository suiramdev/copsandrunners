﻿using Sandbox;

namespace copsandrunners.Assets;

[GameResource("Weapon", "weapon", "A simple weapon.")]
public class Weapon : GameResource
{
	[ResourceType("vmdl")]
	public string WorldModel { get; set; }
	
	[ResourceType("vmdl")]
	public string ViewModel { get; set; }
}
