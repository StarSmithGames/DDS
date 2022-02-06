using Game.Systems.IgnitionSystem;

using Newtonsoft.Json.Linq;

using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Systems.BuildingSystem
{
    public class FireConstruction : ConstructionModel
    {
		public override bool IsCreated
		{
			get => base.IsCreated;
			set
			{
				base.IsCreated = value;

				if(value == true)
				{
					isCompleted = false;
				}
			}
		}

		/// <summary>
		/// Завершена ли конструкция.
		/// </summary>
		public bool IsCompleted { get => isCompleted; set => isCompleted = value; }
		private bool isCompleted = false;

		public bool IsFireEnabled 
		{
			get => isFireEnabled;
			set
			{
				isFireEnabled = value;
				particles.SetActive(isFireEnabled);
			}
		}
		private bool isFireEnabled = false;

        [SerializeField] private bool isFireEnableOnAwake = false;
        [SerializeField] private GameObject particles;

		public bool IsFireProcess => isFireProcess;
		private bool isFireProcess = false;

		private TimeSystem.TimeEvent timeEvent;
		private TimeSystem.Time fireDuration;
		private TimeSystem.Time oneSecond = new TimeSystem.Time() { TotalSeconds = 1 };

		private SignalBus signalBus;
        private TimeSystem.TimeSystem timeSystem;
		private IgnitionHandler ignitionHandler;

		[Inject]
        private void Construct(SignalBus signalBus, TimeSystem.TimeSystem timeSystem, IgnitionHandler ignitionHandler)
		{
            this.signalBus = signalBus;
            this.timeSystem = timeSystem;
			this.ignitionHandler = ignitionHandler;

			if (IsPlaced)
			{
				IsFireEnabled = isFireEnableOnAwake;
				isCompleted = true;
			}
			else
			{
				particles.SetActive(false);
			}

			TimeSystem.TimeEvent timeEvent = new TimeSystem.TimeEvent() { isInfinity = true, triggerTime = new TimeSystem.Time() { TotalSeconds = 1 }, onTrigger = FireTick };

			timeSystem.AddEvent(timeEvent);
		}

		private void OnDestroy()
		{
			
		}

		public override void Interact()
		{
			if (isFireEnabled)
			{
			}
			else
			{
				ignitionHandler.SetIgnition(this);
			}
		}

		public override void StartObserve()
		{
			if (IsPlaced)
			{
				var text = constructionData.GetLocalization(localization.CurrentLanguage);

				if (isFireProcess)
				{
					uiManager.Targets.ShowTargetInformation(text.constructionName, fireDuration.ToStringSimplification(showSecs: true));
				}
				else
				{
					uiManager.Targets.ShowTargetInformation(text.constructionName);
				}
			}
		}

		public override void Observe()
		{
			base.Observe();

			if (IsPlaced)
			{
				if (isFireProcess)
				{
					StartObserve();//update target information
				}
			}
		}

		public void StartFire(TimeSystem.Time fireDuration)
		{
			this.fireDuration = fireDuration;
			IsFireEnabled = true;
			isFireProcess = true;
		}

		private void FireTick()
		{
			if (isFireProcess)
			{
				fireDuration -= oneSecond;

				if(fireDuration.TotalSeconds == 0)
				{
					StopFire();
				}
			}
		}

		public void StopFire()
		{
			IsFireEnabled = false;
			isFireProcess = false;

			StartObserve();
		}
	}
}