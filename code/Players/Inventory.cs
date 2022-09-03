using System.Collections;
using System.Collections.Generic;
using copsandrunners.Items;
using Sandbox;

namespace copsandrunners.Players;

public class Inventory : IEnumerable<Carriable>
{
	public Player Owner { get; private set; }

	public Carriable Active { get; set; }

	public int ActiveSlot
	{
		get => _list.IndexOf( Active );
	}

	private readonly List<Carriable> _list = new(10);
	public int Count => _list.Count;
	public int Capacity => _list.Capacity;
	public Carriable this[ int i ] => _list[i];

	public Inventory( Player owner )
	{
		Owner = owner;
	}

	public bool Add( Carriable carriable, bool makeActive = false )
	{
		Host.AssertServer();

		if ( !carriable.IsValid() )
			return false;

		if ( _list.Contains( carriable ) )
			return false;

		if ( Count >= Capacity )
			return false;
		
		_list.Add( carriable );
		carriable.SetParent( Owner, true );

		if ( makeActive )
			SetActive( Count - 1 );
		
		Event.Run( InventoryEvent.ItemAdded );

		return true;
	}

	public bool Remove( Carriable carriable )
	{
		Host.AssertServer();
		
		if ( !_list.Contains( carriable ) )
			return false;
		
		_list.Remove( carriable );

		Event.Run( InventoryEvent.ItemRemoved );
		
		return true;
	}

	public bool Remove( int slot ) => Remove( _list[slot] );

	public void Clear()
	{
		Host.AssertServer();
		
		Log.Info( "Clear INVENTORY SHEESH" );
		
		foreach ( var carriable in _list )
			carriable.Delete();

		Active = null;

		_list.Clear();
	}
	
	public bool SetActive( Carriable carriable )
	{
		if ( !_list.Contains( carriable ) )
			return false;

		Active = carriable;

		Event.Run( InventoryEvent.SetActive );
		
		return true;
	}

	public bool SetActive( int slot )
	{
		if ( _list[slot] is null )
			return false;
		
		return SetActive( _list[slot] );
	}

	public IEnumerator<Carriable> GetEnumerator() => _list.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
