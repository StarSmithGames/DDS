using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.Systems.BuildingSystem
{
	[CreateAssetMenu(fileName = "ConstructionBlueprint", menuName = "Game/Constructions/ConstructionBlueprint")]
	public class ConstructionBlueprint : ScriptableObject
	{
		[PreviewField]
		public Sprite icon;
		[AssetList]
		public ConstructionModel model;
		public bool isInteractAfterBuilding = true;
	}
}