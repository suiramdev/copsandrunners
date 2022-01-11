using Sandbox;

namespace copsandrunners.Weapons;

public class CopsMelee : Weapon
{
	public override Assets.Melee Asset => Assets.Asset.FromPath<Assets.Melee>( "config/cops.melee" );
}
