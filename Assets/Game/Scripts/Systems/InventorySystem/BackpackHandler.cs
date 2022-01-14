using Game.Entities;
using Game.Signals;
using Game.Systems.InventorySystem;
using Game.Systems.InventorySystem.Signals;

using System;

using Zenject;

public class BackpackHandler : IInitializable, IDisposable
{
	private UIPlayerInventoryWindow window;

	private SignalBus signalBus;
	private UIManager uiManager;
	private Player player;

	public BackpackHandler
		(SignalBus signalBus,
		UIManager uiManager,
		Player player)
	{
		this.signalBus = signalBus;
		this.uiManager = uiManager;
		this.player = player;
}

	public void Initialize()
	{
		window = uiManager.WindowsManager.GetAs<UIPlayerInventoryWindow>();
		window.Inventory.SetInventory(player.Inventory);
		window.ItemViewer.SetItem(null);

		signalBus?.Subscribe<SignalUIInventorySlotClick>(OnSlotClicked);
		signalBus?.Subscribe<SignalUIInventoryDrop>(OnItemDroped);
		signalBus?.Subscribe<SignalUIWindowsBack>(OnWindowsBack);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalUIInventorySlotClick>(OnSlotClicked);
		signalBus?.Unsubscribe<SignalUIInventoryDrop>(OnItemDroped);
		signalBus?.Unsubscribe<SignalUIWindowsBack>(OnWindowsBack);
	}

	public void SetContainerInventory(IInventory inventory)
	{
		player.Freeze();

		UIPlayerInventoryWindow window = uiManager.WindowsManager.GetAs<UIPlayerInventoryWindow>();
		window.Container.SetInventory(inventory);
		window.ReOrganizeSpace(UIPlayerInventoryWindow.InventoryType.InventoryWithContainer);
		uiManager.WindowsManager.Show<UIPlayerInventoryWindow>();
	}

	private void OnSlotClicked(SignalUIInventorySlotClick signal)
	{
		window.ItemViewer.SetItem(signal.slot.Item);
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
}