using Sandbox;

namespace copsandrunners.Weapons;

public abstract class Weapon : BaseWeapon
{
	public virtual Assets.Melee Asset { get; }
	protected virtual string WorldModelPath { get; }
	public override string ViewModelPath { get; }

	public override void Spawn()
	{
		base.Spawn();

		if (!string.IsNullOrEmpty(WorldModelPath)) SetModel( WorldModelPath );
	}

	public override void AttackPrimary()
	{
		if ( ((Player)Owner).IsArrested ) return;
		
		base.AttackPrimary();
	}

	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( ViewModelPath ) )
			return;

		ViewModelEntity = new ViewModel
		{
			Position = Position,
			Owner = Owner,
			EnableViewmodelRendering = true
		};
		ViewModelEntity.SetModel( ViewModelPath );
	}
}
