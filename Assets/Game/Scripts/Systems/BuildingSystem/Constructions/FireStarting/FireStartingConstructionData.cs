using UnityEngine;

namespace Game.Systems.BuildingSystem
{
    [CreateAssetMenu(fileName = "ConstructionData", menuName = "Game/Constructions/FireStarting")]
    public class FireStartingConstructionData : ConstructionData
    {
        [Min(0.5f)]
        public float minIgnitionTime = 1f;
        [Min(0.5f)]
        public float maxIgnitionTime = 5f;

        [Range(0, 80)]
        public float maxTemperature = 80;

        public TimeSystem.Time maxFireTime = new TimeSystem.Time() { Hours = 12 };

        [Space]
        public bool isWindProtection = false;
    }
}