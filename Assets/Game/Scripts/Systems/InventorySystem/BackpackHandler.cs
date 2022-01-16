using Game.Entities;
using Game.Signals;
using Game.Systems.InventorySystem;
using Game.Systems.InventorySystem.Signals;
using Game.Systems.TransactorSystem.Signals;

using System;
using System.ComponentModel;
using System.Transactions;

using Zenject;

public class BackpackHandler : IInitializable, IDisposable
{
	private UIPlayerInventoryWindow playerInventoryWindow;
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
		playerInventoryWindow = uiManager.WindowsManager.GetAs<UIPlayerInventoryWindow>();
		playerInventoryWindow.Inventory.SetInventory(player.Inventory);
		playerInventoryWindow.ItemViewer.SetItem(null);

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
		player.DisableVision();

		playerInventoryWindow.Container.SetInventory(inventory);
		ReOrganizeSpace(InventoryType.InventoryWithContainer);
		uiManager.WindowsManager.Show<UIPlayerInventoryWindow>();
	}

	public void ReOrganizeSpace(InventoryType type)
	{
		if (type == InventoryType.InventoryWithViewer)
		{
			playerInventoryWindow.Container.gameObject.SetActive(false);
			playerInventoryWindow.ItemViewer.gameObject.SetActive(true);

		}
		else if (type == InventoryType.InventoryWithContainer)
		{
			playerInventoryWindow.Container.gameObject.SetActive(true);
			playerInventoryWindow.ItemViewer.gameObject.SetActive(false);
		}

		inventoryType = type;
	}


	private void OnSlotClicked(SignalUIInventorySlotClick signal)
	{
		if(inventoryType == InventoryType.InventoryWithViewer)
		{
			playerInventoryWindow.ItemViewer.SetItem(signal.slot.Item);
		}
		else if (inventoryType == InventoryType.InventoryWithContainer)
		{
			if (!signal.slot.IsEmpty)
			{
				if (signal.inventory == playerInventoryWindow.Inventory)
				{
					transactor.Transact(signal.slot.Item, signal.inventory.Inventory, playerInventoryWindow.Container.Inventory);
				}
				else if (signal.inventory == playerInventoryWindow.Container)
				{
					transactor.Transact(signal.slot.Item, signal.inventory.Inventory, playerInventoryWindow.Inventory.Inventory);
				}
			}
		}
	}

	private void OnItemDroped(SignalUIInventoryDrop signal)
	{
		transactor.Transact(signal.item, player.Inventory, null);
	}

	private void OnWindowsBack(SignalUIWindowsBack signal)
	{
		playerInventoryWindow.ItemViewer.SetItem(null);
		playerInventoryWindow.Container.SetInventory(null);
		player.UnFreeze();
		player.EnableVision();
	}

	private void OnInputClicked(SignalInputClicked signal)
	{
		if(signal.input == InputType.Inventory)
		{
			playerInventoryWindow.ItemViewer.SetItem(null);
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