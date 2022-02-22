using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
	void Enter();
	IEnumerator Tick();
	void Exit();
}