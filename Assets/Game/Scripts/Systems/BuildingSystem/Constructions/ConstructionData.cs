using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Systems.BuildingSystem
{
    [CreateAssetMenu(fileName = "ConstructionData", menuName = "Game/Constructions/ConstructionData")]
    public class ConstructionData : ScriptableObject
    {
        [ListDrawerSettings(ListElementLabelName = "Tittle")]
        [InfoBox("@LocalizationInfo", InfoMessageType.Warning)]
        public List<Localization> localizations = new List<Localization>();

        public bool useBasicInteraction = true;
        [HideIf("useBasicInteraction")]
        public InteractionSettings interact;

        public Localization GetLocalization(SystemLanguage language)
        {
            Assert.AreNotEqual(localizations.Count, 0, $"{Path.GetFullPath(AssetDatabase.GetAssetPath(GetInstanceID()))} localizations size equal zero.");

            return localizations.Find((x) => x.language == language) ?? localizations[0];
        }

        private string LocalizationInfo => "Required :\n" + SystemLanguage.English.ToString();
        [System.Serializable]
        public class Localization
        {
            public SystemLanguage language = SystemLanguage.English;

            public string constructionName;

            private string Tittle => language.ToString() + " " + (!(string.IsNullOrEmpty(constructionName) || string.IsNullOrWhiteSpace(constructionName)));
        }
    }
}