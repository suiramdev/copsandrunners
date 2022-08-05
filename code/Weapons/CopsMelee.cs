using Sandbox;

namespace copsandrunners.Weapons;

public class CopsMelee : Melee
{
	public override Assets.Melee Asset => ResourceLibrary.Get<Assets.Melee>( "config/weapons/cops.melee" );

	public override void AttackPrimary()
	{
		base.AttackPrimary();

		if ( !TraceResult.Hit || !TraceResult.Entity.IsValid || ((Player)TraceResult.Entity).IsArrested )
			return;

		((Player)TraceResult.Entity).Arrest( true );

		Sound.FromWorld( "arrest.whistle", TraceResult.Entity.Position );
	}
}
