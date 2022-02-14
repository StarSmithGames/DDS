using UnityEngine;

namespace Game.Systems.WeatherSystem
{
    [CreateAssetMenu(menuName = "Game/Environment/WeatherPresset", fileName = "WeatherPresset")]
    public class WeatherPresset : ScriptableObject
    {
        public FogPresset fog;
        public CloudsPresset clouds;
    }
}