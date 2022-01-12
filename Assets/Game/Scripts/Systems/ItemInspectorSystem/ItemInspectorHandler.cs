using System;
using UnityEngine;
using Zenject;

public class ItemInspectorHandler : IInitializable, IDisposable, ITickable
{
	private ItemInspectorSettings settings;

	private IItem item;
	private bool isInspection = false;

	private SignalBus signalBus;
	private UIManager uiManager;
	private ItemViewer itemViewer;
	private Player player;
	private Camera camera;

	private Transform cachedItem = null;

	public ItemInspectorHandler
		(GlobalSettings settings,
		SignalBus signalBus,
		UIManager uiManager,
		ItemViewer itemViewer,
		Player player,
		Camera camera)
	{
		this.settings = settings.itemInspector;
		this.signalBus = signalBus;
		this.uiManager = uiManager;
		this.itemViewer = itemViewer;
		this.player = player;
		this.camera = camera;
	}
	public void Initialize()
	{
		signalBus?.Subscribe<SignalUITakeItem>(OnTakeItem);
		signalBus?.Subscribe<SignalUIDropItem>(OnDropItem);
	}
	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalUITakeItem>(OnTakeItem);
		signalBus?.Unsubscribe<SignalUIDropItem>(OnDropItem);
	}

	public void Tick()
	{
		if (cachedItem != null && isInspection)
		{
			if (Input.touchCount > 0)
			{
				float rotX = Input.touches[0].deltaPosition.x * settings.mobileRotationSpeed.x;
				float rotY = Input.touches[0].deltaPosition.y * settings.mobileRotationSpeed.y;

				RotateItem(cachedItem, camera.transform, rotX, rotY);
			}
			if (Input.GetMouseButton(0))
			{
				float rotX = Input.GetAxis("Mouse X") * settings.pcRotationSpeed.x;
				float rotY = Input.GetAxis("Mouse Y") * settings.pcRotationSpeed.y;

				RotateItem(cachedItem, camera.transform, rotX, rotY);
			}
		}
	}

	public void SetItem(ItemModel item)
	{
		cachedItem = item.transform;

		player.Freeze();
		uiManager.Control.DisableButtons();
		itemViewer.SetItem(cachedItem).TransitionIn(
			delegate
			{
				uiManager.Show<UIItemInspectorWindow>();
				isInspection = true;
			});
	}

	private void OnTakeItem(SignalUITakeItem signal)
	{
		isInspection = false;
		uiManager.Hide<UIItemInspectorWindow>();
		GameObject.Destroy(cachedItem.gameObject);
		item = null;
		cachedItem = null;
		player.UnFreeze();
		uiManager.Control.EnableButtons();
	}
	private void OnDropItem(SignalUIDropItem signal)
	{
		isInspection = false;
		itemViewer.TransitionOut(uiManager.Hide<UIItemInspectorWindow>);
		item = null;
		cachedItem = null;
		player.UnFreeze();
		uiManager.Control.EnableButtons();
	}

	private void RotateItem(Transform obj, Transform camera, float rotX, float rotY)
	{
		Vector3 right = Vector3.Cross(camera.up, obj.position - camera.position);
		Vector3 up = Vector3.Cross(obj.position - camera.position, right);
		obj.rotation = Quaternion.AngleAxis(-rotX, up) * obj.rotation;
		obj.rotation = Quaternion.AngleAxis(rotY, right) * obj.rotation;
	}


	[System.Serializable]
	public class ItemInspectorSettings
	{
		public Vector2 mobileRotationSpeed;
		public Vector2 pcRotationSpeed;
	}
}