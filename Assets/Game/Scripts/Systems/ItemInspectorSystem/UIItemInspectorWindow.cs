using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UIItemInspectorWindow : WindowBase
{
	[SerializeField] private Button takeButton;
	[SerializeField] private Button dropButton;

	private SignalBus signalBus;

	[Inject]
	private void Construct(SignalBus signalBus)
	{
		this.signalBus = signalBus;

		takeButton.onClick.AddListener(OnButtonTake);
		dropButton.onClick.AddListener(OnButtonDrop);
	}

	private void OnDestroy()
	{
		takeButton.onClick.RemoveAllListeners();
		dropButton.onClick.RemoveAllListeners();
	}

	private void OnButtonTake()
	{
		signalBus?.Fire(new SignalUITakeItem());
	}

	private void OnButtonDrop()
	{
		signalBus?.Fire(new SignalUIDropItem());
	}
}