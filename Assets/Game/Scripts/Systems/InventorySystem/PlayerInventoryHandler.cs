using Game.Entities;
using Game.Systems.InventorySystem.Signals;

using System;

using Zenject;

public class PlayerInventoryHandler : IInitializable, IDisposable
{
	private UIPlayerInventoryWindow window;

	private SignalBus signalBus;
	private UIManager uiManager;
	private Player player;

	public PlayerInventoryHandler
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

		signalBus?.Subscribe<SignalUISlotClick>(OnSlotClicked);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalUISlotClick>(OnSlotClicked);
	}

	private void OnSlotClicked(SignalUISlotClick signal)
	{
		window.ItemViewer.SetItem(signal.slot.Item);
	}
}