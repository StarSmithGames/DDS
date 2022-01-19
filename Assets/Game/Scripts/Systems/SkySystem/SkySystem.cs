using Funly.SkyStudio;

using Game.Systems.TimeSystem;

using System;
using System.Collections;
using UnityEngine;

using Zenject;

public class SkySystem : IInitializable, IDisposable
{
	public bool IsPaused => isPaused;
	private bool isPaused = false;
	public bool IsSkyProcess => skyCoroutine != null;
	private Coroutine skyCoroutine = null;
	private WaitForSeconds seconds = null;

	private SkySettings settings;
	private TimeSystem timeSystem;
	private TimeOfDayController controller;
	private AsyncManager asyncManager;

	public SkySystem(
		GlobalSettings settings,
		TimeSystem timeSystem,
		TimeOfDayController controller,
		AsyncManager asyncManager)
	{
		this.settings = settings.skySettings;
		this.timeSystem = timeSystem;
		this.controller = controller;
		this.asyncManager = asyncManager;
	}

	public void Initialize()
	{
		seconds = new WaitForSeconds(settings.skyUpdate);
		skyCoroutine = asyncManager.StartCoroutine(Sky());
	}

	public void Dispose()
	{
		asyncManager.StopCoroutine(skyCoroutine);
		skyCoroutine = null;
	}

	private IEnumerator Sky()
	{
		while (true)
		{
			while (isPaused)
			{
				yield return null;
			}

			controller.UpdateSkyForCurrentTime(timeSystem.GlobalTime.CurrentDayPercent);
			controller.UpdateLunum(timeSystem.GlobalTime.CurrentState);
			yield return seconds;
		}
	}

	public void Pause()
	{
		isPaused = true;
	}
	public void UnPause()
	{
		isPaused = false;
	}
}
[System.Serializable]
public class SkySettings
{
	[Min(0)]
	public float skyUpdate;
}