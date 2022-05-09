using System;
using copsandrunners.Entities;
using copsandrunners.UI;
using Sandbox;
using Sandbox.UI;

namespace copsandrunners.Weapons;

public class JailPlacer : Weapon
{
	public override string ViewModelPath => "weapons/rust_flashlight/v_rust_flashlight.vmdl";
	protected override string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl";

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
			.HitLayer( CollisionLayer.Player, false )
			.Ignore( Owner )
			.Run();
			
		var downTraceResult = Trace.Ray( fwdTraceResult.EndPosition + Vector3.Up * 10f, fwdTraceResult.EndPosition + Vector3.Down * 200f )
			.HitLayer( CollisionLayer.Player, false )
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
			Game.Jail?.Delete();
			Game.Jail = new Jail { Position = Position };
			Delete();
		}
	}
}
