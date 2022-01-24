using Game.Systems.IgnitionSystem;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Systems.BuildingSystem
{
    public class FireConstruction : ConstructionModel
    {
		public bool IsEnabled => isEnabled;
		private bool isEnabled = false;

        [SerializeField] private bool isEnableOnAwake = false;
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

			isEnabled = isEnableOnAwake;
			particles.SetActive(isEnabled);

        }

		private void OnDestroy()
		{
			
		}

		public override void Interact()
		{
			if (isEnabled)
			{
				Debug.LogError("Window fire cooking");
			}
			else
			{
				ignitionHandler.SetIgnition(this);
			}
		}
	}
}