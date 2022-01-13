using Game.Systems.InventorySystem.Signals;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UISlot : PoolableObject
{
    [SerializeField] private Button backgroundButton;
	[SerializeField] private Image icon;

	public Item Item => item;
	private Item item;

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
		this.item = item;
		
		UpdateSlot();
	}

	public bool IsEmpty() => item == null;

	private void UpdateSlot()
	{
		icon.enabled = item != null;
		icon.sprite = item?.ItemData.itemSprite ?? null;
	}

	private void OnButtonClick()
	{
		signalBus?.Fire(new SignalUISlotClick() { slot = this });
	}

	public class Factory : PlaceholderFactory<UISlot> { }
}