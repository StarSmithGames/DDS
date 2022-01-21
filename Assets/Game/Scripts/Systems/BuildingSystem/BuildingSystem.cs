using Game.Entities;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.BuildingSystem
{
	public class BuildingSystem : IInitializable, ITickable, IDisposable
	{
		private IConstruction currentConstruction;

		public bool IsBuildingProcess => buildCoroutine != null;
		private Coroutine buildCoroutine = null;
		private WaitForSeconds buildingSeconds;

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
			buildingSeconds = new WaitForSeconds(0.05f);

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

		public void SetConstruction(IConstruction construction)
		{
			currentConstruction = construction;

			player.DisableVision();
			currentConstruction.IsPlaced = false;
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

		public void Tick()
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				if(currentConstruction == null)
				{
					SetConstruction(constructionFactory.Create(settings.blueprints[0]));
				}
			}
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
			if (IsBuildingProcess)
			{
				Reject();
			}
		}
		private void OnBuildingBuilded(SignalBuildingBuild signal)
		{
			if (IsBuildingProcess)
			{
				Accept();
			}	
		}
		private void OnInputClicked(SignalInputUnPressed signal)
		{
			if (IsBuildingProcess)
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
			private List<ConstructionBlueprint> blueprints;

			public Factory(DiContainer container, BuildingSystemSettings settings)
			{
				this.container = container;
				blueprints = settings.blueprints;
			}

			public IConstruction Create(ConstructionBlueprint param)
			{
				IConstruction construction = null;

				if (blueprints.Any((x) => x == param))
				{
					construction = container.InstantiatePrefab(param.model).GetComponent<IConstruction>();
				}

				return construction;
			}

			public IConstruction Create<T>() where T : IConstruction
			{
				for (int i = 0; i < blueprints.Count; i++)
				{
					if(blueprints[i].model is T)
					{
						return container.InstantiatePrefab(blueprints[i].model).GetComponent<IConstruction>();
					}
				}
				return null;
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

		[Space]
		public List<ConstructionBlueprint> blueprints = new List<ConstructionBlueprint>();
		
	}
	[System.Serializable]
	public class ConstructionBlueprint
	{
		public ConstructionModel model;
	}
}