//using UnityEngine;
//using BehaviorDesigner.Runtime;
//using BehaviorDesigner.Runtime.Tasks;
//using UnityEngine.AI;
//using static AIAnimal;

//public class AIGoTo
//{
//	public float speed = 0;
//	public float wanderInnerRadius;
//	public float wanderOuterRadius;
//	//public SharedTransform target;

//	private AIAnimal ai;
//	private Vector3 target;

//	public override void OnAwake()
//	{
//		ai = GetComponent<AIAnimal>();

//		target = GenerateDestination(transform.position, wanderInnerRadius, wanderOuterRadius);
//		SetDestination(target);
//	}

//	public override TaskStatus OnUpdate()
//	{
//		ai.CurrentState = AIState.Walk;
//		ai.MoveRootMotion();

//		if (Vector3.SqrMagnitude(transform.position - target) < 0.1f)
//		{
//			return TaskStatus.Success;
//		}
//		transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
//		return TaskStatus.Running;
//	}

//	protected Vector3 GenerateDestination(Vector3 origin, float innerRadius, float outerRadius)
//	{
//		Vector3 destination = RandomExtensions.RandomPointInAnnulus(origin, innerRadius, outerRadius);
//		//currentDestination.y = Terrain.activeTerrain.SampleHeight(currentDestination);
//		if (!SetDestination(destination))
//		{
//			return GenerateDestination(origin, innerRadius, outerRadius);
//		}

//		return destination;
//	}


//	public bool SetDestination(Vector3 destination)
//	{
//		if (IsPathValid(destination))
//		{
//			ai.NavMeshAgent.SetDestination(destination);

//			return true;
//		}
//		return false;
//	}

//	public bool IsPathValid(Vector3 destination)
//	{
//		NavMeshPath path = new NavMeshPath();
//		ai.NavMeshAgent.CalculatePath(destination, path);

//		return path.status == NavMeshPathStatus.PathComplete;
//	}

//	public override void OnDrawGizmos()
//	{
//		Gizmos.color = Color.green;
//		Gizmos.DrawWireSphere(transform.position, wanderInnerRadius);
//		Gizmos.DrawWireSphere(transform.position, wanderOuterRadius);
//	}
//}