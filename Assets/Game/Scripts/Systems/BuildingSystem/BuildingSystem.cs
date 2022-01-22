using Game.Entities;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.BuildingSystem
{
	public class BuildingSystem : IInitializable, IDisposable
	{
		private IConstruction currentConstruction;

		public bool IsBuildingProcess => buildCoroutine != null;
		private Coroutine buildCoroutine = null;

		private bool isReady = false;

		private RaycastHit lastHit;
		private Vector3 lastPosition;
		private Quaternion lastRotation;
		private float lastAngle;

		private SignalBus signalBus;
		private BuildingSystemSettings settings;
		private AsyncManager asyncManager;
		private UIManager uiManager;
		private ConstructionModel.Factory constructionFactory;
		private Player player;
		private Camera camera;

		private bool isDebug = false;

		public BuildingSystem(
			SignalBus signalBus,
			BuildingSystemSettings settings,
			AsyncManager asyncManager,
			UIManager uiManager,
			ConstructionModel.Factory constructionFactory,
			Player player, Camera camera)
		{
			this.signalBus = signalBus;
			this.settings = settings;
			this.asyncManager = asyncManager;
			this.uiManager = uiManager;
			this.constructionFactory = constructionFactory;
			this.player = player;
			this.camera = camera;
		}

		public void Initialize()
		{
			signalBus?.Subscribe<SignalBuildingCancel>(OnBuildingCanceled);
			signalBus?.Subscribe<SignalBuildingBuild>(OnBuildingBuilded);

			signalBus?.Subscribe<SignalInputUnPressed>(OnInputClicked);
		}

		public void Dispose()
		{
			signalBus?.Unsubscribe<SignalBuildingCancel>(OnBuildingCanceled);
			signalBus?.Unsubscribe<SignalBuildingBuild>(OnBuildingBuilded);

			signalBus?.Unsubscribe<SignalInputUnPressed>(OnInputClicked);
		}

		public void SetBlueprint(ConstructionBlueprint blueprint)
		{
			if (currentConstruction == null)
			{
				SetConstruction(constructionFactory.Create(blueprint));
			}
		}

		public void SetConstruction(IConstruction construction)
		{
			isReady = false;

			currentConstruction = construction;
			currentConstruction.IsPlaced = false;

			player.DisableVision();
			uiManager.WindowsManager.Show<UIBuildingWindow>();
			StartBuild();
		}

		private void StartBuild()
		{
			if (!IsBuildingProcess)
			{
				buildCoroutine = asyncManager.StartCoroutine(Building());
			}
		}
		private IEnumerator Building()
		{
			yield return null;//костыль
			isReady = true;

			while (true)
			{
				RaycastHit hit;

				Ray ray = new Ray(camera.transform.position, camera.transform.forward);

				if(isDebug)
					Debug.DrawLine(camera.transform.position, camera.transform.position + (settings.rayDistance * camera.transform.forward), Color.blue);

				//первый чек проверяет на максимум до rayDistance
				if (CheckCast(ray, out hit, settings.rayDistance) == false)
				{
					ray = new Ray(GetSpherePoint(), Vector3.down);
					
					if(isDebug)
						Debug.DrawLine(GetSpherePoint(), GetSpherePoint() + (settings.rayDistance * 2 * Vector3.down), Color.green);

					//второй чек проверяет луч от rayDistance конечной точки вниз Vector.down до rayDistance * 2f
					CheckCast(ray, out hit, settings.rayDistance * 2f);
				}
				yield return null;
			}

			StopBuild();
		}
		private void StopBuild()
		{
			if (IsBuildingProcess)
			{
				asyncManager.StopCoroutine(buildCoroutine);
				buildCoroutine = null;
			}
		}

		private bool CheckCast(Ray ray, out RaycastHit hit, float distance)
		{
			if (Physics.Raycast(ray, out hit, distance, settings.groundLayers))
			{
				lastHit = hit;
				lastPosition = hit.point;
				lastRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
				lastAngle = 90 - (Vector3.Angle(Vector3.up, hit.normal));

				currentConstruction.Transform.position = lastPosition;
				currentConstruction.Transform.rotation = lastRotation;

				if (CheckPlacementAngle(lastAngle) && !currentConstruction.IsIntersectsColliders)
				{
					currentConstruction.SetMaterial(settings.accept);
				}
				else
				{
					currentConstruction.SetMaterial(settings.reject);
				}

				return true;
			}

			return false;
		}
		
		private bool CheckPlacementAngle(float angle)
		{
			if (angle >= settings.anglePlacement)
				return true;
			return false;
		}

		private Vector3 GetSpherePoint()
		{
			return camera.transform.position + (camera.transform.forward * settings.rayDistance);
		}

		private void Accept()
		{
			uiManager.WindowsManager.Hide<UIBuildingWindow>();
			StopBuild();
			currentConstruction.ResetMaterial();
			currentConstruction.IsPlaced = true;
			currentConstruction = null;

			player.EnableVision();
		}
		private void Reject()
		{
			uiManager.WindowsManager.Hide<UIBuildingWindow>();
			StopBuild();
			GameObject.Destroy(currentConstruction.Transform.gameObject);
			currentConstruction = null;

			player.EnableVision();
		}

		private void OnBuildingCanceled(SignalBuildingCancel signal)
		{
			if (IsBuildingProcess && isReady)
			{
				Reject();
			}
		}
		private void OnBuildingBuilded(SignalBuildingBuild signal)
		{
			if (IsBuildingProcess && isReady)
			{
				Accept();
			}	
		}
		private void OnInputClicked(SignalInputUnPressed signal)
		{
			if (IsBuildingProcess && isReady)
			{
				if(signal.input == InputType.Escape)
				{
					Reject();
				}
				else if (signal.input == InputType.BuildingAccept)
				{
					Accept();
				}
				else if (signal.input == InputType.BuildingReject)
				{
					Reject();
				}
			}
		}

		public class Factory : IFactory<ConstructionBlueprint, IConstruction>
		{
			private DiContainer container;

			public Factory(DiContainer container)
			{
				this.container = container;
			}

			public IConstruction Create(ConstructionBlueprint param)
			{
				return container.InstantiatePrefab(param.model).GetComponent<IConstruction>();
			}
		}
	}
	[System.Serializable]
	public class BuildingSystemSettings
	{
		public float rayDistance;
		public float anglePlacement;
		public LayerMask groundLayers;
		[Space]
		public Material accept;
		public Material reject;
	}
}