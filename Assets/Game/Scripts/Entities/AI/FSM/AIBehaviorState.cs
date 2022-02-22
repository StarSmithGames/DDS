using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehaviorState : IState
{
	protected IFSM fsm;
	protected AI ai;

	public AIBehaviorState(IFSM fsm, AI ai)
	{
		this.fsm = fsm;
		this.ai = ai;
	}

	public virtual void Enter() { }

	public virtual IEnumerator Tick()
	{
		yield return null;
	}

	public virtual void Exit() { }
}