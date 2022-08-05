using Sandbox;

namespace copsandrunners.Entities;

public class Ball : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/ball.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}
}
