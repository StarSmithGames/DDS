using Game.Systems.InspectorSystem.Signals;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UIItemInspectorWindow : WindowBase
{
	[SerializeField] private Transform typeContent;
	[SerializeField] private TMPro.TextMeshProUGUI itemName;
	[SerializeField] private TMPro.TextMeshProUGUI itemDescription;
	[SerializeField] private Button takeButton;
	[SerializeField] private Button useButton;
	[SerializeField] private Button leaveButton;

	private Item item;

	private SignalBus signalBus;

	[Inject]
	private void Construct(SignalBus signalBus)
	{
		this.signalBus = signalBus;

		takeButton.onClick.AddListener(OnButtonTake);
		useButton.onClick.AddListener(OnButtonUse);
		leaveButton.onClick.AddListener(OnButtonLeave);
	}

	private void OnDestroy()
	{
		takeButton.onClick.RemoveAllListeners();
		useButton.onClick.RemoveAllListeners();
		leaveButton.onClick.RemoveAllListeners();
	}

	public void SetItem(Item item)
	{
		this.item = item;

		UpdateInspector();
	}

	public void SetItems(List<Item> items)
	{

	}

	private void UpdateInspector()
	{
		itemName.text = item.ItemData.itemName;
		itemDescription.text = item.ItemData.itemDescription;

		useButton.gameObject.SetActive(item.IsConsumable);
	}

	private void OnButtonTake()
	{
		signalBus?.Fire(new SignalUIInspectorTake() { item = item});
	}

	private void OnButtonUse()
	{
		signalBus?.Fire(new SignalUIInspectorUse() { item = item });
	}

	private void OnButtonLeave()
	{
		signalBus?.Fire(new SignalUIInspectorLeave() { item = item});
	}
}