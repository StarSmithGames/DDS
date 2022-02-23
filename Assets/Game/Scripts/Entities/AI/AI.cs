using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class AI : MonoBehaviour
{
	protected float gravity = -13.0f;
	protected Vector3 rootMotion;

	protected int isIdleHash = -1;
	protected int isDeadHash = -1;
	protected int velocityHash = -1;
	protected int directionHash = -1;
	protected int idleIndexHash = -1;
	protected int deathIndexHash = -1;

	protected Animator animator;
	protected NavMeshAgent navMeshAgent;
	protected CharacterController characterController;

	[Inject]
	private void Construct(Animator animator, NavMeshAgent navMeshAgent, CharacterController characterController)
	{
		this.animator = animator;
		this.navMeshAgent = navMeshAgent;
		this.characterController = characterController;
		
		isIdleHash = Animator.StringToHash("IsIdle");
		isDeadHash = Animator.StringToHash("IsDead");
		idleIndexHash = Animator.StringToHash("Idle");
		velocityHash = Animator.StringToHash("Velocity");
		directionHash = Animator.StringToHash("Direction");

		navMeshAgent.updatePosition = false;
		navMeshAgent.angularSpeed = 0;
	}

	protected virtual void Update()
	{
		if (!characterController.isGrounded)
		{
			rootMotion.y += gravity * Time.deltaTime;
		}
		characterController.Move(rootMotion);
		rootMotion = Vector3.zero;
	}

	protected void OnAnimatorMove()
	{
		navMeshAgent.nextPosition = transform.position;

		rootMotion += animator.deltaPosition;

		//transform.position = animator.rootPosition;
		transform.rotation = animator.rootRotation;
	}

	public void Move(float velocity, float direction)
	{
		animator.SetFloat(velocityHash, velocity);//, 0.25f, Time.deltaTime);//Velocity
		animator.SetFloat(directionHash, direction);
	}

	public void SetIdleIndex(int index, bool isIdle = true)
	{
		animator.SetBool(isIdleHash, isIdle);
		animator.SetInteger(idleIndexHash, index);
	}

	public void SetDeathIndex(int index, bool isDied = true)
	{
		animator.SetBool(isDeadHash, isDied);
		animator.SetFloat(deathIndexHash, index);
	}

	//-180 to 180
	public float CalculateAngleToDesination()
	{
		Vector3 desiredDiff = navMeshAgent.destination - transform.position;
		Vector3 direction = Quaternion.Inverse(transform.rotation) * desiredDiff.normalized;
		return Mathf.Atan2(direction.x, direction.z) * 180.0f / Mathf.PI;
	}
}