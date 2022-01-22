using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.BuildingSystem
{
	public class UIBuildingWindow : WindowBase
	{
		[SerializeField] private Button cancel;
		[SerializeField] private Button place;

		private SignalBus signalBus;

		[Inject]
		private void Construct(SignalBus signalBus)
		{
			this.signalBus = signalBus;

			cancel.onClick.AddListener(OnCancelClicked);
			place.onClick.AddListener(OnPlaceClicked);
		}

		private void OnDestroy()
		{
			cancel.onClick.RemoveAllListeners();
			place.onClick.RemoveAllListeners();
		}

		private void OnCancelClicked()
		{
			signalBus?.Fire(new SignalBuildingCancel());
		}
		private void OnPlaceClicked()
		{
			signalBus?.Fire(new SignalBuildingBuild());
		}
	}
}