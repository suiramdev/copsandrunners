using Sandbox;

namespace copsandrunners.Weapons;

public class CopsMelee : Melee
{
	public override Assets.Melee Asset => Assets.Asset.FromPath<Assets.Melee>( "config/cops.melee" );

	public override void AttackPrimary()
	{
		base.AttackPrimary();

		if ( _traceResult.Entity != null && !((Player)_traceResult.Entity).IsArrested)
		{
			((Player)_traceResult.Entity).Arrest( true );

			Sound.FromWorld( "arrest.whistle", _traceResult.Entity.Position );
		}
	}
}
