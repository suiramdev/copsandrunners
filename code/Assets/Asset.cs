using System;
using System.Collections.Generic;
using System.Linq;

namespace copsandrunners.Assets;

public class Asset : Sandbox.Asset
{
	public static IReadOnlyList<Asset> Assets => _assets;
	internal static List<Asset> _assets = new();

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( !_assets.Contains( this ) )
			_assets.Add( this );
	}

	// Temporary fix of a s&box's issue from 09/01/2022
	// with Resource.FromPath
	public new static T FromPath<T>(string path)
	{
		return (T) Convert.ChangeType(_assets.First( asset => asset.Path == path ), typeof(T));
	}
}
