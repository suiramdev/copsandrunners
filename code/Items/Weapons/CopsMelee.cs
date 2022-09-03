using Sandbox;

namespace copsandrunners.Items.Weapons;

[ClassName("cops")]
public class CopsMelee : Melee
{
	public override void AttackPrimary()
	{
		base.AttackPrimary();

		if ( !TraceResult.Hit || !TraceResult.Entity.IsValid || ((Players.Player)TraceResult.Entity).IsArrested )
			return;

		((Players.Player)TraceResult.Entity).Arrest( true );

		Sound.FromWorld( Info.CustomSound, TraceResult.Entity.Position );
	}
}
