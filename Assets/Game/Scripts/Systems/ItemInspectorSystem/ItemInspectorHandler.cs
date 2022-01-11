using System;
using UnityEngine;
using Zenject;

public class ItemInspectorHandler : IInitializable, IDisposable, ITickable
{
	private IItem item;
	private bool isItemIn = false;

	private SignalBus signalBus;
	private UIManager uiManager;
	private ItemViewer itemViewer;
	private Player player;

	public ItemInspectorHandler(SignalBus signalBus,
		UIManager uiManager,
		ItemViewer itemViewer,
		Player player)
	{
		this.signalBus = signalBus;
		this.uiManager = uiManager;
		this.itemViewer = itemViewer;
		this.player = player;
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
		if(item != null)
		{
			if (isItemIn)
			{
				float rotationSpeedX = 10f;
				float rotationSpeedY = 10f;

				if (Input.touchCount > 0)
				{
					float rotX = Input.touches[0].deltaPosition.x * rotationSpeedX;
					float rotY = Input.touches[0].deltaPosition.y * rotationSpeedY;

					RotateItem((item as Item).transform, Camera.main.transform, rotX, rotY);
				}
				if (Input.GetMouseButton(0))
				{
					float rotX = Input.GetAxis("Mouse X") * rotationSpeedX;
					float rotY = Input.GetAxis("Mouse Y") * rotationSpeedY;

					RotateItem((item as Item).transform, Camera.main.transform, rotX, rotY);
				}
			}
		}
	}

	private void RotateItem(Transform obj, Transform camera, float rotX, float rotY)
	{
		Vector3 right = Vector3.Cross(camera.up, obj.position - camera.position);
		Vector3 up = Vector3.Cross(obj.position - camera.position, right);
		obj.rotation = Quaternion.AngleAxis(-rotX, up) * obj.rotation;
		obj.rotation = Quaternion.AngleAxis(rotY, right) * obj.rotation;
	}

	public void PutItem(IItem item)
	{
		this.item = item;
		
		player.Freeze();
		uiManager.Control.DisableButtons();
		itemViewer.SetItem(item as Item).TransitionIn(
			delegate
			{
				uiManager.Show<UIItemInspectorWindow>();
				isItemIn = true;
				//uiManager.Control.PlayerInspect.Show();
			});
	}

	private void OnTakeItem(SignalUITakeItem signal)
	{
		uiManager.Hide<UIItemInspectorWindow>();
		GameObject.Destroy((item as Item).gameObject);
		item = null;
		player.UnFreeze();
		uiManager.Control.EnableButtons();
	}
	private void OnDropItem(SignalUIDropItem signal)
	{
		itemViewer.TransitionOut(uiManager.Hide<UIItemInspectorWindow>);
		player.UnFreeze();
		uiManager.Control.PlayerInspect.Hide();
		uiManager.Control.EnableButtons();
	}
}