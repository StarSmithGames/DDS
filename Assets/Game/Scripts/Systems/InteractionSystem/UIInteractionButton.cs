using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInteractionButton : Button
{
	public ButtonClickedEvent onPress = new ButtonClickedEvent();
	public ButtonClickedEvent onUnPress = new ButtonClickedEvent();

	public void Show()
	{
		gameObject.SetActive(true);
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);

		if (interactable)
		{
			onPress?.Invoke();
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);

		if (interactable)
		{
			onUnPress?.Invoke();
		}
	}
}