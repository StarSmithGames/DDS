using Game.Managers.InputManger;
using Game.Systems.BuildingSystem;
using Game.Systems.InventorySystem;

using System;
using System.Collections;
using UnityEngine;

using Zenject;

public class InteractionHandler : IInitializable, IDisposable
{
	private bool isPressed = false;
	private IInteractable interactable = null;

	public bool IsHoldingProcess => holdingCoroutine != null;
	private Coroutine holdingCoroutine = null;

	private SignalBus signalBus;
	private GlobalSettings globalSettings;
	private UIManager uiManager;
	private AsyncManager asyncManager;

	public InteractionHandler
		(SignalBus signalBus,
		GlobalSettings globalSettings,
		UIManager uiManager,
		AsyncManager asyncManager)
	{
		this.globalSettings = globalSettings;
		this.uiManager = uiManager;
		this.asyncManager = asyncManager;
		this.signalBus = signalBus;
	}

	public void Initialize()
	{
		signalBus?.Subscribe<SignalInputPressed>(OnInputPressed);
		signalBus?.Subscribe<SignalInputUnPressed>(OnInputUnPressed);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalInputPressed>(OnInputPressed);
		signalBus?.Unsubscribe<SignalInputUnPressed>(OnInputUnPressed);
	}

	public InteractionHandler SetTarget(IInteractable interact)
	{
		this.interactable = interact;

		return this;
	}

	public void Show()
	{
		if (globalSettings.projectSettings.platform == PlatformType.Mobile)
		{
			uiManager.Controls.ButtonA.Show();
		}
	}

	public void Hide()
	{
		if (globalSettings.projectSettings.platform == PlatformType.Mobile)
		{
			uiManager.Controls.ButtonA.Hide();
		}

		HoldingStop();
	}

	private IEnumerator Holding(float duration)
	{
		uiManager.Targets.Filler.ShowFiller();

		float t = 0;

		while (true)
		{
			if (isPressed)
			{
				t += Time.deltaTime;
			}
			else
			{
				if (t > 0)
				{
					t -= Time.deltaTime;
				}
				else
				{
					t = 0;
				}
			}

			uiManager.Targets.Filler.SetFiller(t / duration);

			if (t > duration)
			{
				break;
			}

			yield return null;
		}

		if (isPressed)
		{
			interactable.Interact();
		}

		HoldingStop();
	}
	private void HoldingStop()
	{
		if (IsHoldingProcess)
		{
			asyncManager.StopCoroutine(holdingCoroutine);
			holdingCoroutine = null;

			uiManager.Targets.Filler.HideFiller();
		}
	}

	private InteractionSettings GetSettingsFromInteractable(IInteractable interactable)
	{
		if (this.interactable is IContainer container)
		{
			if (container.IsSearched)
				return container.ContainerData.useBasicInteraction ? globalSettings.basicInteraction : container.ContainerData.interact;
			else
				return container.ContainerData.inspect;
		}
		else if (this.interactable is IConstruction construction)
		{
			return construction.ConstructionData.useBasicInteraction ? globalSettings.basicInteraction : construction.ConstructionData.interact;
		}

		return null;
	}

	private void OnInputPressed(SignalInputPressed signal)
	{
		if (signal.input == InputType.Interaction)
		{
			isPressed = true;

			if (interactable != null)
			{
				InteractionSettings interactData = GetSettingsFromInteractable(interactable);

				if(interactData != null)
				{
					if (interactData.interactionType == InteractionType.Hold)
					{
						if (!IsHoldingProcess)
						{
							holdingCoroutine = asyncManager.StartCoroutine(Holding(interactData.holdDuration));
						}
					}
				}
			}
		}
	}
	private void OnInputUnPressed(SignalInputUnPressed signal)
	{
		if (signal.input == InputType.Interaction)
		{
			isPressed = false;

			if (interactable != null)
			{
				if (interactable is ItemModel item)
				{
					var interactData = item.Item.ItemData.interact;
					if (interactData.interactionType == InteractionType.Click)
					{
						interactable.Interact();
					}
				}
			}
		}
	}
}