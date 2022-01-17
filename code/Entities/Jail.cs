using Sandbox;

namespace copsandrunners.Entities;

public class Jail : ModelEntity
{
	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/jail.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		
		Tags.Add( "Jail" );
	}
}
