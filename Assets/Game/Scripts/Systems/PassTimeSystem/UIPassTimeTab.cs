using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

public class UIPassTimeTab : MonoBehaviour
{
	public UnityAction<UIPassTimeTab> onPassTimeClicked;

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


	[Inject]
	private void Construct()
	{
		passTime.onClick.AddListener(OnPassTimeClicked);
	}

	private void OnDestroy()
	{
		passTime.onClick.RemoveAllListeners();
	}

	private void OnPassTimeClicked()
	{
		onPassTimeClicked?.Invoke(this);
	}
}