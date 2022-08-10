using System;
using System.Diagnostics;
using copsandrunners.Entities;
using copsandrunners.Utils;
using Sandbox;

namespace copsandrunners.Weapons;

public class ThrowingBall : Weapon
{
	public Assets.Weapon Asset => ResourceLibrary.Get<Assets.Weapon>( "config/weapons/ball.weapon" );
	public override string ViewModelPath => Asset.ViewModel;
	protected override string WorldModelPath => Asset.WorldModel;

	private const float Force = 800f;

	public override async void AttackPrimary()
	{
		base.AttackPrimary();

		if ( !IsServer )
			return;
		
		var throwData = ProjectileMotion.ThrowFromAngle( Force, 60f, Owner.EyeRotation.Yaw(), 0.1f );
		Log.Info( "Velocity: " + throwData.Velocity );
		Log.Info( "Up ToF: " + throwData.UpTime + "s" );
		Log.Info( "Total ToF : " + throwData.TotalTime + "s" );
		Log.Info( "EndPos: " + throwData.EndPosition );
		var startingPosition = Position + Owner.Rotation.Forward * 100f;
		
		var ball = new Ball { Position = startingPosition, Velocity = throwData.Velocity };
		await Task.DelaySeconds( throwData.TotalTime );
		ball.Delete();
	}

	public override void Simulate( Client player )
	{
		base.Simulate( player );

		var startingPosition = Position + Owner.Rotation.Forward * 100f;
		
		var throwData = ProjectileMotion.ThrowFromAngle( Force, Math.Clamp( -Owner.EyeRotation.Pitch(), 0, 90 ), Owner.EyeRotation.Yaw(), 0.1f );
		DebugOverlay.Sphere( startingPosition, 5f, Color.Red );
		DebugOverlay.Sphere( startingPosition + throwData.EndPosition, 5f, Color.Red );
		DebugOverlay.Line( startingPosition, startingPosition + throwData.Velocity, Color.Green );
		DebugOverlay.Line( startingPosition, startingPosition + throwData.EndPosition, Color.Blue );
		DebugOverlay.Sphere( startingPosition + (throwData.EndPosition / 2).WithZ( throwData.Height ), 5f, Color.Red );

	}
}
