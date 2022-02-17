using Game.Systems.EnvironmentSystem;
using Game.Systems.IgnitionSystem;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

using static UnityEngine.EventSystems.EventTrigger;

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

        [SerializeField] private bool cheatFireEnableOnAwake = false;
        [SerializeField] private GameObject particles;
		[SerializeField] private EnvironmentArea coverageArea;

		public bool IsFireProcess { get; set; }

		private bool isObserveIt = false;

		private TimeSystem.Time fireDuration;
		private TimeSystem.Time oneSecond = new TimeSystem.Time() { TotalSeconds = 1 };

		private Modifier fireModifier = new Modifier(5);
		private List<IEntity> entitiesInArea = new List<IEntity>();

		private SignalBus signalBus;
		private IgnitionHandler ignitionHandler;

		[Inject]
        private void Construct(SignalBus signalBus, TimeSystem.TimeSystem timeSystem, IgnitionHandler ignitionHandler)
		{
            this.signalBus = signalBus;
			this.ignitionHandler = ignitionHandler;

			if (IsPlaced)
			{
				if (cheatFireEnableOnAwake)
				{
					StartFire();
				}
				else
				{
					particles.SetActive(false);
				}
				isCompleted = true;
			}
			else
			{
				particles.SetActive(false);
			}

			TimeSystem.TimeEvent timeEvent = new TimeSystem.TimeEvent() { isInfinity = true, triggerTime = new TimeSystem.Time() { TotalSeconds = 1 }, onTrigger = FireTick };

			timeSystem.AddEvent(timeEvent);
		}

		private void OnDestroy() { }

		public override void Interact()
		{
			if (IsFireProcess)
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

				if (IsFireProcess)
				{
					uiManager.Targets.ShowTargetInformation(text.constructionName, fireDuration.ToStringSimplification(showSecs: true, cheatFireEnableOnAwake));
				}
				else
				{
					uiManager.Targets.ShowTargetInformation(text.constructionName);
				}
			}

			isObserveIt = true;
		}

		public override void Observe()
		{
			base.Observe();

			if (IsFireProcess)
			{
				StartObserve();
			}

			isObserveIt = true;
		}
		public override void EndObserve()
		{
			base.EndObserve();

			isObserveIt = false;
		}

		public void StartFire(TimeSystem.Time fireDuration)
		{
			this.fireDuration = fireDuration;
			StartFire();
		}
		private void StartFire()
		{
			coverageArea.StartCoverage();
			IsFireProcess = true;
			particles.SetActive(true);
		}
		private void FireTick()
		{
			if (IsFireProcess)
			{
				UpdateEntities();

				if (!cheatFireEnableOnAwake)
				{
					fireDuration -= oneSecond;

					if (fireDuration.TotalSeconds == 0)
					{
						StopFire();
					}
				}
			}
		}
		public void StopFire()
		{
			IsFireProcess = false;
			coverageArea.StopCoverage();
			particles.SetActive(false);

			UpdateEntities();


			if (isObserveIt)
			{
				StartObserve();
			}
			else
			{
				EndObserve();
			}
		}

		private void UpdateEntities()
		{
			if (IsFireProcess)
			{
				var coverageEntities = coverageArea.Entities;
				if (coverageEntities != null)
				{
					for (int i = entitiesInArea.Count - 1; i >= 0; i--)
					{
						if (!coverageEntities.Contains(entitiesInArea[i]))//если у меня есть, а у убласти нету, то удаляем
						{
							if (entitiesInArea[i] is IPlayer player)
							{
								player.Status.Resistances.RemoveModifier(fireModifier);
								entitiesInArea.Remove(player);
							}
						}
					}

					for (int i = 0; i < coverageEntities.Length; i++)
					{
						if (!entitiesInArea.Contains(coverageEntities[i]))//если у меня нету, а у области есть, то добавляем
						{
							if (coverageEntities[i] is IPlayer player)
							{
								entitiesInArea.Add(player);
								player.Status.Resistances.AddModifier(fireModifier);
							}
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < entitiesInArea.Count; i++)
				{
					if (entitiesInArea[i] is IPlayer player)
					{
						player.Status.Resistances.RemoveModifier(fireModifier);
					}
				}
				entitiesInArea.Clear();
			}
		}
	}
}