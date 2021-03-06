using Sandbox;

namespace copsandrunners.Camera.Modifiers;

public class MeleeShake : CameraModifier
{
	private readonly FGDCurve _curve;
	private readonly float _power;
	private readonly float _invertX = Rand.Int( -1, 1 );
	private readonly float _invertY = Rand.FromArray( new[] { -1, 1 } );
	
	private readonly TimeSince _lifeTime = 0;

	public MeleeShake( FGDCurve curve, float power)
	{
		_curve = curve;
		_power = power;
	}

	public override bool Update( ref CameraSetup setup )
	{
		var delta = ((float)_lifeTime).LerpInverse( 0, _curve.Maxs.x );
		var y = _curve.Get( delta );
		
		setup.Rotation *= Rotation.FromAxis( Vector3.Up, _power * y * _invertX);
		setup.Rotation *= Rotation.FromAxis( Vector3.Right, _power * y * _invertY);

		return _lifeTime < _curve.Maxs.x;
	}
}
