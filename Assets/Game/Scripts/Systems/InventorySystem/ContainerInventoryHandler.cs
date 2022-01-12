using System;
using UnityEngine;

using Zenject;
using Zenject.SpaceFighter;

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
		signalBus?.Subscribe<SignalUIContainerBack>(OnContainerBack);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalUIContainerBack>(OnContainerBack);
	}

	public void SetContainer(IContainer container)
	{
		this.container = container;
		player.Freeze();
		uiManager.Show<UIContainerInventoryWindow>();
	}

	private void OnContainerBack(SignalUIContainerBack signal)
	{
		uiManager.Hide<UIContainerInventoryWindow>();
		player.UnFreeze();
	}
}