using Game.Systems.BuildingSystem;
using Game.Systems.LocalizationSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.PassTimeSystem
{
	public class UIPassTimeWindow : ModalWindow
	{
		public UnityAction onCanceled;

		public bool IsSleepTab => sleepOption.isOn;

		public UIPassTimeTab SleepTab => sleepTab;
		public UIPassTimeTab PassTab => passTab;

		[SerializeField] private Toggle sleepOption;
		[SerializeField] private Toggle passOption;
		[Space]
		[SerializeField] private UIPassTimeTab sleepTab;
		[SerializeField] private UIPassTimeTab passTab;
		[Space]
		[SerializeField] private Button close;

		private SignalBus signalBus;
		private LocalizationSystem.LocalizationSystem localization;

		[Inject]
		private void Construct(SignalBus signalBus, UIManager uiManager, LocalizationSystem.LocalizationSystem localization)
		{
			this.signalBus = signalBus;
			this.localization = localization;

			uiManager.WindowsManager.Register(this);

			OnSleepTabSelected(true);

			sleepOption.onValueChanged.AddListener(OnSleepTabSelected);
			passOption.onValueChanged.AddListener(OnPassTabSelected);

			close.onClick.AddListener(OnClosed);

			OnLocalizationChanged();

			signalBus?.Subscribe<SignalLocalizationChanged>(OnLocalizationChanged);
		}

		private void OnDestroy()
		{
			sleepOption.onValueChanged.RemoveAllListeners();
			passOption.onValueChanged.RemoveAllListeners();

			onCanceled = null;
			close.onClick.RemoveAllListeners();

			signalBus?.Unsubscribe<SignalLocalizationChanged>(OnLocalizationChanged);
		}

		public void SetData(float warmthBonus)
		{
		}

		public void SetType(PassTimeType type)
		{
			if(type == PassTimeType.None)
			{
				sleepOption.gameObject.SetActive(true);
				OnSleepTabSelected(true);
			}
			else if(type == PassTimeType.OnlyPassTime)
			{
				OnPassTabSelected(true);
				sleepOption.gameObject.SetActive(false);
			}
			else if(type == PassTimeType.FirstPassTime) 
			{
				sleepOption.gameObject.SetActive(true);
				OnPassTabSelected(true);
			}
		}

		public void Enable(bool trigger)
		{
			sleepTab.Enable(trigger);
			passTab.Enable(trigger);

			sleepOption.interactable = trigger;
			passOption.interactable = trigger;
		}

		private void OnLocalizationChanged()
		{
			sleepTab.Title.text = localization._("ui.passtime.sleep.title");
			sleepTab.Description.text = localization._("ui.passtime.sleep.description");
			sleepTab.ButtonLabel.text = localization._("ui.passtime.sleep.button");

			sleepTab.TimeLabel.text = localization._("ui.passtime.timeLabel");
			sleepTab.CaloriesLabel.text = localization._("ui.passtime.caloriesLabel");
			sleepTab.CaloriesBurnedLabel.text = localization._("ui.passtime.caloriesBurnedLabel");
			sleepTab.WarmthBonusLabel.text = localization._("ui.passtime.warmthBonusLabel");

			passTab.Title.text = localization._("ui.passtime.passtime.title");
			passTab.Description.text = localization._("ui.passtime.passtime.description");
			passTab.ButtonLabel.text = localization._("ui.passtime.passtime.button");

			passTab.TimeLabel.text = localization._("ui.passtime.timeLabel");
			passTab.CaloriesLabel.text = localization._("ui.passtime.caloriesLabel");
			passTab.CaloriesBurnedLabel.text = localization._("ui.passtime.caloriesBurnedLabel");
			passTab.WarmthBonusLabel.text = localization._("ui.passtime.warmthBonusLabel");
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