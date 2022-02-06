using UnityEngine;

namespace Game.Systems.WeatherSystem
{
    [CreateAssetMenu(menuName = "Game/Environment/FogPresset", fileName = "FogPresset")]
    public class FogPresset : ScriptableObject
    {
        public FogType fogType;

        public Color fogColor;
        [Range(0, 1f)]
        public float fogRange;
        [Range(0, 1f)]
        public float normalRange;
    }
}