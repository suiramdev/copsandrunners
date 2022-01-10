using Sandbox;

namespace copsandrunners;

public partial class Weapon : BaseWeapon
{
	public virtual string WorldModelPath => null;
	
	public override void Spawn()
	{
		base.Spawn();
		
		if (!string.IsNullOrEmpty(WorldModelPath)) SetModel( WorldModelPath );
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
