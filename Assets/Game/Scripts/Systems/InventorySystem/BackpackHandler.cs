using Game.Entities;
using Game.Managers.InputManger;
using Game.Signals;
using Game.Systems.InventorySystem.Transactor;

using System;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class BackpackHandler : IInitializable, IDisposable
	{
		private UIPlayerBackpackWindow playerBackpackWindow;
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
			playerBackpackWindow = uiManager.WindowsManager.GetAs<UIPlayerBackpackWindow>();
			playerBackpackWindow.InventoryWindow.Inventory.SetInventory(player.Status.Inventory);
			playerBackpackWindow.InventoryWindow.ItemViewer.SetItem(null);

			playerBackpackWindow.onCanceled += OnBackpackCanceled;

			signalBus?.Subscribe<SignalUIInventorySlotClick>(OnSlotClicked);
			signalBus?.Subscribe<SignalUIInventoryDrop>(OnItemDroped);

			signalBus?.Subscribe<SignalInputUnPressed>(OnInputClicked);
		}

		public void Dispose()
		{
			playerBackpackWindow.onCanceled -= OnBackpackCanceled;

			signalBus?.Unsubscribe<SignalUIInventorySlotClick>(OnSlotClicked);
			signalBus?.Unsubscribe<SignalUIInventoryDrop>(OnItemDroped);

			signalBus?.Unsubscribe<SignalInputUnPressed>(OnInputClicked);
		}

		public void OpenWindow()
		{
			player.Freeze();
			player.DisableVision();
			uiManager.Controls.DisableButtons();

			playerBackpackWindow.InventoryWindow.ItemViewer.SetItem(null);
			ReOrganizeSpace(InventoryType.InventoryWithViewer);
			playerBackpackWindow.Show();
		}

		public void CloseWindow()
		{
			playerBackpackWindow.InventoryWindow.ItemViewer.SetItem(null);
			playerBackpackWindow.InventoryWindow.Container.SetInventory(null);
			player.UnFreeze();
			player.EnableVision();
			uiManager.Controls.EnableButtons();
			playerBackpackWindow.Hide();
		}

		public void SetContainerInventory(IInventory inventory)
		{
			player.Freeze();
			player.DisableVision();

			playerBackpackWindow.InventoryWindow.Container.SetInventory(inventory);
			ReOrganizeSpace(InventoryType.InventoryWithContainer);
			playerBackpackWindow.Show();
		}

		public void ReOrganizeSpace(InventoryType type)
		{
			if (type == InventoryType.InventoryWithViewer)
			{
				playerBackpackWindow.InventoryWindow.Container.gameObject.SetActive(false);
				playerBackpackWindow.InventoryWindow.ItemViewer.gameObject.SetActive(true);

			}
			else if (type == InventoryType.InventoryWithContainer)
			{
				playerBackpackWindow.InventoryWindow.Container.gameObject.SetActive(true);
				playerBackpackWindow.InventoryWindow.ItemViewer.gameObject.SetActive(false);
			}

			inventoryType = type;
		}


		private void OnSlotClicked(SignalUIInventorySlotClick signal)
		{
			if (inventoryType == InventoryType.InventoryWithViewer)
			{
				playerBackpackWindow.InventoryWindow.ItemViewer.SetItem(signal.slot.Item);
			}
			else if (inventoryType == InventoryType.InventoryWithContainer)
			{
				if (!signal.slot.IsEmpty)
				{
					if (signal.inventory == playerBackpackWindow.InventoryWindow.Inventory)
					{
						transactor.Transact(signal.slot.Item, signal.inventory.Inventory, playerBackpackWindow.InventoryWindow.Container.Inventory);
					}
					else if (signal.inventory == playerBackpackWindow.InventoryWindow.Container)
					{
						transactor.Transact(signal.slot.Item, signal.inventory.Inventory, playerBackpackWindow.InventoryWindow.Inventory.Inventory);
					}
				}
			}
		}

		private void OnItemDroped(SignalUIInventoryDrop signal)
		{
			transactor.Transact(signal.item, player.Status.Inventory, null);
		}

		private void OnBackpackCanceled()
		{
			CloseWindow();
		}

		private void OnInputClicked(SignalInputUnPressed signal)
		{
			if (signal.input == InputType.Escape && playerBackpackWindow.IsShowing)
			{
				CloseWindow();
			}
			else if (signal.input == InputType.Inventory)
			{
				if (playerBackpackWindow.IsShowing)
				{
					CloseWindow();
				}
				else
				{
					OpenWindow();
				}
			}
		}

		public enum InventoryType
		{
			InventoryWithViewer,
			InventoryWithContainer,
		}
	}
}