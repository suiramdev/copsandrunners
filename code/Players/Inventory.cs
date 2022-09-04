using System.Collections;
using System.Collections.Generic;
using System.Linq;
using copsandrunners.Items;
using Sandbox;

namespace copsandrunners.Players;

public partial class Inventory : BaseNetworkable
{
	[Net]
	[HideInEditor]
	public Player Owner { get; }

	[HideInEditor]
	public Carriable Active
	{
		get => Owner.ActiveCarriable;
		set => Owner.ActiveCarriable = value;
	}
	
	[HideInEditor]
	public int ActiveSlot
	{
		get => Items.IndexOf( Active );
	}

	[Net]
	public IList<Carriable> Items { get; set; }
	public readonly int Capacity = 10;

	public Inventory( Player owner )
	{
		Owner = owner;
		
		Event.Register( this );
	}
	
	~Inventory()
	{
		Event.Unregister( this );
	}

	public bool Add( Carriable carriable, bool makeActive = false )
	{
		Host.AssertServer();

		if ( !carriable.IsValid() )
			return false;

		if ( Items.Contains( carriable ) )
			return false;

		if ( Items.Count >= Capacity )
			return false;
		
		Items.Add( carriable );
		carriable.SetParent( Owner, true );

		if ( makeActive )
			SetActive( Items.Count - 1 );
		
		Event.Run( InventoryEvent.ItemAdded );

		return true;
	}

	public bool Remove( Carriable carriable )
	{
		Host.AssertServer();
		
		if ( !Items.Contains( carriable ) )
			return false;
		
		Items.Remove( carriable );

		Event.Run( InventoryEvent.ItemRemoved );
		
		return true;
	}

	public bool Remove( int slot ) => Items.ElementAtOrDefault( slot ) != default && Remove( Items[slot] );

	public void Clear()
	{
		Host.AssertServer();
		
		foreach ( var carriable in Items )
			carriable.Delete();

		Active = null;

		Items.Clear();
	}
	
	public bool SetActive( Carriable carriable )
	{
		if ( !Items.Contains( carriable ) )
			return false;

		Active = carriable;

		Event.Run( InventoryEvent.SetActive );
		
		return true;
	}

	public bool SetActive( int slot ) => Items.ElementAtOrDefault( slot ) != default && SetActive( Items[slot] );
}
