using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerStates
{
	public UnityAction<State> onStateChanged;

	[SerializeField] private State currentState = State.Standing;
	public State CurrentState
	{
		get => currentState;
		set
		{
			if(currentState != value)
			{
				currentState = value;
				onStateChanged?.Invoke(currentState);
			}
			else
			{

			}
		}
	}

	public enum State
	{
		Sleeping,
		Resting,
		Standing,
		Walking,
		Sprinting,
		Climbing,
	}
}