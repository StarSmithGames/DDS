using UnityEngine;

using Zenject;

public class ContainerObject : MonoBehaviour, IContainer
{
	public ContainerData ContainerData => containerData;
	[SerializeField] private ContainerData containerData;

	[SerializeField] private Collider coll;

	public IInventory Inventory { get; private set; }

	private InspectorHandler inspector;
	private BackpackHandler backpack;

	private Data data;

	[Inject]
	private void Construct(BackpackHandler backpack, InspectorHandler inspector)
	{
		this.inspector = inspector;
		this.backpack = backpack;

		if(data == null)
		{
			data = new Data();
		}

		Inventory = new Inventory(ContainerData.inventory);
	}

	public void Interact()
	{
		if (data.isInspected)
		{
			backpack.SetContainerInventory(Inventory);
		}
		else
		{
			inspector.SetInventory(Inventory);
		}

		data.isInspected = true;
	}

	public void Enable(bool trigger)
	{
		if (coll == null) coll = GetComponent<Collider>();
		coll.enabled = trigger;
	}


	public bool IsInspected() => data.isInspected;

	public void StartObserve()
	{
	}

	public void Observe()
	{

	}

	public void EndObserve()
	{
	}

	
	public class Data
	{
		public bool isInspected = false;
	}
}