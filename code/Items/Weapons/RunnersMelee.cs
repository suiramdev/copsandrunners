using copsandrunners.Players;
using Sandbox;

namespace copsandrunners.Items.Weapons;

[ClassName("runners")]
public class RunnersMelee : Melee
{
	public override void AttackPrimary()
	{
		base.AttackPrimary();

		if ( !TraceResult.Hit || TraceResult.Entity is null )
			return;

		var targetPawn = (Players.Player)TraceResult.Entity;
		if ( IsServer )
		{
			if ( targetPawn.Role == Roles.Cop )
				targetPawn.Knock();

			TraceResult.Entity.Velocity += TraceResult.Direction * Info.Damages * ForceMultiplier;
		}

		if ( targetPawn.IsArrested )
		{
			targetPawn.Arrest( false );
				
			var particles = Particles.Create( "particles/confetti.vpcf" );
			particles.SetEntityBone( 0, TraceResult.Entity, 2 );
			Sound.FromWorld( Info.CustomSound, TraceResult.Entity.Position );
		}
	}
}
