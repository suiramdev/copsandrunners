using Sandbox;

namespace copsandrunners.Weapons;

public class RunnersMelee : Melee
{
	public override Assets.Melee Asset => ResourceLibrary.Get<Assets.Melee>( "config/weapons/runners.melee" );

	public override void AttackPrimary()
	{
		base.AttackPrimary();

		if ( !TraceResult.Hit || TraceResult.Entity == null )
			return;

		var targetPawn = (Player)TraceResult.Entity;
		if ( IsServer )
		{
			if ( targetPawn.Role == Roles.Cop )
				targetPawn.Knock();

			TraceResult.Entity.Velocity += TraceResult.Direction * Asset.Damages * ForceMultiplier;
		}

		if ( targetPawn.IsArrested )
		{
			targetPawn.Arrest( false );
				
			var particles = Particles.Create( "particles/confetti.vpcf" );
			particles.SetEntityBone( 0, TraceResult.Entity, 2 );
			Sound.FromWorld( "horn", TraceResult.Entity.Position );
		}
	}
}
