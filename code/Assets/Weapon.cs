using Sandbox;

namespace copsandrunners.Assets;

[Library("weapon"), AutoGenerate]
public partial class Weapon : Asset
{
	[Property, ResourceType("vmdl")]
	public string WorldModel { get; set; }
	
	[Property, ResourceType("vmdl")]
	public string ViewModel { get; set; }
	
	[Property]
	public float Damages { get; set; }
}
