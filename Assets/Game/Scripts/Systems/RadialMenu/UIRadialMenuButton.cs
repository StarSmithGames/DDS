using UnityEngine;
using UnityEngine.UI;

using Zenject;
namespace Game.Systems.RadialMenu
{
	public class UIRadialMenuButton : MonoBehaviour
	{
		[SerializeField] private Button button;
		[SerializeField] private Image icon;
		[SerializeField] private Image iconClose;

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

		public void SetIcon(bool trigger)
		{
			icon.enabled = trigger;
			iconClose.enabled = !trigger;
		}

		private void OnButtonClicked()
		{
			signalBus?.Fire(new SignalRadialMenuButton());
		}
	}
}