using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.PassTimeSystem
{
	public class UIPassTimeTab : MonoBehaviour
	{
		public UnityAction onPassTimeClicked;

		public TMPro.TextMeshProUGUI Title => title;
		public TMPro.TextMeshProUGUI Description => description;
		public TMPro.TextMeshProUGUI TimeLabel => timeLabel;
		public TMPro.TextMeshProUGUI CaloriesLabel => caloriesLabel;
		public TMPro.TextMeshProUGUI CaloriesBurnedLabel => caloriesBurnedLabel;
		public TMPro.TextMeshProUGUI WarmthBonusLabel => warmthBonusLabel;
		public TMPro.TextMeshProUGUI ButtonLabel => buttonLabel;

		[SerializeField] private TMPro.TextMeshProUGUI title;
		[SerializeField] private TMPro.TextMeshProUGUI description;
		[Space]
		[SerializeField] private TMPro.TextMeshProUGUI time;
		[SerializeField] private TMPro.TextMeshProUGUI timeLabel;
		[SerializeField] private Button left;
		[SerializeField] private Button right;
		[Space]
		[SerializeField] private TMPro.TextMeshProUGUI caloriesLabel;
		[SerializeField] private TMPro.TextMeshProUGUI calodies;
		[Space]
		[SerializeField] private TMPro.TextMeshProUGUI caloriesBurnedLabel;
		[SerializeField] private TMPro.TextMeshProUGUI caloriesBurned;
		[Space]
		[SerializeField] private TMPro.TextMeshProUGUI warmthBonusLabel;
		[SerializeField] private TMPro.TextMeshProUGUI warmthBonus;
		[Space]
		[SerializeField] private Button passTime;
		[SerializeField] private TMPro.TextMeshProUGUI buttonLabel;

		public int Hours => hours;
		private int hours = 1;

		[Inject]
		private void Construct()
		{
			left.onClick.AddListener(OnLeftClicked);
			right.onClick.AddListener(OnRightClicked);

			passTime.onClick.AddListener(OnPassTimeClicked);
		}

		private void OnDestroy()
		{
			passTime.onClick.RemoveAllListeners();

			left.onClick.RemoveAllListeners();
			right.onClick.RemoveAllListeners();

			onPassTimeClicked = null;
		}

		public void SetData()
		{
			UpdateUI();
		}

		public void Enable(bool trigger)
		{
			left.interactable = trigger;
			right.interactable = trigger;
			passTime.interactable = trigger;
		}

		private void UpdateUI()
		{
			time.text = hours.ToString();
		}

		private void OnLeftClicked()
		{
			if(hours - 1 >= 1)
			{
				hours -= 1;

				UpdateUI();
			}
		}

		private void OnRightClicked()
		{
			if(hours + 1 <= 24)
			{
				hours += 1;

				UpdateUI();
			}
		}

		private void OnPassTimeClicked()
		{
			onPassTimeClicked?.Invoke();
		}
	}
}