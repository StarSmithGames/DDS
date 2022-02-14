using Funly.SkyStudio;

using Game.Systems.TimeSystem;

using System;

using UnityEngine;

using Zenject;

namespace Game.Systems.EnvironmentSystem
{
	public class SkyController : IInitializable, IDisposable
	{
		private SkySettings settings;
		private TimeSystem.TimeSystem timeSystem;
		private TimeOfDayController controller;

		public SkyController(SkySettings settings, TimeSystem.TimeSystem timeSystem, TimeOfDayController controller)
		{
			this.settings = settings;
			this.timeSystem = timeSystem;
			this.controller = controller;
		}

		public void Initialize()
		{
			timeSystem.AddEvent(new TimeEvent()
			{
				 triggerTime = settings.freaquanceSkyTime,
				 onTrigger = Tick,
				 isInfinity = true,
			});
		}

		public void Dispose() { }

		private void Tick()
		{
			controller.UpdateSkyForCurrentTime(timeSystem.GlobalTime.CurrentDayPercent);
			controller.UpdateLunum(timeSystem.GlobalTime.CurrentState);
		}
	}


	[System.Serializable]
	public class SkySettings
	{
		[Tooltip(" ак часто будет обновл€тьс€ погода за один тик.")]
		public TimeSystem.Time freaquanceSkyTime = new TimeSystem.Time() { TotalSeconds = 1 };
	}
}