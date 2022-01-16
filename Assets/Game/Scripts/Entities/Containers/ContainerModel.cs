using Game.Systems.LocalizationSystem;

using UnityEngine;

using Zenject;

public class ContainerModel : MonoBehaviour, IContainer
{
	public ContainerData ContainerData => containerData;
	[SerializeField] private ContainerData containerData;

	[SerializeField] private Collider coll;

	public IInventory Inventory { get; private set; }
	public bool IsSearched => data.isSearched;

	private Data data;

	private InspectorHandler inspector;
	private BackpackHandler backpack;
	private UIManager uiManager;
	private LocalizationSystem localization;

	[Inject]
	private void Construct(BackpackHandler backpack, InspectorHandler inspector, UIManager uiManager, LocalizationSystem localization)
	{
		this.inspector = inspector;
		this.backpack = backpack;
		this.uiManager = uiManager;
		this.localization = localization;

		if (data == null)
		{
			data = new Data();
		}

		Inventory = new Inventory(ContainerData.inventory);
	}


	public void Enable(bool trigger)
	{
		if (coll == null) coll = GetComponent<Collider>();
		coll.enabled = trigger;
	}

	public void Interact()
	{
		if (IsSearched)
		{
			backpack.SetContainerInventory(Inventory);
		}
		else
		{
			inspector.SetInventory(Inventory);
		}

		data.isSearched = true;
	}

	public void StartObserve()
	{
		var text = containerData.GetLocalization(localization.CurrentLanguage);

		if (IsSearched)
		{
			if (Inventory.Items.Count == 0)
			{
				uiManager.Targets.ShowTargetInformation(text.containerName, "Empty");
			}
			else
			{
				uiManager.Targets.ShowTargetInformation(text.containerName, "Searched");
			}
		}
		else
		{
			uiManager.Targets.ShowTargetInformation(text.containerName);
		}
	}

	public void Observe()
	{

	}

	public void EndObserve()
	{
		uiManager.Targets.HideTargetInformation();
	}


	public class Data
	{
		public bool isSearched = false;
	}
}