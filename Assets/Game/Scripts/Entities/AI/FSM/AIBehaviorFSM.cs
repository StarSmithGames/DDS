using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviorFSM : IFSM
{
	public bool IsInTransition => isInTransition;
	private bool isInTransition = false;

	public IState CurrentState => currentState;
	private IState currentState;

	private AI ai;

	public AIBehaviorFSM(AI ai)
	{
		this.ai = ai;

		Start();
	}


	private void Start()
	{
		ai.StartCoroutine(Tick());
	}

	public IEnumerator Tick()
	{
		while (true)
		{
			yield return currentState?.Tick();
		}
	}

	public void SetState(IState state)
	{
		if (currentState == state) return;

		isInTransition = true;
		currentState?.Exit();
		currentState = state;
		currentState?.Enter();
		isInTransition = false;
	}
}