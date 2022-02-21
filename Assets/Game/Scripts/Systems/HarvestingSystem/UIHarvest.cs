using Game.Systems.LocalizationSystem;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.HarvestingSystem
{
	public class UIHarvest : MonoBehaviour
	{
		public TMPro.TextMeshProUGUI InfoTitle => infoTitle;
		public TMPro.TextMeshProUGUI YieldTitle => yieldTitle;
		public TMPro.TextMeshProUGUI ToolTitle => toolTitle;

		[SerializeField] private TMPro.TextMeshProUGUI itemName;
		[Space]
		[SerializeField] private GameObject infoRoot;
		[SerializeField] private TMPro.TextMeshProUGUI infoTitle;
		[SerializeField] private TMPro.TextMeshProUGUI hoursLabel;
		[SerializeField] private TMPro.TextMeshProUGUI caloriesLabel;
		[Space]
		[SerializeField] private GameObject yieldsRoot;
		[SerializeField] private Transform yieldsContent;
		[SerializeField] private TMPro.TextMeshProUGUI yieldTitle;
		[Space]
		[SerializeField] private GameObject toolRoot;
		[SerializeField] private TMPro.TextMeshProUGUI toolTitle;
		[SerializeField] private Image toolIcon;
		[SerializeField] private Button prevTool;
		[SerializeField] private Button nextTool;

		private TimeSystem.Time time;
		private int calories = 2500;
		private List<UIYieldItem> yields = new List<UIYieldItem>();

		private SignalBus signalBus;
		private UIYieldItem.Factory yieldFactory;
		private LocalizationSystem.LocalizationSystem localizationSystem;

		[Inject]
		private void Construct(SignalBus signalBus, UIYieldItem.Factory yieldFactory)
		{
			this.signalBus = signalBus;
			this.yieldFactory = yieldFactory;

			if (yieldsContent.childCount > 0)
			{
				foreach (Transform child in yieldsContent)
				{
					Destroy(child.gameObject);
				}
			}
		}

		public void SetData(HarvestingConstruction construction)
		{
		}

		private void UpdateUI()
		{
			hoursLabel.text = time.ToStringSimplification();
			caloriesLabel.text = $"{calories} {localizationSystem._("")}";
		}
	}
}