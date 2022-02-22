using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Zenject;

public class AI : MonoBehaviour
{
	public NavMeshAgent NavMeshAgent => navMeshAgent;

	[SerializeField] protected float gravity = -13.0f;
	[Space]
	[SerializeField] protected Animator animator;
	[SerializeField] protected NavMeshAgent navMeshAgent;
	[SerializeField] protected CharacterController characterController;
	[SerializeField] protected FieldOfView fov;

	protected bool isAlive = true;

	private Coroutine brainCoroutine = null;
	public bool IsBrainProccess => brainCoroutine != null;

	protected virtual void Start()
	{
		StartBrain();
	}

	private void StartBrain()
	{
		if (!IsBrainProccess)
		{
			brainCoroutine = StartCoroutine(Brain());
		}
	}
	private IEnumerator Brain()
	{
		while (isAlive)
		{
			yield return null;
		}

		Death();

		StopBrain();
	}
	private void StopBrain()
	{
		if (IsBrainProccess)
		{
			StopCoroutine(brainCoroutine);
			brainCoroutine = null;
		}
	}

	public virtual void MoveTick()
	{

	}

	protected virtual void Death()
	{
		Debug.LogError("Is Dead");
	}

	[Button]
	private void DeathButton()
	{
		isAlive = false;
	}
}