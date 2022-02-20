using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.InventorySystem
{
    public class UIItemViewer : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI itemName;
        [SerializeField] private TMPro.TextMeshProUGUI itemDescription;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Transform itemTypeContent;
        [SerializeField] private Button useButton;
        [SerializeField] private Button actionsButton;
        [SerializeField] private Button dropButton;

        private Item item;

        private SignalBus signalBus;
        private LocalizationSystem.LocalizationSystem localization;

        [Inject]
        private void Construct(SignalBus signalBus, LocalizationSystem.LocalizationSystem localization)
        {
            this.signalBus = signalBus;
            this.localization = localization;

            useButton.onClick.AddListener(OnUseClicked);
            actionsButton.onClick.AddListener(OnActionsClicked);
            dropButton.onClick.AddListener(OnDropClicked);
        }

        private void OnDestroy()
        {
            useButton.onClick.RemoveAllListeners();
            actionsButton.onClick.RemoveAllListeners();
            dropButton.onClick.RemoveAllListeners();
        }

        public void SetItem(Item item)
        {
            this.item = item;

            UpdateView();
        }

        private void UpdateView()
        {
            itemName.enabled = item != null;
            itemName.text = item?.ItemData.ItemName ?? "";

            itemDescription.enabled = item != null;
            itemDescription.text = item?.ItemData.ItemDescription ?? "";

            itemIcon.enabled = item != null;
            itemIcon.sprite = item?.ItemData.itemSprite ?? null;

            useButton.gameObject.SetActive(false);
            actionsButton.gameObject.SetActive(false);
            dropButton.gameObject.SetActive(item != null);
        }

        private void OnUseClicked()
        {

        }
        private void OnActionsClicked()
        {

        }
        private void OnDropClicked()
        {
            signalBus?.Fire(new SignalUIInventoryDrop() { item = item });
            SetItem(null);
        }
    }
}