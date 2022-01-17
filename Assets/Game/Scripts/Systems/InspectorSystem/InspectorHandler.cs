using Game.Entities;
using Game.Systems.InspectorSystem;
using Game.Systems.InspectorSystem.Signals;

using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using UnityEngine;

using Zenject;

public class InspectorHandler : IInitializable, IDisposable, ITickable
{
	private Settings settings;

	private bool isInspection = false;

	private SignalBus signalBus;
	private UIManager uiManager;
	private AsyncManager asyncManager;
	private TransformTransition viewer;
	private Player player;
	private Camera camera;

	private InspectType inspectType;
	private Item currentItem = null;
	private ItemModel currentModel = null;
	private IInventory currentInventory = null;

	public bool IsItemsReviewProcess => itemsReviewCoroutine != null;
	private Coroutine itemsReviewCoroutine = null;

	public InspectorHandler
		(GlobalSettings settings,
		SignalBus signalBus,
		UIManager uiManager,
		AsyncManager asyncManager,
		TransformTransition itemViewer,
		Player player,
		Camera camera)
	{
		this.settings = settings.inspectorSettings;
		this.signalBus = signalBus;
		this.uiManager = uiManager;
		this.asyncManager = asyncManager;
		this.viewer = itemViewer;
		this.player = player;
		this.camera = camera;
	}
	public void Initialize()
	{
		signalBus?.Subscribe<SignalUIInspectorTake>(OnTakeItem);
		signalBus?.Subscribe<SignalUIInspectorUse>(OnUseItem);
		signalBus?.Subscribe<SignalUIInspectorLeave>(OnLeaveItem);
	}
	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalUIInspectorTake>(OnTakeItem);
		signalBus?.Unsubscribe<SignalUIInspectorUse>(OnUseItem);
		signalBus?.Unsubscribe<SignalUIInspectorLeave>(OnLeaveItem);
	}

	public void Tick()
	{
		if (isInspection)
		{
			if(currentModel != null)
			{
				if (Input.touchCount > 0)
				{
					float rotX = Input.touches[0].deltaPosition.x * settings.mobileRotationSpeed.x;
					float rotY = Input.touches[0].deltaPosition.y * settings.mobileRotationSpeed.y;

					RotateItem(currentModel.transform, camera.transform, rotX, rotY);
				}
				if (Input.GetMouseButton(0))
				{
					float rotX = Input.GetAxis("Mouse X") * settings.pcRotationSpeed.x;
					float rotY = Input.GetAxis("Mouse Y") * settings.pcRotationSpeed.y;

					RotateItem(currentModel.transform, camera.transform, rotX, rotY);
				}
			}
		}
	}

	public void SetItem(ItemModel model)
	{
		inspectType = InspectType.Item;

		currentItem = model.Item;
		currentModel = model;

		player.Freeze();
		player.DisableVision();
		uiManager.Controls.DisableButtons();
		viewer
			.SetItem(currentModel.transform)
			.TransitionIn(OpenInspector);
	}

	public void SetInventory(IInventory inventory)
	{
		inspectType = InspectType.Inventory;

		currentInventory = inventory;

		player.Freeze();
		player.DisableVision();
		uiManager.Controls.DisableButtons();

		itemsReviewCoroutine = asyncManager.StartCoroutine(ItemsReview(inventory.Items));
	}

	private IEnumerator ItemsReview(List<Item> items)
	{
		for (int i = items.Count - 1; i >= 0; i--)//safe delete
		{
			yield return new WaitWhile(() => isInspection);

			currentItem = items[i];
			currentModel = GameObject.Instantiate(items[i].ItemData.prefab);
			currentModel.Enable(false);
			viewer.SetItem(currentModel.transform).In();

			OpenInspector();
		}

		itemsReviewCoroutine = null;
	} 

	private void RotateItem(Transform obj, Transform camera, float rotX, float rotY)
	{
		Vector3 right = Vector3.Cross(camera.up, obj.position - camera.position);
		Vector3 up = Vector3.Cross(obj.position - camera.position, right);
		obj.rotation = Quaternion.AngleAxis(-rotX, up) * obj.rotation;
		obj.rotation = Quaternion.AngleAxis(rotY, right) * obj.rotation;
	}

	private void OpenInspector()
	{
		UIItemInspectorWindow window = uiManager.WindowsManager.GetAs<UIItemInspectorWindow>();
		window.SetItem(currentModel.Item);
		uiManager.WindowsManager.Show<UIItemInspectorWindow>();

		isInspection = true;
	}

	private void OnTakeItem(SignalUIInspectorTake signal)
	{
		isInspection = false;

		if (inspectType == InspectType.Item)
		{
			uiManager.WindowsManager.Hide<UIItemInspectorWindow>();

			player.Status.Inventory.Add(currentItem);
			GameObject.Destroy(currentModel.gameObject);

			player.UnFreeze();
			player.EnableVision();
			uiManager.Controls.EnableButtons();
		}
		else if (inspectType == InspectType.Inventory)
		{
			if (!IsItemsReviewProcess)
			{
				uiManager.WindowsManager.Hide<UIItemInspectorWindow>();

				player.Status.Inventory.Add(currentItem);
				currentInventory.Remove(currentItem);
				GameObject.Destroy(currentModel.gameObject);

				player.UnFreeze();
				player.EnableVision();
				uiManager.Controls.EnableButtons();
			}
			else
			{
				player.Status.Inventory.Add(currentItem);
				currentInventory.Remove(currentItem);
				GameObject.Destroy(currentModel.gameObject);
			}
		}
	}

	private void OnUseItem(SignalUIInspectorUse signal)
	{

	}

	private void OnLeaveItem(SignalUIInspectorLeave signal)
	{
		isInspection = false;

		if (inspectType == InspectType.Item)
		{
			viewer
				.SetItem(currentModel.transform)
				.TransitionOut(() =>
				{
					uiManager.WindowsManager.Hide<UIItemInspectorWindow>();

					player.UnFreeze();
					player.EnableVision();
					uiManager.Controls.EnableButtons();
				});
		}
		else if (inspectType == InspectType.Inventory)
		{
			if (!IsItemsReviewProcess)
			{
				uiManager.WindowsManager.Hide<UIItemInspectorWindow>();

				player.UnFreeze();
				player.EnableVision();
				uiManager.Controls.EnableButtons();
			}

			GameObject.Destroy(currentModel.gameObject);
		}
	}

	private enum InspectType
	{
		Item,
		Inventory,
	}

	[System.Serializable]
	public class Settings
	{
		public Vector2 mobileRotationSpeed;
		public Vector2 pcRotationSpeed;
	}
}