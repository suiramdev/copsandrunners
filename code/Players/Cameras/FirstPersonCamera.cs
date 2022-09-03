using Sandbox;

namespace copsandrunners.Players.Cameras;

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
}
