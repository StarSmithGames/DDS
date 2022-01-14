using Sirenix.OdinInspector;

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;

public class Inventory : IInventory
{
    public event UnityAction OnInventoryChanged;

    public List<Item> Items { get; private set; }

    public Inventory(InventorySettings settings)
	{
        Items = settings.GenerateItems();
    }

	public bool Add(Item item)
	{
        Items.Add(item);
        OnInventoryChanged?.Invoke();
        return true;
	}
    public bool Remove(Item item)
	{
        Items.Remove(item);
        OnInventoryChanged?.Invoke();
        return true;
	}
}

[System.Serializable]
public class InventorySettings
{
    public bool useRandomItems = true;
    public bool shuffleList = true;
    //sort by

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

		if (shuffleList)
		{
            result = result.OrderBy(x => Guid.NewGuid()).ToList();
        }

        return result;
	}
}

[System.Serializable]
public class RandomInventorySettings
{
    [Tooltip("Items не повтор€ютс€.")]
    public bool isUnique = false;
    public bool useRandomCount = true;

    [HideIf("useRandomCount")]
    [MinValue(1), MaxValue(10)]
    public int itemsCount = 1;

    [ShowIf("useRandomCount")]
    [MinMaxSlider(1, 10)]
    public Vector2Int itemsMinMaxCount = new Vector2Int(1, 5);

    [Tooltip("Items которые точно будут включены рандомайзером.(Ѕудут добавлены к текущим.)")]
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
    [Tooltip("“ипы данных которые будут игнорироватьс€ рандомайзером.")]
    public List<ItemData> ignoreList = new List<ItemData>();
}