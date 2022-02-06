using UnityEngine;

namespace Game.Systems.BuildingSystem
{
    [CreateAssetMenu(fileName = "ConstructionData", menuName = "Game/Constructions/FireStarting")]
    public class FireStartingConstructionData : ConstructionData
    {
        [Range(0, 80)]
        public float maxTemperature = 80;

        public TimeSystem.Time maxFireTime = new TimeSystem.Time() { Hours = 12 };

        [Space]
        public bool isWindProtection = false;
    }
}