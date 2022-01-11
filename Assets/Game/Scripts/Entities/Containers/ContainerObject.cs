using UnityEngine;

using Zenject;

public class ContainerObject : MonoBehaviour, IContainer
{
	public ContainerData ContainerData => containerData;
	[SerializeField] private ContainerData containerData;

	private UIManager uiManager;

	private Data data;

	[Inject]
	private void Construct(UIManager uiManager)
	{
		this.uiManager = uiManager;

		if(data == null)
		{
			data = new Data();
		}
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