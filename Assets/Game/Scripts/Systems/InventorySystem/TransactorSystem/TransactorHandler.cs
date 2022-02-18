using System;
using Zenject;

namespace Game.Systems.InventorySystem.Transactor
{
	public class TransactorHandler : IInitializable, IDisposable
	{
		private UIItemTransactorWindow window;

		private Item item;
		private IInventory from, to;

		private SignalBus signalBus;
		private UIManager uiManager;

		public TransactorHandler(SignalBus signalBus, UIManager uiManager)
		{
			this.signalBus = signalBus;
			this.uiManager = uiManager;
		}

		public void Initialize()
		{
			this.window = uiManager.WindowsManager.GetAs<UIItemTransactorWindow>();

			signalBus?.Subscribe<SignalUITransactorAll>(OnTransactionAll);
			signalBus?.Subscribe<SignalUITransactorGive>(OnTransactionGive);
			signalBus?.Subscribe<SignalUITransactorBack>(OnTransactionBack);
		}

		public void Dispose()
		{
			signalBus?.Unsubscribe<SignalUITransactorAll>(OnTransactionAll);
			signalBus?.Unsubscribe<SignalUITransactorGive>(OnTransactionGive);
			signalBus?.Unsubscribe<SignalUITransactorBack>(OnTransactionBack);
		}

		/// <summary>
		/// ѕередача одного айтема откуда-то куда-то.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="from"></param>
		/// <param name="to">ћожет быть null, тогда from будет считатьс€ как общее пространство(мир)</param>
		public void Transact(Item item, IInventory from, IInventory to)
		{
			this.item = item;
			this.from = from;
			this.to = to;

			if (item.ItemData.isStackable && item.CurrentStackSize != 1)
			{
				window.SetTransaction(item);
				uiManager.WindowsManager.Show<UIItemTransactorWindow>();
			}
			else
			{
				from.Remove(item);

				if (to != null)
				{
					to.Add(item);
				}
				else
				{

				}
			}
		}

		private void OnTransactionAll(SignalUITransactorAll signal)
		{
			from.Remove(item);
			if (to != null)
			{
				to.Add(item);
			}
			else
			{

			}

			uiManager.WindowsManager.Hide<UIItemTransactorWindow>();
		}
		private void OnTransactionGive(SignalUITransactorGive signal)
		{
			if (signal.count == item.CurrentStackSize)
			{
				from.Remove(item);
				if (to != null)
				{
					to.Add(item);
				}
				else
				{

				}
			}
			else
			{
				Item newItem = item.Copy();
				newItem.CurrentStackSize = signal.count;
				item.CurrentStackSize = item.CurrentStackSize - newItem.CurrentStackSize;

				if (to != null)
				{
					to.Add(newItem);
				}
			}

			uiManager.WindowsManager.Hide<UIItemTransactorWindow>();
		}
		private void OnTransactionBack(SignalUITransactorBack signal)
		{
			uiManager.WindowsManager.Hide<UIItemTransactorWindow>();
		}
	}
}