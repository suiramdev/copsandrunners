using Sandbox;

namespace copsandrunners.Weapons;

public class RobbersMelee : Melee
{
	public override Assets.Melee Asset => Assets.Asset.FromPath<Assets.Melee>( "config/robbers.melee" );

	public override void AttackPrimary()
	{
		base.AttackPrimary();
		
		if ( _traceResult.Entity != null )
		{
			var targetPawn = (PlayerPawn)_traceResult.Entity;
			if (IsServer)
				if (targetPawn.Role == Roles.Cop)
					Log.Info( "Knock" );
				else
					_traceResult.Entity.Velocity += _traceResult.Direction * Asset.Damages * _forceMultiplier;

			if ( targetPawn.IsJailed )
			{
				targetPawn.IsJailed = false;
				
				var particles = Particles.Create( "particles/confetti.vpcf" );
				particles.SetEntityBone( 0, _traceResult.Entity, 2 );
				Sound.FromEntity( "horn.slap", _traceResult.Entity );
			}
		}
	}
}
