using copsandrunners.Entities;
using Sandbox;

namespace copsandrunners.Items.Weapons;

[ClassName("jail_placer")]
public class JailPlacer : Weapon
{
	private Entity _preview;

	public new Vector3 Position;
	
	public override void CreateViewModel()
	{
		base.CreateViewModel();

		_preview = new JailPreview();
	}
	
	public override void Simulate( Client player )
	{
		base.Simulate( player );

		var fwdTraceResult = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 200f )
			.WithoutTags( "player" )
			.Ignore( Owner )
			.Run();
			
		var downTraceResult = Trace.Ray( fwdTraceResult.EndPosition + Vector3.Up * 10f, fwdTraceResult.EndPosition + Vector3.Down * 200f )
			.WithoutTags( "player" )
			.Ignore( Owner )
			.Run();
		
		if (IsClient) 
			_preview.Position = downTraceResult.EndPosition;
		else
			Position = downTraceResult.EndPosition;
	}

	public override void DestroyViewModel()
	{
		base.DestroyViewModel();
		
		_preview?.Delete();
	}

	public override void AttackPrimary()
	{
		base.AttackPrimary();
		
		if ( IsServer )
		{
			Game.Instance.Jail?.Delete();
			Game.Instance.Jail = new Jail { Position = Position };
			Delete();
		}
	}
}
