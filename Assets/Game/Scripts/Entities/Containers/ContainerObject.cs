using UnityEngine;

using Zenject;

public class ContainerObject : MonoBehaviour, IContainer
{
	public ContainerData ContainerData => containerData;
	[SerializeField] private ContainerData containerData;

	public IInventory Inventory { get; private set; }

	private ContainerInventoryHandler containerInventory;

	private Data data;

	[Inject]
	private void Construct(ContainerInventoryHandler containerInventory)
	{
		this.containerInventory = containerInventory;

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
			Debug.LogError("Opened");
		}
		else
		{
			Debug.LogError("Opening");
		}

		data.isInspected = true;

		containerInventory.SetContainer(this);
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