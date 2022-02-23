using System.Collections;

public interface IState
{
	void Enter();
	IEnumerator Tick();
	void Exit();
}