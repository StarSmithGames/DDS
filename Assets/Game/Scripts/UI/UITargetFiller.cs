using UnityEngine;
using UnityEngine.UI;

public class UITargetFiller : MonoBehaviour
{
	[SerializeField] private Image filler;

    public void SetFiller(float value)
    {
        filler.fillAmount = value;
    }

    public void ShowFiller()
    {
        SetFiller(0);
        gameObject.SetActive(true);
    }
    public void HideFiller()
    {
        gameObject.SetActive(false);
    }
}