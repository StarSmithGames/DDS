using Game.Systems.InventorySystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.HarvestingSystem
{
    public class UIYieldItem : PoolableObject
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMPro.TextMeshProUGUI itemName;
        [SerializeField] private TMPro.TextMeshProUGUI itemCount;

        public void SetItem(Item item)
		{
            itemIcon.sprite = item.ItemData.itemSprite;
            itemName.text = item.ItemData.ItemName;
            itemCount.enabled = item.IsStackable;
            itemCount.text = item.IsStackable ? $"x{item.CurrentStackSize}" : "";
        }

        public class Factory : PlaceholderFactory<UIYieldItem> { }
    }
}