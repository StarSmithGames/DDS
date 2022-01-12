using UnityEngine;

using Zenject;

/// <summary>
/// Физическое представление Item.
/// </summary>
public class ItemModel : MonoBehaviour, IEntity, IInteractable
{
	[SerializeField] private Item item;
	public Item Item => item;

	private ItemInspectorHandler itemInspector;

	[Inject]
	private void Construct(ItemInspectorHandler itemInspector)
	{
		this.itemInspector = itemInspector;
	}

	public void Interact()
	{
		itemInspector.SetItem(this);
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
}