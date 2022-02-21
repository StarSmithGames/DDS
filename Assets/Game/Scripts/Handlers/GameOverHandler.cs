using Game.Entities;
using Game.Systems.TimeSystem;

using System;

using Zenject;

public class GameOverHandler : IInitializable, IDisposable
{
	private SignalBus signalBus;
	private UIManager uiManager;
	private Player player;
	private TimeSystem timeSystem;

	public GameOverHandler(SignalBus signalBus, UIManager uiManager, Player player, TimeSystem timeSystem)
	{
		this.signalBus = signalBus;
		this.uiManager = uiManager;
		this.player = player;
		this.timeSystem = timeSystem;
	}

	public void Initialize()
	{
		signalBus?.Subscribe<SignalPlayerDied>(OnPlayerDied);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalPlayerDied>(OnPlayerDied);
	}

	private void OnPlayerDied(SignalPlayerDied signal)
	{
		player.Kill();
		timeSystem.StopRewind();
		timeSystem.Pause();
		uiManager.WindowsManager.Show<UIDeathWindow>();
	}
}
