using System.Collections;

public interface IFSM
{
	IEnumerator Tick();

	IState CurrentState { get; }

	void SetState(IState state);
}