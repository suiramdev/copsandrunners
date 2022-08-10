using Sandbox;

namespace copsandrunners.Weapons;

public abstract class Melee : Weapon
{
	public virtual Assets.Melee Asset { get; }
	public override string ViewModelPath => Asset.ViewModel;
	protected override string WorldModelPath => Asset.WorldModel;
	protected float ForceMultiplier;
	protected TraceResult TraceResult;

	public override void AttackPrimary()
	{
		base.AttackPrimary();
		
		//ForceMultiplier = Rand.Float( Asset.RandMultiplier.x, Asset.RandMultiplier.y );
		
		// Outdated
		//_ = new Camera.Modifiers.MeleeShake( Asset.ShakeCurve, Asset.ShakeForce * ForceMultiplier );
		(((Player)Owner).CameraMode as Camera.FirstPersonCamera)?.Shake( Asset.ShakeCurve,
			Asset.ShakeForce * ForceMultiplier ); // HOW THE FUCK SHAKING WORKS
		ViewModelEntity?.SetAnimParameter( "attack_hit", true );
		Sound.FromEntity( "woosh.melee", Owner);
		
		TraceResult = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 50 )
			.WithTag( "player" )
			.Ignore( Owner )
			.Run();

		if ( TraceResult.Entity != null )
		{
			Sound.FromEntity( "punch.melee", Owner );
			Sound.FromEntity( "hit.melee", Owner );
		}
	}
}
