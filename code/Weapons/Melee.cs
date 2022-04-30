using Sandbox;

namespace copsandrunners.Weapons;

public abstract class Melee : Weapon
{
	public override Assets.Melee Asset { get; }
	protected float ForceMultiplier;
	protected TraceResult TraceResult;

	public override void AttackPrimary()
	{
		base.AttackPrimary();
		
		ForceMultiplier = Rand.Float( Asset.RandMultiplier.x, Asset.RandMultiplier.y );
		
		_ = new Camera.Modifiers.MeleeShake( Asset.ShakeCurve, Asset.ShakeForce * ForceMultiplier );
		ViewModelEntity?.SetAnimParameter( "attack_hit", true );
		Sound.FromEntity( "woosh.melee", Owner);
		
		TraceResult = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 50 )
			.HitLayer( CollisionLayer.All, false )
			.HitLayer( CollisionLayer.Player )
			.Ignore( Owner )
			.Run();

		if ( TraceResult.Entity != null )
		{
			Sound.FromEntity( "punch.melee", Owner );
			Sound.FromEntity( "hit.melee", Owner );
		}
	}
}
