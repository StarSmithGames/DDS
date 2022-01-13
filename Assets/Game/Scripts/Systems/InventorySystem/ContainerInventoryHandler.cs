using Game.Entities;
using Game.Signals;

using System;
using UnityEngine;

using Zenject;

public class ContainerInventoryHandler : IInitializable, IDisposable
{
	private IContainer container;

	private SignalBus signalBus;
	private UIManager uiManager;
	private Player player;

	public ContainerInventoryHandler
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
		signalBus?.Subscribe<SignalUIWindowsBack>(OnContainerBack);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalUIWindowsBack>(OnContainerBack);
	}

	public void SetContainer(IContainer container)
	{
		this.container = container;
		player.Freeze();
		uiManager.WindowsManager.Show<UIContainerInventoryWindow>();
	}

	private void OnContainerBack(SignalUIWindowsBack signal)
	{
		uiManager.WindowsManager.Hide<UIContainerInventoryWindow>();
		player.UnFreeze();
	}
}