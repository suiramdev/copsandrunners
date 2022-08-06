using System;
using System.Diagnostics;
using System.Threading.Tasks;
using copsandrunners.Entities;
using Sandbox;
using Trace = Sandbox.Trace;

namespace copsandrunners.Weapons;

public struct ThrowData
{
	public Vector3 Velocity;
	public float Time;
	public Vector3 StartPosition;
	public Vector3 EndPosition;
	public Vector3 Displacement;
}

public class ThrowingBall : Weapon
{
	public Assets.Weapon Asset => ResourceLibrary.Get<Assets.Weapon>( "config/weapons/ball.weapon" );
	public override string ViewModelPath => Asset.ViewModel;
	protected override string WorldModelPath => Asset.WorldModel;

	private const float Gravity = 9.8f;

	public override async void AttackPrimary()
	{
		base.AttackPrimary();

		if ( IsServer )
		{
			var throwData = ThrowFromAngle(Owner.EyePosition, Math.Clamp( -Owner.EyeRotation.Pitch(), 0f, 90f ).DegreeToRadian(), Owner.EyeRotation.Yaw().DegreeToRadian(), 100f, Gravity );
			var ball = new Ball
			{
				Position = Owner.EyePosition,
				Velocity = throwData.Velocity
			};

			await Task.Delay( (int)MathF.Round( throwData.Time * 1000 ) );
		
			ball.Delete();	
		}
	}

	// Debugging part
	public override void Simulate( Client player )
	{
		base.Simulate( player );
		var throwData = ThrowFromAngle(Position, Math.Clamp(-Owner.EyeRotation.Pitch(), 0, 90).DegreeToRadian(), Owner.EyeRotation.Yaw().DegreeToRadian(), 100f, Gravity );
		
		/*var previousPosition = Position;
		const float resolution = 50f;
		for ( var i = 0; i < resolution; i++ )
		{
			var simulationTime = i / resolution * throwData.Time;
			var displacement = throwData.Velocity * simulationTime + Vector3.Down * Gravity * simulationTime * simulationTime / 2f;
			var position = Position + displacement;
			DebugOverlay.Line( previousPosition, position, Color.Green );
			previousPosition = position;
		}*/
		
		const float resolution = 5f;
		for ( var i = 0; i < resolution; i++ )
		{
			var displacement = throwData.Displacement / i;
			DebugOverlay.Sphere( throwData.StartPosition + displacement, 10f, Color.Red);
		}
		
		DebugOverlay.Sphere( throwData.StartPosition, 10f, Color.Red);
		DebugOverlay.Sphere( throwData.EndPosition, 10f, Color.Red);
	}

	// Explain the following code:
	private static ThrowData ThrowFromAngle( Vector3 startPosition, float pitchAngle, float yawAngle, float speed, float gravity )
	{
		var velocity = new Vector3(
			speed * MathF.Cos( pitchAngle ) * MathF.Cos( yawAngle ),
			speed * MathF.Cos( pitchAngle ) * MathF.Sin( yawAngle ),
			speed * MathF.Sin( pitchAngle ) - gravity
		);
		var time = startPosition.z > 0 ?
			(speed * MathF.Sin( pitchAngle ) + MathF.Sqrt( MathF.Pow( speed * MathF.Sin( pitchAngle ), 2 ) + 2 * gravity * startPosition.z )) / gravity
			: -velocity.z / gravity * 2;
		var displacement = velocity * time + Vector3.Down * gravity;
		var endPosition = (startPosition + displacement).WithZ( 0 );

		return new ThrowData
		{
			Velocity = velocity,
			Time = time,
			StartPosition = startPosition,
			EndPosition = endPosition,
			Displacement = displacement
		};
	}
}
