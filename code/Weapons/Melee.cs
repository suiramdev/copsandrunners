using Sandbox;

namespace copsandrunners.Weapons;

public abstract class Melee : Weapon
{
	public virtual Assets.Melee Asset { get; }
	protected float _forceMultiplier;
	protected TraceResult _traceResult;

	public override void AttackPrimary()
	{
		base.AttackPrimary();
		
		_forceMultiplier = Rand.Float( Asset.RandMultiplier.x, Asset.RandMultiplier.y );
		
		_ = new Camera.Modifiers.MeleeShake( Asset.ShakeCurve, Asset.ShakeForce * _forceMultiplier );
		ViewModelEntity?.SetAnimBool( "attack_hit", true );
		Sound.FromEntity( "woosh.melee", Owner);
		
		_traceResult = Trace.Ray( Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 50 )
			.HitLayer( CollisionLayer.All, false )
			.HitLayer( CollisionLayer.Player )
			.Ignore( Owner )
			.Run();

		if ( _traceResult.Entity != null )
		{
			Sound.FromEntity( "punch.melee", Owner );
			Sound.FromEntity( "hit.melee", Owner );
		}
	}
}
