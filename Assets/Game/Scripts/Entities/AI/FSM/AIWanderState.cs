using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIWanderState : AIBehaviorState
{
	private Settings settings;

	private Vector3 target;


	public AIWanderState(IFSM fsm, AI ai, Settings settings) : base(fsm, ai)
	{
		this.settings = settings;
	}

	public override void Enter()
	{
		target = GenerateDestination(ai.transform.position, settings.innerRadius, settings.outerRadius);
		SetDestination(target);

		Debug.LogError("Start Wander " + target);
	}

	public override IEnumerator Tick()
	{
		Debug.LogError("Wander");

		ai.MoveTick();

		yield return null;
	}


	public bool SetDestination(Vector3 destination)
	{
		if (IsPathValid(destination))
		{
			ai.NavMeshAgent.SetDestination(destination);

			return true;
		}
		return false;
	}

	public bool IsPathValid(Vector3 destination)
	{
		NavMeshPath path = new NavMeshPath();
		ai.NavMeshAgent.CalculatePath(destination, path);

		return path.status == NavMeshPathStatus.PathComplete;
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
		public float innerRadius;
		public float outerRadius;
	}
}