using Game.Systems.IgnitionSystem;

using Newtonsoft.Json.Linq;

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

		public bool IsFireEnabled => isFireEnabled;
		private bool isFireEnabled = false;

        [SerializeField] private bool isFireEnableOnAwake = false;
        [SerializeField] private GameObject particles;

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
				isFireEnabled = isFireEnableOnAwake;
				isCompleted = true;

				particles.SetActive(isFireEnabled);
			}
			else
			{
				particles.SetActive(false);
			}
		}

		private void OnDestroy()
		{
			
		}

		public override void Interact()
		{
			if (isFireEnabled)
			{
				Debug.LogError("Window fire cooking");
			}
			else
			{
				Debug.LogError("Window firing");
				ignitionHandler.SetIgnition(this);
			}
		}
	}
}