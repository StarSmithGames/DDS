using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class InteractionHandler : IInitializable, IDisposable
{
	private bool isPressed = false;
	private IInteractable interact = null;

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
		signalBus?.Subscribe<SignalInputClicked>(OnInputClicked);
		signalBus?.Subscribe<SignalInputPressed>(OnInputPressed);
		signalBus?.Subscribe<SignalInputUnPressed>(OnInputUnPressed);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalInputClicked>(OnInputClicked);
		signalBus?.Unsubscribe<SignalInputPressed>(OnInputPressed);
		signalBus?.Unsubscribe<SignalInputUnPressed>(OnInputUnPressed);
	}

	public InteractionHandler SetTarget(IInteractable interact)
	{
		this.interact = interact;

		return this;
	}

	public void Show()
	{
		if(globalSettings.projectSettings.platform == PlatformType.Mobile)
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
		uiManager.Targets.ShowFiller();

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

			uiManager.Targets.SetFiller(t / duration);

			if (t > duration)
			{
				break;
			}

			yield return null;
		}

		if (isPressed)
		{
			interact.Interact();
		}

		HoldingStop();
	}
	private void HoldingStop()
	{
		if (IsHoldingProcess)
		{
			asyncManager.StopCoroutine(holdingCoroutine);
			holdingCoroutine = null;

			uiManager.Targets.HideFiller();
		}
	}

	private void OnInputClicked(SignalInputClicked signal)
	{
		if(signal.input == InputType.Interaction)
		{
			if (interact != null)
			{
				if (interact is ItemModel item)
				{
					var interactData = item.Item.ItemData.interact;
					if (interactData.interactionType == InteractionType.Click)
					{
						interact.Interact();
					}
				}
			}
		}
	}
	private void OnInputPressed(SignalInputPressed signal)
	{
		if (signal.input == InputType.Interaction)
		{
			isPressed = true;

			if (interact != null)
			{
				if (interact is IContainer container)
				{
					var interactData = container.IsSearched ?
														container.ContainerData.interact :
														container.ContainerData.inspect;

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
		}
	}
}