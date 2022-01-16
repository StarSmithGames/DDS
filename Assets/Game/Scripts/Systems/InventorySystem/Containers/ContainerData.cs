using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "ContainerData", menuName = "ContainerData")]
public class ContainerData : ScriptableObject
{
    [ListDrawerSettings(ListElementLabelName = "Tittle")]
    [InfoBox("@LocalizationInfo", InfoMessageType.Warning)]
    public List<Localization> localizations = new List<Localization>();

    public InteractionSettings interact;
    public InteractionSettings inspect;
    [Space]
    public InventorySettings inventory;

    public Localization GetLocalization(SystemLanguage language)
    {
        return localizations.Find((x) => x.language == language) ?? localizations[0];
    }

    private string LocalizationInfo => "Required :\n" + SystemLanguage.English.ToString();
    [System.Serializable]
    public class Localization
	{
        public SystemLanguage language = SystemLanguage.English;

        public string containerName;

        private string Tittle => language.ToString() + " " + (!(string.IsNullOrEmpty(containerName) || string.IsNullOrWhiteSpace(containerName)));
    }
}