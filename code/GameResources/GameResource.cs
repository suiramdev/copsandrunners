using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sandbox;

namespace copsandrunners;

public abstract class GameResource : Sandbox.GameResource
{
	private static readonly Dictionary<string, GameResource> Collection = new( StringComparer.OrdinalIgnoreCase );

	public string Label { get; set; }

	[HideInEditor]
	[JsonIgnore]
	public string Title { get; set; }
	
	public static T GetInfo<T>( string resourceName ) where T : GameResource
	{
		if ( string.IsNullOrEmpty( resourceName ) || !Collection.ContainsKey( resourceName ) )
			return null;

		return Collection[resourceName] as T;
	}

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( TypeLibrary is null )
			return;

		if ( string.IsNullOrEmpty( ResourceName ) )
			return;

		if ( Collection.ContainsKey( ResourceName ) )
		{
			Log.Error( $"There is already a resource tied to {ResourceName}" );
			return;
		}

		var typeDescription = TypeLibrary.GetDescription<object>( ResourceName );
		if ( typeDescription is null )
			return;

		Title = typeDescription.Title;
		Collection[ResourceName] = this;

		if ( !string.IsNullOrEmpty( Title ) )
			Collection[Title] = this;
	}
}
