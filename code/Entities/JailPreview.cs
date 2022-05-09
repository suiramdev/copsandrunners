using Sandbox;

namespace copsandrunners.Entities;

public class JailPreview : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/pointer.vmdl" );
	}
}
