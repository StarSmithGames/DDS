using Game.Entities;
using Game.Signals;
using Game.Systems.InventorySystem;
using Game.Systems.InventorySystem.Signals;

using System;
using System.ComponentModel;

using Zenject;

public class BackpackHandler : IInitializable, IDisposable
{
	private UIPlayerInventoryWindow window;
	private InventoryType inventoryType;

	private SignalBus signalBus;
	private UIManager uiManager;
	private Player player;
	private TransactorHandler transactor;

	public BackpackHandler
		(SignalBus signalBus,
		UIManager uiManager,
		Player player,
		TransactorHandler transactor)
	{
		this.signalBus = signalBus;
		this.uiManager = uiManager;
		this.player = player;
		this.transactor = transactor;
	}

	public void Initialize()
	{
		window = uiManager.WindowsManager.GetAs<UIPlayerInventoryWindow>();
		window.Inventory.SetInventory(player.Inventory);
		window.ItemViewer.SetItem(null);

		signalBus?.Subscribe<SignalUIInventorySlotClick>(OnSlotClicked);
		signalBus?.Subscribe<SignalUIInventoryDrop>(OnItemDroped);
		signalBus?.Subscribe<SignalUIWindowsBack>(OnWindowsBack);

		signalBus?.Subscribe<SignalInputClicked>(OnInputClicked);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalUIInventorySlotClick>(OnSlotClicked);
		signalBus?.Unsubscribe<SignalUIInventoryDrop>(OnItemDroped);
		signalBus?.Unsubscribe<SignalUIWindowsBack>(OnWindowsBack);

		signalBus?.Unsubscribe<SignalInputClicked>(OnInputClicked);
	}

	public void SetContainerInventory(IInventory inventory)
	{
		player.Freeze();

		window.Container.SetInventory(inventory);
		ReOrganizeSpace(InventoryType.InventoryWithContainer);
		uiManager.WindowsManager.Show<UIPlayerInventoryWindow>();
	}

	public void ReOrganizeSpace(InventoryType type)
	{
		if (type == InventoryType.InventoryWithViewer)
		{
			window.Container.gameObject.SetActive(false);
			window.ItemViewer.gameObject.SetActive(true);

		}
		else if (type == InventoryType.InventoryWithContainer)
		{
			window.Container.gameObject.SetActive(true);
			window.ItemViewer.gameObject.SetActive(false);
		}

		inventoryType = type;
	}


	private void OnSlotClicked(SignalUIInventorySlotClick signal)
	{
		if(inventoryType == InventoryType.InventoryWithViewer)
		{
			window.ItemViewer.SetItem(signal.slot.Item);
		}
		else if (inventoryType == InventoryType.InventoryWithContainer)
		{
			if (!signal.slot.IsEmpty)
			{
				if (signal.inventory == window.Inventory)
				{
					transactor.Transact(signal.slot.Item, signal.inventory.Inventory, window.Container.Inventory);
				}
				else if (signal.inventory == window.Container)
				{
					transactor.Transact(signal.slot.Item, signal.inventory.Inventory, window.Inventory.Inventory);
				}
			}
		}
	}

	private void OnItemDroped(SignalUIInventoryDrop signal)
	{
		player.Inventory.Remove(signal.item);
	}

	private void OnWindowsBack(SignalUIWindowsBack signal)
	{
		window.ItemViewer.SetItem(null);
		window.Container.SetInventory(null);
		player.UnFreeze();
	}

	private void OnInputClicked(SignalInputClicked signal)
	{
		if(signal.input == InputType.Inventory)
		{
			window.ItemViewer.SetItem(null);
			ReOrganizeSpace(InventoryType.InventoryWithViewer);
			uiManager.WindowsManager.Show<UIPlayerInventoryWindow>();
		}
	}

	public enum InventoryType
	{
		InventoryWithViewer,
		InventoryWithContainer,
	}
}