using Sandbox;

namespace copsandrunners.Weapons;

public class Slap : Weapon
{
	public Assets.Slap Asset { get; } = Assets.Asset.FromPath<Assets.Slap>( "config/weapon.slap" );
	
	public override void AttackPrimary()
	{
		base.AttackPrimary();

		float forceMultiplier = Rand.Float( Asset.RandMultiplier.x, Asset.RandMultiplier.y );

		new CameraModifiers.SlapShake( Asset.ShakeCurve, Asset.ShakeForce * forceMultiplier );
		ViewModelEntity?.SetAnimBool( "attack_hit", true );
		Sound.FromEntity( "woosh.slap", Owner );
		
		TraceResult trace = Trace.Ray( Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 50 )
			.HitLayer( CollisionLayer.All, false )
			.HitLayer( CollisionLayer.Player )
			.Ignore( Owner )
			.Run();

		if ( trace.Entity != null )
		{
			if (IsServer)
				trace.Entity.Velocity += trace.Direction * Asset.SlapPower * forceMultiplier;
			Sound.FromEntity( "hit.slap", Owner );
		}
	}
}
