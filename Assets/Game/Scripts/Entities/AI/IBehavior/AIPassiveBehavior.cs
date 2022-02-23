public class AIPassiveBehavior : AIBehavior
{
	private AI ai;
	private AIWanderState wanderState;
	private AIIdleState idleState;

	public AIPassiveBehavior(AI ai, AIWanderState wanderState, AIIdleState idleState)
	{
		this.ai = ai;
		this.wanderState = wanderState;
		this.idleState = idleState;
	}

	public override void Initialize()
	{
		FSM = new AIFSM(ai);
		FSM.SetState(idleState);
	}
}
