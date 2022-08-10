using Sandbox;

namespace copsandrunners;

public partial class Player
{
	[Net] private bool _isArrested { get; set; }
	[Net] public bool IsArrested => (Team != Teams.Cops) && _isArrested;
	[Net] public bool IsFrozen { get; set; }
	
	public void Arrest( bool isArrested )
	{
		_isArrested = isArrested;

		if ( isArrested && IsServer )
			Position = Game.Instance.Jail.Position;
		
		Event.Run( PlayerEvent.Arrested );
	}

	[Event("knock")]
	public async void Knock()
	{
		if ( IsFrozen ) return;
		
		IsFrozen = true;
		await Task.DelaySeconds( 3 );
		IsFrozen = false;
		
		Event.Run( PlayerEvent.Knocked );
	}
}
