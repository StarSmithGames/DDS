using Sirenix.OdinInspector;

using System.Collections.Generic;
using UnityEngine;

public class Inventory : IInventory
{
    private List<Item> items;

    public Inventory(InventorySettings settings)
	{
        items = settings.GenerateItems();
    }
}

[System.Serializable]
public class InventorySettings
{
    public bool useRandomItems = true;

    [HideIf("useRandomItems")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Tittle")]
    public List<Item> items = new List<Item>();

    [ShowIf("useRandomItems")]
    [InlineProperty]
    public RandomInventorySettings randomSettings;

    public List<Item> GenerateItems()
	{
        List<Item> result = new List<Item>();

		if (useRandomItems)
		{

		}
		else
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].useRandom)
				{
                    result.Add(items[i].GenerateItem());
                }
				else
				{
                    result.Add(items[i]);
				}
			}
		}

        return result;
	}
}

[System.Serializable]
public class RandomInventorySettings
{
    public bool useRandomCount = true;

    [HideIf("useRandomCount")]
    [MinValue(1), MaxValue(10)]
    public int itemsCount = 1;

    [ShowIf("useRandomCount")]
    [MinMaxSlider(1, 10)]
    public Vector2Int itemsMinMaxCount = new Vector2Int(1, 5);

    [Tooltip("Item'ы которые точно будут включены рандомайзером.(Будут добавлены к текущим.)")]
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Tittle")]
    public List<Item> staticItems = new List<Item>();

	#region Bools
	[ToggleGroup("useConsumableItems", "Use Consumable Items")]
    public bool useConsumableItems = true;

    [ToggleGroup("useFireItems", "Use Fire Items")]
    public bool useFireItems = true;
    [ToggleGroup("useFireItems")]
    [ToggleLeft]
    public bool useAccelerantItems = true;
    [ToggleGroup("useFireItems")]
    [ToggleLeft]
    public bool useFuelItems = true;
    [ToggleGroup("useFireItems")]
    [ToggleLeft]
    public bool useStarterItems = true;
    [ToggleGroup("useFireItems")]
    [ToggleLeft]
    public bool useTinderItems = true;

    [ToggleGroup("useToolItems", "Use Tool Items")]
    public bool useToolItems = true;

    [ToggleGroup("useMaterialItems", "Use Material Items")]
    public bool useMaterialItems = true;
	#endregion

	[Space]
    [Tooltip("Типы данных которые будут игнорироваться рандомайзером.")]
    public List<ItemData> ignoreList = new List<ItemData>();
}