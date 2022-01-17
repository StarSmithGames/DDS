using UnityEngine;
using UnityEngine.UI;

public class UIAttribute : MonoBehaviour
{
	[SerializeField] private GameObject content;
	[SerializeField] private Image filler;
	[Space]
	[SerializeField] private Image fillerModifier;
	[SerializeField] private Color colorIncrease;
	[SerializeField] private Color colorDecrease;

	public void SetFillAmount(float value)
	{
		filler.fillAmount = value;
	}

	public void SetModifierFillAmount(float value)
	{
		if(value < 0)
		{
			fillerModifier.color = colorDecrease;
		}
		else
		{
			fillerModifier.color = colorIncrease;
		}

		fillerModifier.gameObject.SetActive(value != 0);
		fillerModifier.fillAmount = Mathf.Abs(value);
	}
}