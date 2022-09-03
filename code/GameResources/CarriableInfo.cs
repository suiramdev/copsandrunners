using System.Text.Json.Serialization;
using Sandbox;

namespace copsandrunners.GameResources;

public enum HoldType
{
	None,
	Pistol,
	Rifle,
	Shotgun,
	Carry,
	Fists
}

[GameResource( "Carriable", "carry", "Carriable template", Icon = "inventory_2" )]
public class CarriableInfo : GameResource
{
	[Category( "General" )] 
	public bool Spawnable { get; set; }
	
	[Category( "General" )]
	public bool CanDrop { get; set; }
	
	[Title( "View Model" ), Category( "Appearance" ), ResourceType( "vmdl" )]
	public string ViewModelPath { get; set; }

	[Title( "Hands Model" ), Category( "Appearance" ), ResourceType( "vmdl" )]
	public string HandsModelPath { get; set; }
	
	[Title( "World Model" ), Category( "Appearance" ), ResourceType( "vmdl" )]
	public string WorldModelPath { get; set; }
	
	[Category( "Appearance" )]
	public HoldType HoldType { get; set; } = HoldType.None;
	
	[HideInEditor]
	[JsonIgnore]
	public Model HandsModel { get; private set; }

	[HideInEditor]
	[JsonIgnore]
	public Model ViewModel { get; private set; }

	[HideInEditor]
	[JsonIgnore]
	public Model WorldModel { get; private set; }
	
	protected override void PostLoad()
	{
		base.PostLoad();

		HandsModel = Model.Load( HandsModelPath );
		ViewModel = Model.Load( ViewModelPath );
		WorldModel = Model.Load( WorldModelPath );
	}
}
