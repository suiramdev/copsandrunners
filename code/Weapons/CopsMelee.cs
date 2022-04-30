using Sandbox;

namespace copsandrunners.Weapons;

public class CopsMelee : Melee
{
	public override Assets.Melee Asset => Assets.Asset.FromPath<Assets.Melee>( "config/cops.melee" );

	public override void AttackPrimary()
	{
		base.AttackPrimary();

		if ( TraceResult.Entity != null && !((Player)TraceResult.Entity).IsArrested)
		{
			((Player)TraceResult.Entity).Arrest( true );

			Sound.FromWorld( "arrest.whistle", TraceResult.Entity.Position );
		}
	}
}
