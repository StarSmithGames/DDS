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

	private Coroutine coroutine;

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

		HoldStop();

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
			switch (lastInteract)
			{
				case IContainer container:
				{
					var interactData = container.IsInspected() ?
														container.ContainerData.interact :
														container.ContainerData.inspect;

					if (interactData.interactableType == InteractableType.Hold)
					{
						coroutine = StartCoroutine(Holding(interactData.holdDuration));
					}
					break;
				}
				
			}
		}
	}
	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);

		if (lastInteract != null)
		{
			switch (lastInteract)
			{
				case IItem item:
				{
					var interactData = item.Item.ItemData.interact;
					if (interactData.interactableType == InteractableType.Click)
					{
						if (isPressed)
						{
							lastInteract.Interact();
						}
					}

					break;
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

		HoldStop();
	}
	private void HoldStop()
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = null;

			uiManager.Targets.HideFiller();
		}
	}
}