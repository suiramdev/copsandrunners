using System;
using Sandbox;

namespace copsandrunners.Utils;

public static class ProjectileMotion
{

	private const float Gravity = 9.8f;
	private static float Meow => Map.Physics.AirDensity; // μ sounds like meow

	public struct ThrowData
	{
		public Vector3 Velocity;
		public float UpTime;
		public float TotalTime;
		public float Height;
		public Vector3 EndPosition;
	}

	/// <summary>
	/// Predict the trajectory of a projectile.
	/// </summary>
	/// <param name="force">Projectile's initial velocity</param>
	/// <param name="pitchAngle">Throw pitch angle</param>
	/// <param name="yawAngle">Throw yaw angle</param>
	/// <param name="mass">Projectile's mass</param>
	/// <returns>Predicted trajectory data</returns>
	public static ThrowData ThrowFromAngle( float force, float pitchAngle, float yawAngle, float mass )
	{
		var velocity = new Vector3(
			force * MathF.Cos( pitchAngle.DegreeToRadian() ) * MathF.Cos( yawAngle.DegreeToRadian() ),
			force * MathF.Cos( pitchAngle.DegreeToRadian() ) * MathF.Sin( yawAngle.DegreeToRadian() ),
			force * MathF.Sin( pitchAngle.DegreeToRadian() )
		);
		var acceleration = new Vector3(
			MathF.Pow( velocity.x, 2 ) * Meow / mass,
			MathF.Pow( velocity.y, 2 ) * Meow / mass,
			MathF.Pow( velocity.z, 2 ) * Meow / mass
		);
		var upTime = 2 * velocity.z / (acceleration.z + Gravity) / 2; // time to reach max height
		var height = velocity.z * upTime - 0.5f * (acceleration.z + Gravity) * MathF.Pow( upTime, 2 ) ; // max height
		var time = upTime + MathF.Sqrt( height / (Gravity / 2) ); // Time to reach up + time to fall
		
		var endPosition = new Vector3(
			velocity.x - 0.5f * acceleration.x * MathF.Pow( upTime, 2 ),
			velocity.y - 0.5f * acceleration.y * MathF.Pow( upTime, 2 ),
			0
		);

		return new ThrowData
		{
			Velocity = velocity,
			UpTime = upTime,
			TotalTime = time,
			Height = height,
			EndPosition = endPosition
		};
	}
}
