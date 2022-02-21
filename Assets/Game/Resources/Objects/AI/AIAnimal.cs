using CMF;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class AIAnimal : MonoBehaviour
{
	public Transform destinationTest;

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
	[Space]
	[SerializeField] private Animator animator;
	[SerializeField] private NavMeshAgent navMeshAgent;
	[SerializeField] private CharacterController characterController;

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

	protected Vector3 currentDestination;

	private Vector3 rootMotion;

	[Inject]
	private void Construct()
	{
		navMeshAgent.updatePosition = false;
		navMeshAgent.angularSpeed = 0;

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
			yield return GoTo();
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

	protected virtual IEnumerator GoTo()
    {
		if (destinationTest != null)
		{
			currentDestination = destinationTest.position;
			navMeshAgent.SetDestination(currentDestination);
			CurrentState = AIState.Walk;
			MoveAIRootMotion();

			yield return null;
		}
	}


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

	float currentSpeed = 0;
	float currentAngle = 0;

	//Seek, Flee
	protected void MoveAIRootMotion()
	{
		Vector3 desiredDiff = navMeshAgent.destination - transform.position;
		Vector3 direction = Quaternion.Inverse(transform.rotation) * desiredDiff.normalized;
		float angleBtwTarget = Mathf.Atan2(direction.x, direction.z) * 180.0f / Mathf.PI;//-180 to 180
		float targetSpeed = 0;

		switch (CurrentState)
		{
			case AIState.Idle:
			{
				targetSpeed = 0f;
				currentAngle = Mathf.Clamp(angleBtwTarget, -45, 45);
				break;
			}
			case AIState.Walk:
			{
				targetSpeed = 0.333f;
				currentAngle = Mathf.Clamp(angleBtwTarget, -45, 45);
				break;
			}
			case AIState.Trot:
			{
				targetSpeed = 0.666f;
				currentAngle = Mathf.Clamp(angleBtwTarget, -45, 45);
				break;
			}
			case AIState.Run:
			{
				targetSpeed = 1f;
				currentAngle = Mathf.Clamp(angleBtwTarget, -45, 45);
				break;
			}
		}
		//
		if (Mathf.Abs(angleBtwTarget) > 90 || navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)//если угол до цели больше, то останавливаемся//достиг цели
		{
			targetSpeed = 0f;
			currentSpeed = Mathf.Max(currentSpeed - 0.5f * Time.deltaTime, targetSpeed);
		}
		else
		{
			if (navMeshAgent.remainingDistance >= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)//движение к цели
			{
				currentSpeed = Mathf.Min(currentSpeed + 1f * Time.deltaTime, targetSpeed);
			}
			else
			{
				currentSpeed = Mathf.Max(currentSpeed - 0.5f * Time.deltaTime, targetSpeed);
			}
		}

		animator.SetFloat(velocityHash, currentSpeed);//, 0.25f, Time.deltaTime);//Velocity
		animator.SetFloat(directionHash, currentAngle);//Direction
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

		if (navMeshAgent == null) return;
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(Application.isPlaying ? currentDestination : transform.position, navMeshAgent.stoppingDistance);

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
		Trot,
		Run,
	}
}