using Sandbox;

namespace copsandrunners.Camera;

public class FirstPersonCamera : CameraMode
{
	private Vector3 _lastPos;
	private readonly float _invertX = Rand.Int( -1, 1 );
	private readonly float _invertY = Rand.FromArray( new[] { -1, 1 } );
	
	private readonly TimeSince _lifeTime = 0;

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
		if ( eyePos.Distance( _lastPos ) < 300 ) // TODO: Tweak this, or add a way to invalidate lastpos when teleporting
		{
			Position = Vector3.Lerp( eyePos.WithZ( _lastPos.z ), eyePos, 20.0f * Time.Delta );
		}
		else
		{
			Position = eyePos;
		}

		Rotation = pawn.EyeRotation;

		Viewer = pawn;
		_lastPos = Position;
	}
	
	public bool Shake(FGDCurve curve, float power)
	{
		var delta = ((float)_lifeTime).LerpInverse( 0, curve.Maxs.x );
		var y = curve.Get( delta );
		
		Rotation *= Rotation.FromAxis( Vector3.Up, power * y * _invertX);
		Rotation *= Rotation.FromAxis( Vector3.Right, power * y * _invertY);

		return _lifeTime < curve.Maxs.x;
	}
}
