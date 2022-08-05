using Sandbox;

namespace copsandrunners.Assets;

[GameResource("Swep", "swep", "A swep.")]
public partial class Swep : GameResource
{
	[Property, ResourceType("vmdl")]
	public string WorldModel { get; set; }
	
	[ResourceType("vmdl")]
	public string ViewModel { get; set; }
}
