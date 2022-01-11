using UnityEngine;

using Zenject;

public class Item : MonoBehaviour, IItem
{
	public ItemData ItemData => data;
	[SerializeField] private ItemData data;

	private ItemInspectorHandler itemInspector;

	[Inject]
	private void Construct(ItemInspectorHandler itemInspector)
	{
		this.itemInspector = itemInspector;
	}

	public void Interact()
	{
		itemInspector.PutItem(this);
	}

	public void StartObserve()
	{
	}

	public void Observe()
	{
	}

	public void EndObserve()
	{
	}
}