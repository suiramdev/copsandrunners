using Sandbox;

namespace copsandrunners.Entities;

public class JailCollisions : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/jail.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		
		Tags.Add( "ArrestNoCollide" );
	}
}
