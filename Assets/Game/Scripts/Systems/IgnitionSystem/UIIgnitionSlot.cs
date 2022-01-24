using Game.Systems.InventorySystem;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.IgnitionSystem
{
    public class UIIgnitionSlot : MonoBehaviour
    {
        public bool IsEmpty => items?.Count == 0;
        public Item CurrentItem => currentItem;

        [SerializeField] private Image empty;
        [SerializeField] private Image icon;
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

            UpdateIcon();
            UpdateInfo();
            UpdateButtons();
        }

        private void UpdateIcon()
		{
            empty.enabled = IsEmpty;
            icon.enabled = !IsEmpty;
        }
        private void UpdateInfo()
		{
            if (!IsEmpty)
            {
                currentItem = items[currentIndex];

                var texts = currentItem.ItemData.GetLocalization(localization.CurrentLanguage);

                icon.sprite = currentItem.ItemData.itemSprite;
                itemName.text = texts.itemName;
                itemStackSize.text = currentItem.CurrentStackSize + " of " + currentItem.MaximumStackSize;
            }
        }

        private void UpdateButtons()
		{
            buttonLeft.gameObject.SetActive(!IsEmpty && currentIndex > 0);
            buttonRight.gameObject.SetActive(!IsEmpty && currentIndex < items.Count);
        }

        private void OnButtonLeftClicked()
		{
            currentIndex--;
            UpdateButtons();
            UpdateInfo();
        }
        private void OnButtonRightClicked()
		{
            currentIndex++;
            UpdateButtons();
            UpdateInfo();
        }
    }
}