using Zenject;

[System.Serializable]
public class PlayerStates
{
	private State currentState = State.Standing;
	public State CurrentState
	{
		get => currentState;
		set
		{
			if(currentState != value)
			{
				currentState = value;

				signalBus?.Fire(new SignalPlayerStateChanged() { state = currentState });
			}
			else
			{

			}
		}
	}

	private SignalBus signalBus;

	public PlayerStates(SignalBus signalBus)
	{
		this.signalBus = signalBus;
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