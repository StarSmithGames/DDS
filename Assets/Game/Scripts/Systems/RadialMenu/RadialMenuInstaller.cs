using Game.Systems.BuildingSystem;
using Game.Systems.InventorySystem;

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

			Container.BindFactory<UIRadialMenuOption, UIRadialMenuOption.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(12)
					.FromComponentInNewPrefab(radialMenuSettings.optionPrefab))
					.WhenInjectedInto<RadialMenuHandler>();

			Container.BindInstance(radialMenuSettings).WhenInjectedInto<RadialMenuHandler>();

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
		public RadialMenuData primaryMenu;

		//public bool useSeparators = true;
		//[ShowIf("useSeparators")]
		//public GameObject separatorPrefab;
	}

	[System.Serializable]
	public class RadialMenuData 
	{
		public List<RadialMenuOptionData> options = new List<RadialMenuOptionData>();
	}

	[System.Serializable]
	public class RadialMenuOptionData
	{
		public RadialMenuOptionType optionType = RadialMenuOptionType.None;

		[ShowIf("IsHasIcon")]
		[PreviewField]
		public Sprite optionIcon;

		[ShowIf("optionType", RadialMenuOptionType.GoTo)]
		public RadialMenuData radialMenu;

		[ShowIf("IsHasItem")]
		public Item item;

		[ShowIf("IsHasBlueprint")]
		[PreviewField]
		public ConstructionBlueprint blueprint;

		public bool IsEmpty()
		{
			switch (optionType)
			{
				case RadialMenuOptionType.GoTo:
				{
					return radialMenu.options.Count == 0;
				}
				case RadialMenuOptionType.Drink:
				case RadialMenuOptionType.Eat:
				{
					return item == null;
				}

				case RadialMenuOptionType.Build:
				{
					return blueprint == null;
				}
			}

			return optionIcon == null;
		}

		public bool IsCanSetColor()
		{
			return !IsHasItem && !IsHasBlueprint;
		}

		public Sprite GetIcon()
		{
			if (optionType == RadialMenuOptionType.None) return null;
			if (optionType == RadialMenuOptionType.Build) return blueprint.icon;
			if (IsHasItem) return item.ItemData.itemSprite;

			return optionIcon;
		}

		private bool IsHasIcon => optionType != RadialMenuOptionType.None && optionType != RadialMenuOptionType.Build;
		private bool IsHasItem => optionType == RadialMenuOptionType.Drink || optionType == RadialMenuOptionType.Eat;
		private bool IsHasBlueprint => optionType == RadialMenuOptionType.Build;
	}

	public enum RadialMenuOptionType
	{
		None				= 0,

		GoTo				= 1,
		GoToDrink			= 2,
		GoToFood			= 3,

		Drink				= 10,
		Eat					= 11,

		Build			= 20,

		Crafting			= 30,

		OpenPassTime		= 50,

		//Weapons = 3,
		//Journal				= 5,
		//LightSources		= 6,
		//EmergencyStims		= 7,
		//OpenCharacterStatus	= 10,
		//OpenNavigation		= 11,
	}
}