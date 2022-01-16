using Game.Systems.LocalizationSystem;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

/// <summary>
/// Физическое представление Item.
/// </summary>
public class ItemModel : MonoBehaviour, IEntity, IInteractable
{
	[SerializeField] private Item item;
	public Item Item => item;

	[SerializeField] private Collider coll;

	private InspectorHandler itemInspector;
	private UIManager uiManager;
	private LocalizationSystem localization;

	[Inject]
	private void Construct(InspectorHandler itemInspector, UIManager uiManager, LocalizationSystem localization)
	{
		this.itemInspector = itemInspector;
		this.uiManager = uiManager;
		this.localization = localization;
	}

	public void Interact()
	{
		itemInspector.SetItem(this);
	}

	public void Enable(bool trigger)
	{
		if (coll == null) coll = GetComponent<Collider>();
		coll.enabled = trigger;
	}

	public void StartObserve()
	{
		var texts = item.ItemData.GetLocalization(localization.CurrentLanguage);
		uiManager.Targets.ShowTargetInformation(texts.itemName);
	}
	public void Observe()
	{
	}
	public void EndObserve()
	{
		uiManager.Targets.HideTargetInformation();
	}

	[Button]
	private void SavePositionAndRoation()
	{
		//Item.ItemData.prefabPossitionOffsetView = transform.position;
	}

	
}