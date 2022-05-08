using Sandbox;

namespace copsandrunners.Entities;

public class Jail : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/ball_pit.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		new JailCollisions()
			.SetParent( this );
	}
}
