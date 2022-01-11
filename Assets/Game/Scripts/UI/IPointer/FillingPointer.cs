using UnityEngine;
using UnityEngine.UI;

public class FillingPointer : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image filled;

    public void SetSprite(Sprite sprite)
	{
        icon.sprite = sprite;
    }

    public void SetFillAmount(float value)
	{
        filled.fillAmount = value;
    }
}