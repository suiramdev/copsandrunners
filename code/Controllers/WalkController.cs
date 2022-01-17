using copsandrunners.Entities;
using Sandbox;

namespace copsandrunners.Controllers;

public class WalkController : Sandbox.WalkController
{
	public override float GetWishSpeed()
	{
		if ( ((Player)Pawn).IsFrozen )
			return 0;
		return base.GetWishSpeed();
	}

	public override void Move()
	{
		var move = new MoveHelper( Position, Velocity );
		move.Trace = move.Trace.Size( mins, maxs )
			.Ignore( Pawn );
		if (!((Player)Pawn).IsArrested)
			move.Trace = move.Trace.WithoutTags( "Jail" );
		move.MaxStandableAngle = GroundAngle;
		move.TryMove( Time.Delta );
		
		Position = move.Position;
		Velocity = move.Velocity;
	}
	
	public override void StepMove()
	{
		var move = new MoveHelper( Position, Velocity );
		move.Trace = move.Trace.Size( mins, maxs )
			.Ignore( Pawn );
		if ( !((Player)Pawn).IsArrested )
			move.Trace = move.Trace.WithoutTags( "Jail" );
		move.MaxStandableAngle = GroundAngle;
		move.TryMoveWithStep( Time.Delta, StepSize );
		
		Position = move.Position;
		Velocity = move.Velocity;
	}

	public override TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0 )
	{
		if ( liftFeet > 0 )
		{
			start += Vector3.Up * liftFeet;
			maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		var trace = Trace.Ray( start + TraceOffset, end + TraceOffset )
			.Size( mins, maxs )
			.HitLayer( CollisionLayer.All, false )
			.HitLayer( CollisionLayer.Solid, true )
			.HitLayer( CollisionLayer.GRATE, true )
			.HitLayer( CollisionLayer.PLAYER_CLIP, true )
			.Ignore( Pawn );
		if ( !((Player)Pawn).IsArrested )
			trace = trace.WithoutTags( "Jail" );
		var traceResults = trace.Run();

		traceResults.EndPos -= TraceOffset;
		return traceResults;
	}
}
