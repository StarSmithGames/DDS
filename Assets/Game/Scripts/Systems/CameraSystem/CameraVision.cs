using System.Collections;
using UnityEngine;

using Zenject;
using Zenject.SpaceFighter;

public class CameraVision : MonoBehaviour
{
	[SerializeField] private Camera camera;

	private Coroutine visionCoroutine = null;
	public bool IsVisionProccess => visionCoroutine != null;

	private bool isVisionPaused = false;
	public bool IsVisionPaused => IsVisionProccess && isVisionPaused;

	private WaitForFixedUpdate visionSeconds = new WaitForFixedUpdate();

	private Vector3 lastHitPoint;

	private IEntity currentEntity = null;
	private IEntity CurrentEntity
	{
		get => currentEntity;
		set
		{
			if (currentEntity != value)
			{
				currentEntity?.EndObserve();
				currentEntity = value;
				currentEntity?.StartObserve();
			}
			else
			{
				currentEntity?.Observe();
			}
		}
	}

	private VisionSettings settings;
	private UIManager uiManager;
	private InteractionHandler interaction;

	[Inject]
	private void Construct(GlobalSettings data, UIManager uiManager, InteractionHandler interaction)
	{
		this.settings = data.visionSettings;
		this.uiManager = uiManager;
		this.interaction = interaction;

		StartVision();
	}


	#region Vision
	public void StartVision()
	{
		if (!IsVisionProccess)
		{
			visionCoroutine = StartCoroutine(Vision());
		}
	}
	private IEnumerator Vision()
	{
		while (true)
		{
			RaycastHit hit;
			Ray ray = new Ray(camera.transform.position, camera.transform.forward);

			//каст для мира
			if (Physics.Raycast(ray, out hit, settings.maxRayDistance, ~settings.ignoringLayers))
			{
				lastHitPoint = hit.point;

				Collider[] collidersIntersects = Physics.OverlapSphere(hit.point, settings.sphereRadius, settings.interactLayers);

				for (int i = 0; i < collidersIntersects.Length; i++)
				{
					if (collidersIntersects[i] != null)
					{
						Debug.DrawLine(lastHitPoint, collidersIntersects[i].transform.position);
					}
				}

				if (collidersIntersects.Length > 0)
				{
					uiManager.Targets.ShowTarget();
				}
				else
				{
					uiManager.Targets.HideTarget();
				}

				//каст для интерактивных объектов
				if (Physics.Raycast(ray, out hit, settings.rayDistance, settings.interactLayers))
				{
					if (hit.transform.parent == null)
					{
						CurrentEntity = hit.transform.GetComponent<IEntity>();
					}
					else
					{
						CurrentEntity = hit.transform.GetComponentInParent<IEntity>();
					}
				}
				else
				{
					CurrentEntity = null;
				}
			}
			else
			{
				CurrentEntity = null;
				uiManager.Targets.HideTarget();
			}

			CheckEntity();

			yield return visionSeconds;
		}

		StopVision();
	}
	public void PauseVision()
	{
		isVisionPaused = !isVisionPaused;
	}
	public void StopVision()
	{
		if (IsVisionProccess)
		{
			StopCoroutine(visionCoroutine);
			visionCoroutine = null;
		}
	}
	#endregion


	private void CheckEntity()
	{
		if(CurrentEntity == null)
		{
			interaction.Hide();
		}
		else
		{
			if (CurrentEntity is IInteractable interactable)
			{
				interaction.SetTarget(interactable).Show();
			}
		}
	}


	private void OnDrawGizmos()
	{
		if (Application.isPlaying && settings != null)
		{
			Color color = Color.blue;
			color.a = 0.1f;
			Gizmos.color = color;
			Gizmos.DrawSphere(camera.transform.position, settings.maxRayDistance);

			color = Color.red;
			color.a = 0.1f;
			Gizmos.color = color;
			Gizmos.DrawSphere(lastHitPoint, settings.sphereRadius);
		}
	}
}

[System.Serializable]
public class VisionSettings
{
	public LayerMask interactLayers;
	public LayerMask ignoringLayers;
	public float maxRayDistance = 5f;
	public float rayDistance = 5f;
	public float sphereRadius = 1f;
}