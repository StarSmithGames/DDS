using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UIContainerInventoryWindow : WindowBase
{
	[SerializeField] private Button backButton;

	private SignalBus signalBus;

	[Inject]
	private void Construct(SignalBus signalBus)
	{
		this.signalBus = signalBus;

		backButton.onClick.AddListener(OnButtonBack);
	}
	private void OnDestroy()
	{
		backButton.onClick.RemoveAllListeners();
	}

	private void OnButtonBack()
	{
		signalBus?.Fire(new SignalUIContainerBack());
	}
}