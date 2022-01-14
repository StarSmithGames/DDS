using UnityEngine;

using Zenject;

public class UIPlayerInventoryWindow : WindowBase
{
	public UIInventory Inventory => inventory;
	[SerializeField] private UIInventory inventory;

	public UIInventory Container => container;
	[SerializeField] private UIInventory container;

	public UIItemViewer ItemViewer => itemViewer;
	[SerializeField] private UIItemViewer itemViewer;

	private SignalBus signalBus;

	[Inject]
	private void Construct(SignalBus signalBus)
	{
		this.signalBus = signalBus;
	}

	public void ReOrganizeSpace(InventoryType inventory)
	{
		if(inventory == InventoryType.InventoryWithViewer)
		{
			container.gameObject.SetActive(false);
			itemViewer.gameObject.SetActive(true);
		}
		else if(inventory == InventoryType.InventoryWithContainer)
		{
			container.gameObject.SetActive(true);
			itemViewer.gameObject.SetActive(false);
		}
	}

	public enum InventoryType
	{
		InventoryWithViewer,
		InventoryWithContainer,
	}
}