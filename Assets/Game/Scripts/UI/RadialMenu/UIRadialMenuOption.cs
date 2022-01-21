using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.RadialMenu
{
	public class UIRadialMenuOption : PoolableObject
	{
		[SerializeField] private Button button;

		public float Rotation { get; set; }

		private SignalBus signalBus;

		[Inject]
		private void Construct(SignalBus signalBus)
		{
			this.signalBus = signalBus;

			button.onClick.AddListener(OnButtonClicked);
		}

		private void OnDestroy()
		{
			button.onClick.RemoveAllListeners();
		}

		private void OnButtonClicked()
		{
			signalBus?.Fire(new SignalRadialMenuOptionChanged() { option = this });
		}


		public class Factory : PlaceholderFactory<UIRadialMenuOption> { }
	}
}