using System;
using System.Collections.Generic;
using System.Linq;

namespace copsandrunners.Assets;

public partial class Asset : Sandbox.Asset
{
	public static IReadOnlyList<Asset> Assets => _assets;
	internal static List<Asset> _assets = new();

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( !_assets.Contains( this ) )
			_assets.Add( this );
	}

	public static T FromPath<T>(string path)
	{
		return (T) Convert.ChangeType(_assets.First( asset => asset.Path == path ), typeof(T));
	}
}
