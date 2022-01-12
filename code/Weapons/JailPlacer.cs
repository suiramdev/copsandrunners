using Sandbox;

namespace copsandrunners.Weapons;

public class JailPlacer : Weapon
{
	public override string ViewModelPath => "weapons/rust_flashlight/v_rust_flashlight.vmdl";
	public override string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl";

	public override void AttackPrimary()
	{
		base.AttackPrimary();

		Game.JailPosition = Owner.Position;
		Log.Info( "Jail placed !" );
	}
}
