using System.Collections;

public abstract class AIBehaviorState : IState
{
	public virtual void Enter() { }

	public virtual IEnumerator Tick()
	{
		yield return null;
	}

	public virtual void Exit() { }
}