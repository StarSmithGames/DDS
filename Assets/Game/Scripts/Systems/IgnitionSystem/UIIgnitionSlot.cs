using Game.Systems.InventorySystem;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.IgnitionSystem
{
    public class UIIgnitionSlot : MonoBehaviour
    {
        public bool IsEmpty => CurrentItem == null;
        public Item CurrentItem => currentItem;

        [SerializeField] private Image empty;
        [SerializeField] private Image icon;
        [SerializeField] private TMPro.TextMeshProUGUI itemType;
        [SerializeField] private TMPro.TextMeshProUGUI itemName;
        [SerializeField] private TMPro.TextMeshProUGUI itemStackSize;
        [SerializeField] private Button buttonLeft;
        [SerializeField] private Button buttonRight;

        private List<Item> items;
        private int currentIndex;
        private Item currentItem;

        private SignalBus signalBus;
        private LocalizationSystem.LocalizationSystem localization;

        [Inject]
        private void Construct(SignalBus signalBus, LocalizationSystem.LocalizationSystem localization)
		{
            this.signalBus = signalBus;
            this.localization = localization;

            buttonLeft.onClick.AddListener(OnButtonLeftClicked);
            buttonRight.onClick.AddListener(OnButtonRightClicked);
        }

		private void OnDestroy()
		{
            buttonLeft.onClick.RemoveAllListeners();
            buttonRight.onClick.RemoveAllListeners();
        }

		public void SetItems(List<Item> items)
		{
            this.items = items;
            currentIndex = 0;
            currentItem = null;

            UpdateItemInfo();
            UpdateButtons();
        }

        public void Block()
		{
            buttonLeft.enabled = false;
            buttonRight.enabled = false;
        }

        public void UnBlock()
		{
            buttonLeft.enabled = true;
            buttonRight.enabled = true;
        }

        private void UpdateItemInfo()
		{
            currentItem = items.Count > 0 ? items[currentIndex] : null;

            empty.enabled = IsEmpty;
            icon.enabled = !IsEmpty;

            itemName.enabled = itemStackSize.enabled = itemType.enabled = !IsEmpty;

            if (!IsEmpty)
            {
                var texts = currentItem.ItemData.GetLocalization(localization.CurrentLanguage);

                icon.sprite = currentItem.ItemData.itemSprite;
                itemName.text = texts.itemName;
                itemStackSize.text = currentItem.ItemData.isStackable && itemStackSize.enabled ? currentItem.CurrentStackSize + " of " + currentItem.MaximumStackSize : "";

                signalBus?.Fire(new SignalUIIgnitionSlotItemChanged());
            }
        }

        private void UpdateButtons()
		{
            buttonLeft.gameObject.SetActive(!IsEmpty && currentIndex > 0);
            buttonRight.gameObject.SetActive(!IsEmpty && currentIndex < items.Count - 1);
        }

        private void OnButtonLeftClicked()
		{
            currentIndex--;
            UpdateButtons();
            UpdateItemInfo();
        }
        private void OnButtonRightClicked()
		{
            currentIndex++;
            UpdateButtons();
            UpdateItemInfo();
        }
    }
}