using Sandbox;

/* Taken directly from fp.sandbox */
public class ViewModel : BaseViewModel
{
	private static float SwingInfluence => 0.05f;
	private static float ReturnSpeed => 5.0f;
	private static float MaxOffsetLength => 10.0f;
	private static float BobCycleTime => 7;
	private static Vector3 BobDirection => new Vector3( 0.0f, 1.0f, 0.5f );

	private Vector3 _swingOffset;
	private float _lastPitch;
	private float _lastYaw;
	private float _bobAnim;

	private bool _activated = false;

	private const bool EnableSwingAndBob = true;

	private float YawInertia { get; set; }
	private float PitchInertia { get; set; }

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );

		if ( !Local.Pawn.IsValid() )
			return;

		if ( !_activated )
		{
			_lastPitch = camSetup.Rotation.Pitch();
			_lastYaw = camSetup.Rotation.Yaw();

			YawInertia = 0;
			PitchInertia = 0;

			_activated = true;
		}

		Position = camSetup.Position;
		Rotation = camSetup.Rotation;

		var cameraBoneIndex = GetBoneIndex( "camera" );
		if ( cameraBoneIndex != -1 )
		{
			camSetup.Rotation *= (Rotation.Inverse * GetBoneTransform( cameraBoneIndex ).Rotation);
		}

		var newPitch = Rotation.Pitch();
		var newYaw = Rotation.Yaw();

		PitchInertia = Angles.NormalizeAngle( newPitch - _lastPitch );
		YawInertia = Angles.NormalizeAngle( _lastYaw - newYaw );

		if ( EnableSwingAndBob )
		{
			var playerVelocity = Local.Pawn.Velocity;

			if ( Local.Pawn is Player player )
			{
				var controller = player.GetActiveController();
				if ( controller != null && controller.HasTag( "noclip" ) )
				{
					playerVelocity = Vector3.Zero;
				}
			}

			var verticalDelta = playerVelocity.z * Time.Delta;
			var viewDown = Rotation.FromPitch( newPitch ).Up * -1.0f;
			verticalDelta *= (1.0f - System.MathF.Abs( viewDown.Cross( Vector3.Down ).y ));
			var pitchDelta = PitchInertia - verticalDelta * 1;
			var yawDelta = YawInertia;

			var offset = CalcSwingOffset( pitchDelta, yawDelta );
			offset += CalcBobbingOffset( playerVelocity );

			Position += Rotation * offset;
		}
		else
		{
			SetAnimParameter( "aim_yaw_inertia", YawInertia );
			SetAnimParameter( "aim_pitch_inertia", PitchInertia );
		}

		_lastPitch = newPitch;
		_lastYaw = newYaw;
	}

	private Vector3 CalcSwingOffset( float pitchDelta, float yawDelta )
	{
		Vector3 swingVelocity = new Vector3( 0, yawDelta, pitchDelta );

		_swingOffset -= _swingOffset * ReturnSpeed * Time.Delta;
		_swingOffset += (swingVelocity * SwingInfluence);

		if ( _swingOffset.Length > MaxOffsetLength )
		{
			_swingOffset = _swingOffset.Normal * MaxOffsetLength;
		}

		return _swingOffset;
	}

	private Vector3 CalcBobbingOffset( Vector3 velocity )
	{
		_bobAnim += Time.Delta * BobCycleTime;

		var twoPI = System.MathF.PI * 2.0f;

		if ( _bobAnim > twoPI )
		{
			_bobAnim -= twoPI;
		}

		var speed = new Vector2( velocity.x, velocity.y ).Length;
		speed = speed > 10.0 ? speed : 0.0f;
		var offset = BobDirection * (speed * 0.005f) * System.MathF.Cos( _bobAnim );
		offset = offset.WithZ( -System.MathF.Abs( offset.z ) );

		return offset;
	}
}
