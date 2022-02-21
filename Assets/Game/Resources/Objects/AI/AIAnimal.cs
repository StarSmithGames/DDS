using CMF;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Zenject;

using static AIAnimal;

public class AIAnimal : MonoBehaviour
{
	public AIState CurrentState
	{
		get => currentState;
		set
		{
			currentState = value;
			animator.SetBool(isIdleHash, currentState == AIState.Idle);
		}
	}
	private AIState currentState;

	public NavMeshAgent NavMeshAgent => navMeshAgent;

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
			yield return null;
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

	public void MoveRootMotion()
	{
		Vector3 desiredDiff = navMeshAgent.destination - transform.position;
		Vector3 direction = Quaternion.Inverse(transform.rotation) * desiredDiff.normalized;
		float angleBtwTarget = Mathf.Atan2(direction.x, direction.z) * 180.0f / Mathf.PI;//-180 to 180
		float targetSpeed = 0;

		currentAngle = Mathf.Clamp(angleBtwTarget, -45, 45);

		switch (CurrentState)
		{
			case AIState.Idle:
			{
				targetSpeed = 0f;
				break;
			}
			case AIState.Walk:
			{
				targetSpeed = 0.333f;
				break;
			}
			case AIState.Trot:
			{
				targetSpeed = 0.666f;
				break;
			}
			case AIState.Run:
			{
				targetSpeed = 1f;
				break;
			}
		}
		//
		if (Mathf.Abs(angleBtwTarget) > 90 || navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)//если угол до цели больше, то останавливаемся//достиг цели
		{
			currentSpeed = Mathf.Max(currentSpeed - 0.5f * Time.deltaTime, 0);
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

public abstract class AIBehavior
{
	protected AIAnimal ai;

	public AIBehavior(AIAnimal ai)
	{
		this.ai = ai;
	}

	public abstract IEnumerator Execute();
}

public class AIGoToBehavior : AIBehavior
{
	public AIGoToBehavior(AIAnimal ai) : base(ai) { }

	public override IEnumerator Execute()
	{
		ai.CurrentState = AIState.Walk;
		ai.MoveRootMotion();

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
}

public class AISeekBehavior : AIBehavior
{
	private Transform target;
	private AIGoToBehavior goToBehavior;

	public AISeekBehavior(AIAnimal ai, Transform target) : base(ai)
	{
		this.target = target;
		goToBehavior = new AIGoToBehavior(ai);
	}

	public override IEnumerator Execute()
	{
		goToBehavior.SetDestination(target.position);

		yield return goToBehavior.Execute();
	}
}

public class AIWanderBehavior : AIBehavior
{
	private AIGoToBehavior goToBehavior;

	public AIWanderBehavior(AIAnimal ai) : base(ai)
	{
		goToBehavior = new AIGoToBehavior(ai);
	}

	public override IEnumerator Execute()
	{
		yield return null;
	}

	protected void GenerateDestination(Vector3 origin, float innerRadius, float outerRadius)
	{
		Vector3 currentDestination = RandomExtensions.RandomPointInAnnulus(origin, innerRadius, outerRadius);
		//currentDestination.y = Terrain.activeTerrain.SampleHeight(currentDestination);
		if (!goToBehavior.SetDestination(currentDestination))
		{
			GenerateDestination(origin, innerRadius, outerRadius);
		}
	}
}

public class AIFleeBehavior : AIBehavior
{
	public AIFleeBehavior(AIAnimal ai) : base(ai)
	{
	}

	public override IEnumerator Execute()
	{
		throw new System.NotImplementedException();
	}
}