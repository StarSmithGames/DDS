using UnityEngine;

namespace Game.Systems.LocalizationSystem
{
	public class LocalizationData : ScriptableObject
	{
		public string Id;
		public string[] Keys;
		public string[] Values;
	}
}