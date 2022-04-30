using copsandrunners.Entities;
using Sandbox;

namespace copsandrunners.Weapons;

public class JailPlacer : Weapon
{
	public override string ViewModelPath => "weapons/rust_flashlight/v_rust_flashlight.vmdl";
	protected override string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl";

	public override void AttackPrimary()
	{
		base.AttackPrimary();

		if ( Game.Jail != null ) Game.Jail.Delete();
		Game.Jail = new Jail { Position = Owner.Position };
		
		if (IsServer)
			Delete();
	}
}
