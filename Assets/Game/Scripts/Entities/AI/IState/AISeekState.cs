using Sirenix.OdinInspector;

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIFollowState : AIBehaviorState
{
	private Settings settings;

	private Transform target;

	private Transform transform;
	private NavMeshAgent agent;

	public AIFollowState(Settings settings, AI ai, NavMeshAgent navMeshAgent)
	{
		this.settings = settings;
		transform = ai.transform;
		agent = navMeshAgent;

		SetTarget(target);
	}

	public override IEnumerator Tick()
	{

		yield return null;
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
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


	[System.Serializable]
	public class Settings
	{
		public Transform testTarget;
		[Space]
		public bool useNavMeshStoppingDistance = true;
		[HideIf("useNavMeshStoppingDistance")]
		public float stoppingDistance = 2f;
	}
}