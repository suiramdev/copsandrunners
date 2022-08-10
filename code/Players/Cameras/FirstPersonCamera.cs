using System.Threading.Tasks;
using Sandbox;

namespace copsandrunners.Cameras;

public class FirstPersonCamera : CameraMode
{
	private Vector3 _lastPos;

	public override void Activated()
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		Position = pawn.EyePosition;
		Rotation = pawn.EyeRotation;

		_lastPos = Position;
	}

	public override void Update()
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		var eyePos = pawn.EyePosition;
		Position = eyePos.Distance( _lastPos ) < 300 ? Vector3.Lerp( eyePos.WithZ( _lastPos.z ), eyePos, 20.0f * Time.Delta ) : eyePos;

		Rotation = pawn.EyeRotation;

		Viewer = pawn;
		_lastPos = Position;
	}

	#region Shake effect
	private readonly float _invertX = Rand.Int( -1, 1 );
	private readonly float _invertY = Rand.FromArray( new[] { -1, 1 } );

	private TimeSince _sinceShake = 0;
	public Task Shake(FGDCurve curve, float power) // Should it really be a Task ?
	{
		_sinceShake = 0;
		while ( true )
		{
			if ( _sinceShake >= curve.Maxs.x )
				break;

			var delta = ((float)_sinceShake).LerpInverse( 0, curve.Maxs.x );
			var y = curve.Get( delta );
		
			Rotation *= Rotation.FromAxis( Vector3.Up, power * y * _invertX);
			Rotation *= Rotation.FromAxis( Vector3.Right, power * y * _invertY);	
		}
		
		return Task.CompletedTask;
	}
	#endregion
}
