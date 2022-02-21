using Game.Systems.LocalizationSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.HarvestingSystem
{
	public class UIHarvestingWindow : WindowBase
	{
		public UnityAction onClosed;

		[SerializeField] private UIHarvest breakDown;
		[SerializeField] private UIHarvestCarcass carcass;
		[Space]
		[SerializeField] private Button close;

		private SignalBus signalBus;
		private UIManager uiManager;
		private LocalizationSystem.LocalizationSystem localizationSystem;
		
		[Inject]
		private void Construct(SignalBus signalBus, UIManager uiManager, LocalizationSystem.LocalizationSystem localizationSystem)
		{
			this.signalBus = signalBus;
			this.uiManager = uiManager;
			this.localizationSystem = localizationSystem;

			uiManager.WindowsManager.Register(this);

			OnLocalizationChanged();

			close.onClick.AddListener(OnClosed);

			signalBus?.Subscribe<SignalLocalizationChanged>(OnLocalizationChanged);
		}

		private void OnDestroy()
		{
			onClosed = null;

			close.onClick.RemoveAllListeners();

			signalBus?.Unsubscribe<SignalLocalizationChanged>(OnLocalizationChanged);
		}



		private void OnLocalizationChanged()
		{
			breakDown.InfoTitle.text = localizationSystem._("ui.harvesting.break_down.info_title");

			breakDown.YieldTitle.text = localizationSystem._("ui.harvesting.yield_title");
			breakDown.ToolTitle.text = localizationSystem._("ui.harvesting.tool_title");

			carcass.InfoTitle.text = localizationSystem._("ui.harvesting.carcass_harvesting.info_title");

			carcass.ToolTitle.text = localizationSystem._("ui.harvesting.tool_title");
		}

		private void OnClosed()
		{
			onClosed?.Invoke();
		}
	}
}