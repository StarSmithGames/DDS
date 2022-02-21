using Game.Systems.BuildingSystem;
using Game.Systems.LocalizationSystem;

using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.PassTimeSystem
{
	public class UIPassTimeWindow : WindowBase
	{
		public UnityAction onCanceled;

		public UIPassTimeTab CurrentTab {
			get
			{
				if(Type == PassTimeType.Both)
				{
					return sleepOption.isOn ? sleepTab : passTab;
				}
				else if (Type == PassTimeType.OnlySleep)
				{
					return sleepTab;
				}

				return passTab;
			}
		}

		public PassTimeType Type { get; private set; }

		public UIPassTimeTab SleepTab => sleepTab;
		public UIPassTimeTab PassTab => passTab;

		[SerializeField] private Toggle sleepOption;
		[SerializeField] private Toggle passOption;
		[Space]
		[SerializeField] private UIPassTimeTab sleepTab;
		[SerializeField] private UIPassTimeTab passTab;
		[Space]
		[SerializeField] private Button close;

		public bool IsEnable { get; private set; }

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
			Type = type;

			if (type == PassTimeType.Both)
			{
				sleepOption.gameObject.SetActive(true);
				passOption.gameObject.SetActive(true);
				OnSleepTabSelected(true);
				OnPassTabSelected(true);
			}
			else if (type == PassTimeType.OnlySleep)
			{
				sleepOption.gameObject.SetActive(false);
				passOption.gameObject.SetActive(false);
				OnSleepTabSelected(true);
				OnPassTabSelected(false);
			}
			else if(type == PassTimeType.OnlyPassTime)
			{
				sleepOption.gameObject.SetActive(false);
				passOption.gameObject.SetActive(false);
				OnSleepTabSelected(false);
				OnPassTabSelected(true);
			}
		}

		public void Enable(bool trigger)
		{
			sleepTab.Enable(trigger);
			passTab.Enable(trigger);

			sleepOption.interactable = trigger;
			passOption.interactable = trigger;

			close.interactable = trigger;

			IsEnable = trigger;
		}

		private void OnLocalizationChanged()
		{
			sleepTab.Title.text = localization._("ui.pass_time.sleep.title");
			sleepTab.Description.text = localization._("ui.pass_time.sleep.description");
			sleepTab.ButtonLabel.text = localization._("ui.pass_time.sleep.button");

			sleepTab.TimeLabel.text = localization._("ui.pass_time.time_label");
			sleepTab.CaloriesLabel.text = localization._("ui.pass_time.calories_label");
			sleepTab.CaloriesBurnedLabel.text = localization._("ui.pass_time.calories_burned_label");
			sleepTab.WarmthBonusLabel.text = localization._("ui.pass_time.warmth_bonus_label");

			passTab.Title.text = localization._("ui.pass_time.pass.title");
			passTab.Description.text = localization._("ui.pass_time.pass.description");
			passTab.ButtonLabel.text = localization._("ui.pass_time.pass.button");

			passTab.TimeLabel.text = localization._("ui.pass_time.time_label");
			passTab.CaloriesLabel.text = localization._("ui.pass_time.calories_label");
			passTab.CaloriesBurnedLabel.text = localization._("ui.pass_time.calories_burned_label");
			passTab.WarmthBonusLabel.text = localization._("ui.pass_time.warmth_bonus_label");
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