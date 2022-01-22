using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class UIInventorySlot : PoolableObject
	{
		[SerializeField] private Button backgroundButton;
		[SerializeField] private Image icon;
		[Space]
		[SerializeField] private GameObject stackContent;
		[SerializeField] private TMPro.TextMeshProUGUI stackSize;
		[Space]
		[SerializeField] private GameObject eqippedContent;
		[Space]
		[SerializeField] private GameObject durabilityContent;
		[SerializeField] private TMPro.TextMeshProUGUI durability;
		[Space]
		[SerializeField] private GameObject weightContent;
		[SerializeField] private TMPro.TextMeshProUGUI weight;

		public bool IsEmpty => item == null;

		public Item Item => item;
		private Item item;
		private UIInventory owner;

		private SignalBus signalBus;

		[Inject]
		private void Construct(SignalBus signalBus)
		{
			this.signalBus = signalBus;

			backgroundButton.onClick.AddListener(OnButtonClick);
		}

		private void OnDestroy()
		{
			backgroundButton.onClick.RemoveAllListeners();
		}

		public void SetItem(Item item)
		{
			if (this.item != null)
			{
				this.item.OnItemChanged -= UpdateSlot;
			}

			this.item = item;

			if (this.item != null)
			{
				this.item.OnItemChanged += UpdateSlot;
			}

			UpdateSlot();
		}

		public void SetOwner(UIInventory owner)
		{
			this.owner = owner;
		}


		private void UpdateSlot()
		{
			bool isItem = !IsEmpty;

			icon.sprite = isItem ? item.ItemData.itemSprite : null;
			icon.enabled = isItem;

			stackSize.text = isItem ? "<size=18>x</size>" + item.CurrentStackSize : "";
			stackContent.SetActive(isItem ? (item.ItemData.isStackable && item.CurrentStackSize > 1) : false);

			eqippedContent.SetActive(false);

			durability.text = isItem ? item.CurrentDurability + "%" : "";
			durabilityContent.SetActive(isItem ? item.ItemData.isBreakable : false);

			weight.text = isItem ? item.CurrentWeight + "KG" : "";
			weightContent.SetActive(isItem);
		}

		private void OnButtonClick()
		{
			signalBus?.Fire(new SignalUIInventorySlotClick() { inventory = owner, slot = this });
		}

		public class Factory : PlaceholderFactory<UIInventorySlot> { }
	}
}