using Sandbox;

namespace copsandrunners.Weapons;

public class CopsMelee : Melee
{
	public override Assets.Melee Asset => Assets.Asset.FromPath<Assets.Melee>( "config/cops.melee" );

	public override void AttackPrimary()
	{
		base.AttackPrimary();
		
		if ( _traceResult.Entity != null && !((PlayerPawn)_traceResult.Entity).IsJailed)
		{
			((PlayerPawn)_traceResult.Entity).IsJailed = true;

			Sound.FromWorld( "arrest.whistle", _traceResult.Entity.Position );
		}
	}
}
