using System;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using static UnityEditor.Progress;

namespace Game.Systems.InventorySystem.Transactor
{
    public class UIItemTransactorWindow : WindowBase
    {
        [SerializeField] private TMPro.TextMeshProUGUI itemName;
        [Space]
        [SerializeField] private Button decreaseButton;
        [SerializeField] private TMPro.TextMeshProUGUI itemCount;
        [SerializeField] private Button inscreaseButton;
        [Space]
        [SerializeField] private Button allButton;
        [SerializeField] private Button giveButton;
        [SerializeField] private Button backButton;

        private Item item;
        private int currentCount = 1;

        private SignalBus signalBus;
        private LocalizationSystem.LocalizationSystem localization;

        [Inject]
        private void Construct(SignalBus signalBus, UIManager uiManager, LocalizationSystem.LocalizationSystem localization)
        {
            this.signalBus = signalBus;
            this.localization = localization;

            uiManager.WindowsManager.Register(this);

            inscreaseButton.onClick.AddListener(ButtonIncreaseClicked);
            decreaseButton.onClick.AddListener(ButtonDecreaseClicked);

            allButton.onClick.AddListener(ButtonAllClicked);
            giveButton.onClick.AddListener(ButtonGiveClicked);
            backButton.onClick.AddListener(ButtonBackClicked);
        }
        private void OnDestroy()
        {
            inscreaseButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.RemoveAllListeners();

            allButton.onClick.RemoveAllListeners();
            giveButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }

        public void SetTransaction(Item item)
        {
            this.item = item;

            currentCount = (int)Math.Ceiling((double)item.CurrentStackSize / 2);

            UpdateTransaction();
        }

        private void UpdateTransaction()
        {
            itemName.text = item?.ItemData.ItemName ?? "";
            UpdateTranstitionCount();
        }

        private void UpdateTranstitionCount()
        {
            itemCount.text = currentCount + "/" + item.CurrentStackSize;
        }

        private void ButtonIncreaseClicked()
        {
            if (currentCount < item.CurrentStackSize)
            {
                currentCount++;

                UpdateTranstitionCount();
            }
        }
        private void ButtonDecreaseClicked()
        {
            if (currentCount > item.MinimumStackSize)
            {
                currentCount--;

                UpdateTranstitionCount();
            }
        }

        private void ButtonAllClicked()
        {
            signalBus?.Fire(new SignalUITransactorAll());
        }
        private void ButtonGiveClicked()
        {
            signalBus?.Fire(new SignalUITransactorGive() { count = currentCount });
        }
        private void ButtonBackClicked()
        {
            signalBus?.Fire(new SignalUITransactorBack());
        }
    }
}