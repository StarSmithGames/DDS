using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdleState : AIBehaviorState
{
	private AI ai;

	private int lastIdle = -1;

	public AIIdleState(AI ai)
	{
		this.ai = ai;
	}

	public override void Enter()
	{
		GenerateRandomIdle();
	}

	public override IEnumerator Tick()
	{
		yield return new WaitForSeconds(Random.Range(3f, 15f));
		GenerateRandomIdle();
	}

	private void GenerateRandomIdle()
	{
		int idle = Random.Range(0, 8);

		if(idle == lastIdle)
		{
			GenerateRandomIdle();
			return;
		}

		lastIdle = idle;

		ai.SetIdleIndex(idle);
	}
}