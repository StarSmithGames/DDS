using System.Collections.Generic;

using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ItemsManager", menuName = "Installers/ItemsManager")]
public class ItemsManager : ScriptableObjectInstaller
{
	private const string Assets = "Game/Resources/Assets/";

	[Title("Items")]
	[AssetList(AutoPopulate = true, Path = Assets)]
	[ReadOnly] [SerializeField] private List<ItemData> allItems = new List<ItemData>();

	//[TitleGroup("Consumable Items")]
	//[AssetList(AutoPopulate = true, Path = Assets, CustomFilterMethod = "Limits")]
	//[ReadOnly] [SerializeField] private List<ConsumableItemSD> allConsumables = new List<ConsumableItemSD>();
	//[HorizontalGroup("Consumable Items/Split")]
	//[VerticalGroup("Consumable Items/Split/Left")]
	//[AssetList(AutoPopulate = true, Path = Assets, CustomFilterMethod = "Limits")]
	//[ReadOnly] [SerializeField] private List<PotionItemSD> allDrinks = new List<PotionItemSD>();

	//[LabelWidth(100)]
	//[VerticalGroup("Consumable Items/Split/Left")]
	//[ReadOnly] [SerializeField] private WaterItemSD potableWater;
	//public WaterItemSD PotableWater => potableWater;

	//[LabelWidth(100)]
	//[VerticalGroup("Consumable Items/Split/Left")]
	//[ReadOnly] [SerializeField] private WaterItemSD unsafeWater;
	//public WaterItemSD UnSafeWater => unsafeWater;

	//[VerticalGroup("Consumable Items/Split/Right")]
	//[AssetList(AutoPopulate = true, Path = Assets, CustomFilterMethod = "Limits")]
	//[ReadOnly] [SerializeField] private List<FoodItemSD> allFood = new List<FoodItemSD>();
	//[LabelWidth(100)]
	//[VerticalGroup("Consumable Items/Split/Right")]
	//[ReadOnly] [SerializeField] private SnowItemSD snow;
	//[LabelWidth(100)]
	//[VerticalGroup("Consumable Items/Split/Right")]
	//[AssetList(AutoPopulate = true, Path = Assets)]
	//[ReadOnly] [SerializeField] private List<MeatItemSD> allMeat = new List<MeatItemSD>();


	[TitleGroup("Fire Items")]
	[AssetList(AutoPopulate = true, Path = Assets)]
	[ReadOnly] [SerializeField] private List<FireItemData> allFireItems = new List<FireItemData>();

	[HorizontalGroup("Fire Items/Split")]
	[VerticalGroup("Fire Items/Split/Left")]
	[AssetList(AutoPopulate = true, Path = Assets)]
	[ReadOnly] [SerializeField] private List<FireStarterData> allStarterItems = new List<FireStarterData>();
	[VerticalGroup("Fire Items/Split/Left")]
	[AssetList(AutoPopulate = true, Path = Assets)]
	[ReadOnly] [SerializeField] private List<FireFuelData> allFuelItems = new List<FireFuelData>();

	[VerticalGroup("Fire Items/Split/Right")]
	[AssetList(AutoPopulate = true, Path = Assets)]
	[ReadOnly] [SerializeField] private List<FireTinderData> allTinderItems = new List<FireTinderData>();
	[VerticalGroup("Fire Items/Split/Right")]
	[AssetList(AutoPopulate = true, Path = Assets)]
	[ReadOnly] [SerializeField] private List<FireAccelerantData> allAccelerantItems = new List<FireAccelerantData>();

	//[TitleGroup("Tools Items")]
	//[AssetList(AutoPopulate = true, Path = Assets)]
	//[ReadOnly] [SerializeField] private List<ToolItemSD> allTools = new List<ToolItemSD>();

	//[AssetList(AutoPopulate = true, Path = Assets)]
	//[HorizontalGroup("Tools Items/Split")]
	//[VerticalGroup("Tools Items/Split/Left")]
	//[ReadOnly] [SerializeField] private List<ToolWeaponSD> allWeapons = new List<ToolWeaponSD>();

	//[AssetList(AutoPopulate = true, Path = Assets)]
	//[VerticalGroup("Tools Items/Split/Right")]
	//[ReadOnly] [SerializeField] private List<ItemAmmunitionSD> allAmmunitions = new List<ItemAmmunitionSD>();

	////Materials Items
	//[TitleGroup("Materials Items")]
	//[AssetList(AutoPopulate = true, Path = Assets)]
	//[ReadOnly] [SerializeField] private List<MaterialItemSD> allMaterials = new List<MaterialItemSD>();

	////Blueprints

	//[TitleGroup("Blueprints")]
	//[AssetList(AutoPopulate = true, Path = Assets)]
	//[ReadOnly] [SerializeField] private List<BlueprintSD> allBlueprints = new List<BlueprintSD>();

	private bool Limits(ItemData item)
	{
		return true;
	}

	public override void InstallBindings()
	{

	}
}