using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class AIAnimal : AI
{
	protected float currentSpeed = 0;
	protected float currentAngle = 0;

	protected Vector3 rootMotion;

	protected int idleIndex = 0;
	protected int deathIndex = 0;

	protected int isIdleHash = -1;
	protected int isDeadHash = -1;
	protected int velocityHash = -1;
	protected int directionHash = -1;
	protected int idleIndexHash = -1;
	protected int deathIndexHash = -1;

	[Inject]
	private void Construct()
	{
		navMeshAgent.updatePosition = false;
		navMeshAgent.angularSpeed = 0;

		isIdleHash = Animator.StringToHash("IsIdle");
		isDeadHash = Animator.StringToHash("IsDead");
		idleIndexHash = Animator.StringToHash("Idle");
		velocityHash = Animator.StringToHash("Velocity");
		directionHash = Animator.StringToHash("Direction");
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

	protected override void Death()
	{
		animator.SetFloat(deathIndexHash, deathIndex);
		animator.SetBool(isDeadHash, true);
	}

	public override void MoveTick()
	{
		MoveRootMotion();
	}

	public void MoveRootMotion()
	{
		Vector3 desiredDiff = navMeshAgent.destination - transform.position;
		Vector3 direction = Quaternion.Inverse(transform.rotation) * desiredDiff.normalized;
		float angleBtwTarget = Mathf.Atan2(direction.x, direction.z) * 180.0f / Mathf.PI;//-180 to 180
		float targetSpeed = 0.333f;

		currentAngle = Mathf.Clamp(angleBtwTarget, -45, 45);

		//switch (CurrentState)
		//{
		//	case AIState.Idle:
		//	{
		//		targetSpeed = 0f;
		//		break;
		//	}
		//	case AIState.Walk:
		//	{
		//		targetSpeed = 0.333f;
		//		break;
		//	}
		//	case AIState.Trot:
		//	{
		//		targetSpeed = 0.666f;
		//		break;
		//	}
		//	case AIState.Run:
		//	{
		//		targetSpeed = 1f;
		//		break;
		//	}
		//}

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
		if (navMeshAgent == null) return;

		//path
		Gizmos.color = Color.red;
		Vector3[] corners = navMeshAgent.path.corners;

		for (int i = 0; i < corners.Length - 1; i++)
		{
			Gizmos.DrawLine(corners[i], corners[i + 1]);
		}
	}
}