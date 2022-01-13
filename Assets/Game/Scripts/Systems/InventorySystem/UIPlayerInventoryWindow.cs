using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UIPlayerInventoryWindow : WindowBase
{
	public UIInventory Inventory => inventory;
	[SerializeField] private UIInventory inventory;

	public UIItemViewer ItemViewer => itemViewer;
	[SerializeField] private UIItemViewer itemViewer;

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
		//signalBus?.Fire(new SignalUIContainerBack());
	}
}