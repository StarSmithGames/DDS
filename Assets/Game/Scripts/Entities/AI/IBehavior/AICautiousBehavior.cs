public class AICautiousBehavior : AIBehavior
{
	private AI ai;
	private AIWanderState wanderState;

	public AICautiousBehavior(AI ai, AIWanderState wanderState)
	{
		this.ai = ai;
		this.wanderState = wanderState;
	}

	public override void Initialize()
	{
		FSM = new AIFSM(ai);
		FSM.SetState(wanderState);
	}
}
