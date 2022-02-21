using CMF;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class AIRabbit : MonoBehaviour
{
	public AIState CurrentState
	{
		get => currentState;
		protected set
		{
			currentState = value;
			animator.SetBool(isIdleHash, currentState == AIState.Idle);
		}
	}
	private AIState currentState;

	public float wanderInnerRadius;
	public float wanderOuterRadius;

	[SerializeField] private float gravity = -13.0f;
	[SerializeField] private float acceleration = 1;
	[SerializeField] private float deceleration = 0.5f;
	[Space]
	[SerializeField] private Animator animator;
	[SerializeField] private NavMeshAgent navMeshAgent;
	[SerializeField] private CharacterController characterController;
	[Range(0, 1)]
	[SerializeField] private float velocity;
	[Range(0, 1)]
	[SerializeField] private float direction;

	bool isAlive = true;

	protected int idleIndex = 0;
	protected int deathIndex = 0;

	protected int isIdleHash = -1;
	protected int idleIndexHash = -1;
	protected int velocityHash = -1;
	protected int directionHash = -1;

	protected int deathIndexHash = -1;

	private Coroutine brainCoroutine = null;
	public bool IsBrainProccess => brainCoroutine != null;

	private float walkAnimationSpeed = 1;
	private float runAnimationSpeed = 1;

	protected Vector3 currentDestination;

	private Vector3 rootMotion;

	[Inject]
	private void Construct()
	{
		navMeshAgent.updatePosition = false;
		isIdleHash = Animator.StringToHash("IsIdle");
		idleIndexHash = Animator.StringToHash("Idle");
		velocityHash = Animator.StringToHash("Velocity");
		directionHash = Animator.StringToHash("Direction");

		//viewsTargers = view.visibleTargets;
	}

	private void Start()
	{
		StartBrain();
	}

	#region Brain
	private void StartBrain()
	{
		if (!IsBrainProccess)
		{
			//view.StartView();

			brainCoroutine = StartCoroutine(Brain());
		}
	}
	private IEnumerator Brain()
	{
		while (isAlive)
		{
			yield return Behavior();
		}
		//death
		StopBrain();
	}
	private void StopBrain()
	{
		if (IsBrainProccess)
		{
			StopCoroutine(brainCoroutine);
			brainCoroutine = null;

			//StartCoroutine(Death());
		}
	}
	#endregion

	protected virtual IEnumerator Behavior() 
	{
		if (navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)
		{
			CurrentState = AIState.Walk;
			MoveAIRootMotion();
		}
		else
		{
			CurrentState = AIState.Idle;
			yield return new WaitForSeconds(Random.Range(0.25f, 3f));
			GenerateDestination(transform.position, wanderInnerRadius, wanderOuterRadius);
		}

		//else
		//{
		//	if (waypointTimer == 0)
		//	{
		//		CurrentState = AIState.Idle;
		//	}

		//	waypointTimer += Time.deltaTime;

		//	if (waypointTimer >= waitTime)
		//	{

		//		if (Random.Range(0, 100) < 65)
		//		{
		//			CurrentState = AIState.Walk;
		//		}
		//		else
		//		{
		//			CurrentState = AIState.Run;
		//		}

		//		

		//		idleIndex = Random.Range(1, 7);
		//		animator.SetInteger(idleIndexHash, idleIndex);

		//		waitTime = Random.Range(3, 7);
		//		waypointTimer = 0;
		//	}
		//}

		yield return null;
	}



	//private void Update()
	//{
	//	
	//	animator.SetFloat(velocityHash, velocity);
	//	animator.SetFloat(directionHash, direction);
	//}
	private void Update()
	{
		if (!characterController.isGrounded)
		{
			rootMotion.y += gravity * Time.deltaTime;
		}
		characterController.Move(rootMotion);
		rootMotion = Vector3.zero;
	}


	private void OnAnimatorMove()
	{
		navMeshAgent.nextPosition = transform.position;

		rootMotion += animator.deltaPosition;

		//Transform.position = animator.rootPosition;
		transform.rotation = animator.rootRotation;
	}

	protected void MoveAIRootMotion()
	{
		float speed = navMeshAgent.desiredVelocity.magnitude;
		Vector3 direction = Quaternion.Inverse(transform.rotation) * navMeshAgent.desiredVelocity;
		float angle = Mathf.Atan2(direction.x, direction.z) * 180.0f / Mathf.PI;

		if (navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)
		{
			if (CurrentState == AIState.Run)
			{
				navMeshAgent.speed = Mathf.Min(navMeshAgent.speed + acceleration * Time.deltaTime, runAnimationSpeed);
			}
			else if (CurrentState == AIState.Walk)
			{
				if (walkAnimationSpeed <= 1f)
				{
					if (Mathf.Abs(angle) > 100)
					{
						navMeshAgent.speed = 0;
					}
					navMeshAgent.speed = Mathf.Max(navMeshAgent.speed - 1 * Time.deltaTime, walkAnimationSpeed * 0.5f);
				}
				else if (walkAnimationSpeed > 1f)
				{
					navMeshAgent.speed = Mathf.Max(navMeshAgent.speed - 1 * Time.deltaTime, 0.5f);
				}
			}
		}
		else if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)
		{
			navMeshAgent.speed = Mathf.Max(navMeshAgent.speed - deceleration * Time.deltaTime, 0f);
		}
		else
		{
			navMeshAgent.speed = Mathf.Max(navMeshAgent.speed - deceleration * Time.deltaTime, 0f);
		}

		animator.SetFloat(directionHash, angle);
		animator.SetFloat(velocityHash, speed);//, 0.3f, Time.deltaTime);
	}

	protected void GenerateDestination(Vector3 origin, float innerRadius, float outerRadius)
	{
		currentDestination = RandomExtensions.RandomPointInAnnulus(origin, innerRadius, outerRadius);
		//currentDestination.y = Terrain.activeTerrain.SampleHeight(currentDestination);
		if (CheckPath(navMeshAgent, currentDestination))
		{
			navMeshAgent.SetDestination(currentDestination);
		}
		else
		{
			GenerateDestination(origin, innerRadius, outerRadius);
		}
	}

	public virtual void Enable(bool trigger)
	{
		animator.enabled = trigger;
		characterController.enabled = trigger;
		navMeshAgent.enabled = trigger;
		//view.StopView();

		//for (int i = 0; i < hitZones.Count; i++)
		//{
		//	hitZones[i].Collider.enabled = false;
		//}

		//interaction.enabled = true;
	}

	protected bool CheckPath(NavMeshAgent agent, Vector3 destination)
	{
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath(destination, path);

		return path.status == NavMeshPathStatus.PathComplete;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, wanderInnerRadius);
		Gizmos.DrawWireSphere(transform.position, wanderOuterRadius);

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(currentDestination, 0.25f);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(currentDestination, navMeshAgent.stoppingDistance);

		//path
		Gizmos.color = Color.red;
		Vector3[] corners = navMeshAgent.path.corners;

		for (int i = 0; i < corners.Length - 1; i++)
		{
			Gizmos.DrawLine(corners[i], corners[i + 1]);
		}
	}


	public enum AIState
	{
		Idle,
		Walk,
		Run,
	}
}