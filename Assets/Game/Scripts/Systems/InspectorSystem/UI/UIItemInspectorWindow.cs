using Game.Systems.InspectorSystem.Signals;
using Game.Systems.LocalizationSystem;
using Game.Systems.LocalizationSystem.Signals;

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
	private LocalizationSystem localization;

	[Inject]
	private void Construct(SignalBus signalBus, LocalizationSystem localization)
	{
		this.signalBus = signalBus;
		this.localization = localization;

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

	private void UpdateInspector()
	{
		var texts = item?.ItemData.GetLocalization(localization.CurrentLanguage) ?? null;

		itemName.text = texts?.itemName ?? "";
		itemDescription.text = texts?.itemDescription ?? "";

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