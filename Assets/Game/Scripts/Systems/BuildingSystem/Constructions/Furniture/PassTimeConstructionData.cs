using UnityEngine;

namespace Game.Systems.BuildingSystem
{
	[CreateAssetMenu(fileName = "PassTimeConstructionData", menuName = "Game/Constructions/PassTime")]
	public class PassTimeConstructionData : ConstructionData
	{
		public int warmthBonus;
	}
}