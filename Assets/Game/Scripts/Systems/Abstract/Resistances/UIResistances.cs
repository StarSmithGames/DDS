using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResistances : MonoBehaviour
{
	public TMPro.TextMeshProUGUI FeelsLike => feelsLike;
	public TMPro.TextMeshProUGUI AirFeels => airFeels;
	public TMPro.TextMeshProUGUI WindFeels => windFeels;
	public TMPro.TextMeshProUGUI BonusesFeels => bonusesFeels;
	public RawImage WindDirection => windDirection;

	[SerializeField] private TMPro.TextMeshProUGUI feelsLike;
	[SerializeField] private TMPro.TextMeshProUGUI airFeels;
	[SerializeField] private TMPro.TextMeshProUGUI windFeels;
	[SerializeField] private TMPro.TextMeshProUGUI bonusesFeels;
	[SerializeField] private RawImage windDirection;
}