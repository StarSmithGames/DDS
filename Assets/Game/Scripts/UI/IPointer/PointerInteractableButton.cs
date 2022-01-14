using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenject;

public class PointerInteractableButton : Button
{
	private UIManager uiManager;

	private IInteractable lastInteract = null;

	public bool IsHoldingProcess => holdingCoroutine != null;
	private Coroutine holdingCoroutine = null;

	private bool isPressed = false;

	[Inject]
	private void Construct(UIManager uiManager)
	{
		this.uiManager = uiManager;
	}

	public PointerInteractableButton Show()
	{
		gameObject.SetActive(true);

		return this;
	}
	public PointerInteractableButton Hide()
	{
		gameObject.SetActive(false);

		HoldingStop();

		return this;
	}

	public PointerInteractableButton SetTarget(IInteractable interact)
	{
		lastInteract = interact;
		return this;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);

		isPressed = true;

		if (lastInteract != null)
		{
			if (lastInteract is IContainer container)
			{
				var interactData = container.IsInspected() ?
													container.ContainerData.interact :
													container.ContainerData.inspect;

				if (interactData.interactableType == InteractableType.Hold)
				{
					if (!IsHoldingProcess)
					{
						holdingCoroutine = StartCoroutine(Holding(interactData.holdDuration));
					}
				}
			}
		}
	}
	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);

		if (lastInteract != null)
		{
			if (lastInteract is ItemModel item)
			{
				var interactData = item.Item.ItemData.interact;
				if (interactData.interactableType == InteractableType.Click)
				{
					if (isPressed)
					{
						lastInteract.Interact();
					}
				}
			}
		}

		isPressed = false;
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
				if(t > 0)
				{
					t -= Time.deltaTime;
				}
				else
				{
					t = 0;
				}
			}

			uiManager.Targets.SetFiller(t / duration);

			if(t > duration)
			{
				break;
			}

			yield return null;
		}

		if (isPressed)
		{
			lastInteract.Interact();
		}

		HoldingStop();
	}
	private void HoldingStop()
	{
		if (IsHoldingProcess)
		{
			StopCoroutine(holdingCoroutine);
			holdingCoroutine = null;

			uiManager.Targets.HideFiller();
		}
	}
}