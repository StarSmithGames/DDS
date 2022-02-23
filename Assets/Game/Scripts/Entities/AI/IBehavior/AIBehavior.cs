public abstract class AIBehavior : IBehavior
{
	public IFSM FSM { get; protected set; }

	public virtual void Initialize() { }
	public virtual void Tick() { }
}