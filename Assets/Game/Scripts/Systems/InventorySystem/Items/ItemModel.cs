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

	[Inject]
	private void Construct(InspectorHandler itemInspector)
	{
		this.itemInspector = itemInspector;
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

	public void Observe()
	{
	}
	public void StartObserve()
	{
	}
	public void EndObserve()
	{
	}

	[Button]
	private void SavePositionAndRoation()
	{
		//Item.ItemData.prefabPossitionOffsetView = transform.position;
	}

	
}