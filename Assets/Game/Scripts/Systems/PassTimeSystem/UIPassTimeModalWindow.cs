using Game.Systems.BuildingSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.PassTimeSystem
{
	public class UIPassTimeModalWindow : ModalWindow
	{
		public UnityAction onCanceled;

		[SerializeField] private Toggle sleepOption;
		[SerializeField] private Toggle passOption;
		[Space]
		[SerializeField] private UIPassTimeTab sleepTab;
		[SerializeField] private UIPassTimeTab passTab;
		[Space]
		[SerializeField] private Button close;

		[Inject]
		private void Construct(UIManager uiManager)
		{
			uiManager.WindowsManager.Register(this);

			OnSleepTabSelected(true);

			sleepOption.onValueChanged.AddListener(OnSleepTabSelected);
			passOption.onValueChanged.AddListener(OnPassTabSelected);

			close.onClick.AddListener(OnClosed);
		}

		private void OnDestroy()
		{
			sleepOption.onValueChanged.RemoveAllListeners();
			passOption.onValueChanged.RemoveAllListeners();

			onCanceled = null;
			close.onClick.RemoveAllListeners();
		}

		public void SetData(float warmthBonus)
		{

		}

		private void OnSleepTabSelected(bool value)
		{
			sleepTab.gameObject.SetActive(value);
			passTab.gameObject.SetActive(!value);
		}

		private void OnPassTabSelected(bool value)
		{
			sleepTab.gameObject.SetActive(!value);
			passTab.gameObject.SetActive(value);
		}

		private void OnClosed()
		{
			onCanceled?.Invoke();
		}
	}
}