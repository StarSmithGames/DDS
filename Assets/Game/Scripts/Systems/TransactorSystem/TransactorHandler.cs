using Game.Systems.TransactorSystem.Signals;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using static UnityEditor.Progress;

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

		this.window = uiManager.WindowsManager.GetAs<UIItemTransactorWindow>();
	}

	public void Initialize()
	{
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
			to.Add(item);
		}
	}

	private void OnTransactionAll(SignalUITransactorAll signal)
	{
		from.Remove(item);
		to.Add(item);

		uiManager.WindowsManager.Hide<UIItemTransactorWindow>();
	}
	private void OnTransactionGive(SignalUITransactorGive signal)
	{
		if(signal.count == item.CurrentStackSize)
		{
			from.Remove(item);
			to.Add(item);
		}
		else
		{
			Item newItem = item.Copy();
			newItem.CurrentStackSize = signal.count;
			item.CurrentStackSize = item.CurrentStackSize - newItem.CurrentStackSize;
			to.Add(newItem);
		}

		uiManager.WindowsManager.Hide<UIItemTransactorWindow>();
	}
	private void OnTransactionBack(SignalUITransactorBack signal)
	{
		uiManager.WindowsManager.Hide<UIItemTransactorWindow>();
	}
}