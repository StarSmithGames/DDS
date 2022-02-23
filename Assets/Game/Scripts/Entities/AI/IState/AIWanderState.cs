using Sirenix.OdinInspector;

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIWanderState : AIBehaviorState
{
	private bool isTest = false;

	private Transform transform;

	private float currentSpeed = 0;
	private float currentAngle = 0;

	private Settings settings;
	private AI ai;
	private NavMeshAgent agent;

	public AIWanderState(Settings settings, AI ai, NavMeshAgent navMeshAgent)
	{
		this.settings = settings;
		transform = ai.transform;
		this.ai = ai;
		agent = navMeshAgent;

		isTest = settings.testDestination != null;
	}

	public override void Enter()
	{
		GenerateDestination();
	}

	public override IEnumerator Tick()
	{
		if (isTest)
		{
			agent.SetDestination(settings.testDestination.position);
		}

		float angle = ai.CalculateAngleToDesination();

		float targetAngle = Mathf.Clamp(angle, -45, 45);
		float targetSpeed = 0.333f;

		if (Mathf.Abs(angle) > 90)//если угол до цели больше, то останавливаемся
		{
			currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, 50f * Time.deltaTime);
			currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, 0.15f * Time.deltaTime);
		}
		else
		{
			if(IsReachedDestination())
			{
				currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, 25f * Time.deltaTime);
				currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, 0.5f * Time.deltaTime);

				if (currentSpeed == 0)
				{
					yield return new WaitForSeconds(Random.Range(0.5f, 3));
					GenerateDestination();
				}
			}
			else if (IsReachesDestination())
			{
				currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, 50f * Time.deltaTime);
				currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 1f * Time.deltaTime);
			}
			//else
			//{
			//	Debug.LogError("WHY I'm HERE");
			//	currentSpeed = Mathf.Max(currentSpeed - 0.25f * Time.deltaTime, targetSpeed);
			//}
		}

		ai.Move(currentSpeed, currentAngle);

		yield return null;
	}

	//достиг цели
	public bool IsReachedDestination()
	{
		return agent.remainingDistance < (settings.useNavMeshStoppingDistance ? agent.stoppingDistance : settings.stoppingDistance) && !agent.pathPending;
	}

	//движение к цели
	public bool IsReachesDestination()
	{
		return agent.remainingDistance >= (settings.useNavMeshStoppingDistance ? agent.stoppingDistance : settings.stoppingDistance) && !agent.pathPending;
	}

	public bool SetDestination(Vector3 destination)
	{
		if (IsPathValid(destination))
		{
			agent.SetDestination(destination);

			return true;
		}
		return false;
	}

	public bool IsPathValid(Vector3 destination)
	{
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath(destination, path);

		return path.status == NavMeshPathStatus.PathComplete;
	}

	private void GenerateDestination()
	{
		Vector3 target = GenerateDestination(transform.position, settings.innerRadius, settings.outerRadius);
		SetDestination(target);
	}

	private Vector3 GenerateDestination(Vector3 origin, float innerRadius, float outerRadius)
	{
		Vector3 destination = RandomExtensions.RandomPointInAnnulus(origin, innerRadius, outerRadius);
		//currentDestination.y = Terrain.activeTerrain.SampleHeight(currentDestination);
		if (!SetDestination(destination))
		{
			return GenerateDestination(origin, innerRadius, outerRadius);
		}

		return destination;
	}


	[System.Serializable]
	public class Settings
	{
		public Transform testDestination;
		[Space]
		public float innerRadius;
		public float outerRadius;
		[Space]
		public bool useNavMeshStoppingDistance = true;
		[HideIf("useNavMeshStoppingDistance")]
		public float stoppingDistance = 2f;
	}
}