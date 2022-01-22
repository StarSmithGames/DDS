using Game.Systems.BuildingSystem;

using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;
using Zenject;

namespace Game.Systems.RadialMenu
{
	[CreateAssetMenu(fileName = "RadialMenuInstaller", menuName = "Installers/RadialMenuInstaller")]
    public class RadialMenuInstaller : ScriptableObjectInstaller<RadialMenuInstaller>
    {
		[SerializeField] private RadialMenuSettings radialMenuSettings;

		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalRadialMenuButton>();
			Container.DeclareSignal<SignalRadialMenuOptionChanged>();

			Container.BindFactory<UIRadialMenuOption, UIRadialMenuOption.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(12)
					.FromComponentInNewPrefab(radialMenuSettings.optionPrefab))
					.WhenInjectedInto<RadialMenuHandler>();

			Container.BindInstance(radialMenuSettings)
					.WhenInjectedInto<RadialMenuHandler>();

			Container.BindInterfacesAndSelfTo<RadialMenuHandler>().AsSingle();
		}
	}

	[System.Serializable]
	public class RadialMenuSettings
	{
		public float maxRadius = 200f;
		public bool isSnap = true;

		public bool useTilt = true;
		[ShowIf("useTilt")]
		public float tiltAmount = 15;

		[Space]
		public float animationIn = 0.25f;
		public float animationOut = 0.25f;

		[Space]
		public UIRadialMenuOption optionPrefab;

		[Space]
		public List<RadialMenuOptionData> primaryOptions = new List<RadialMenuOptionData>();

		//public bool useSeparators = true;
		//[ShowIf("useSeparators")]
		//public GameObject separatorPrefab;
	}
	[System.Serializable]
	public class RadialMenuOptionData
	{
		public RadialMenuOptionType optionType = RadialMenuOptionType.None;

		[HideIf("optionType", RadialMenuOptionType.None)]
		[PreviewField]
		public Sprite optionIcon;

		[ShowIf("optionType", RadialMenuOptionType.CampCrafting)]
		public List<ConstructionBlueprint> campBlueprints = new List<ConstructionBlueprint>();
	}

	public enum RadialMenuOptionType
	{
		None				= 0, 
		Weapons				= 1,
		CampCrafting		= 2,
		Crafting			= 3,
		Journal				= 4,
		LightSources		= 5,
		EmergencyStims		= 6,
		Food				= 7,
		Drink				= 8,
		OpenCharacterStatus	= 9,
		OpenNavigation		= 10,
	}
}