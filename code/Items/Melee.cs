using copsandrunners.GameResources;
using Sandbox;

namespace copsandrunners.Items;

public abstract class Melee : Weapon
{
	public new MeleeInfo Info => (MeleeInfo)base.Info;
	protected float ForceMultiplier;
	protected TraceResult TraceResult;

	public override void AttackPrimary()
	{
		base.AttackPrimary();
		
		//ForceMultiplier = Rand.Float( Asset.RandMultiplier.x, Asset.RandMultiplier.y );
		
		// Outdated
		//_ = new Camera.Modifiers.MeleeShake( Asset.ShakeCurve, Asset.ShakeForce * ForceMultiplier );
		/*(((Player)Owner).CameraMode as Cameras.FirstPersonCamera)?.Shake( Asset.ShakeCurve,
			Asset.ShakeForce * ForceMultiplier ); // HOW THE FUCK SHAKING WORKS*/
		ViewModelEntity?.SetAnimParameter( "attack_hit", true );
		Sound.FromEntity( Info.SwingSound, Owner);
		
		TraceResult = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 50 )
			.WithTag( "player" )
			.Ignore( Owner )
			.Run();

		if ( TraceResult.Entity != null ) 
			Sound.FromEntity( Info.HitSound, Owner );
	}
}
