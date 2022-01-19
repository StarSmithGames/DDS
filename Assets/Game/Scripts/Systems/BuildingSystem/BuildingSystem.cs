using Funly.SkyStudio;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.BuildingSystem
{
	public class BuildingSystem : IInitializable, ITickable, IDisposable
	{
		private ConstructionModel currentConstruction;

		public bool IsBuildingProcess => buildCoroutine != null;
		private Coroutine buildCoroutine = null;
		private WaitForSeconds buildingSeconds;

		private RaycastHit lastHit;

		private Vector3 lastPosition;
		private Quaternion lastRotation;
		private float lastAngle;

		private BuildingSystemSettings settings;
		private AsyncManager asyncManager;
		private Camera camera;

		public BuildingSystem(BuildingSystemSettings settings, AsyncManager asyncManager, Camera camera)
		{
			this.settings = settings;
			this.asyncManager = asyncManager;
			this.camera = camera;
			Debug.LogError(camera != null);
		}

		public void Initialize()
		{
			buildingSeconds = new WaitForSeconds(0.05f);
		}

		public void Dispose()
		{

		}

		public void SetConstruction(ConstructionModel model)
		{
			currentConstruction = model;

			StartBuild();
		}

		private void StartBuild()
		{
			if (!IsBuildingProcess)
			{
				buildCoroutine = asyncManager.StartCoroutine(Building());
			}
			else
			{
				StopBuild();
			}
		}
		private IEnumerator Building()
		{
			while (true)
			{
				RaycastHit hit;

				Ray ray = new Ray(camera.transform.position, camera.transform.forward);

				//первый чек проверяет на максимум до rayDistance
				if (CheckCast(ray, out hit, settings.rayDistance) == false)
				{
					ray = new Ray(GetSpherePoint(), Vector3.down);
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

				//cameraService.UnLockVision();

				//GeneralAvailability.PlayerUI.OpenRadialMenu();

				//onEndBuild?.Invoke();
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

				if (CheckPlacementAngle(lastAngle)/* && !currentBuilding.IsIntersects*/)
					Draw(true);
				else
					Draw(false);

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

		private void Draw(bool trigger)
		{
			//isCanBuild = trigger;

			currentConstruction.transform.position = lastPosition;
			currentConstruction.transform.rotation = lastRotation;

			//currentBuilding.IsPlacement = false;

			currentConstruction.SetMaterial(trigger ? settings.accept : settings.reject);
		}


		public void Tick()
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				Debug.LogError("HERER");

				ConstructionModel model = GameObject.Instantiate(settings.model);
				SetConstruction(model);
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
		public ConstructionModel model;
	}
}