using UnityEngine;
using UnityEngine.UI;

public class UITargets : MonoBehaviour
{

    public UITargetInformation TargetInformation => targetInformation;

    [SerializeField] private Image target;
    public UITargetFiller Filler => filler;
    [SerializeField] private UITargetFiller filler;
    [SerializeField] private UITargetInformation targetInformation;

    public void HideAll()
	{
        HideTarget();
        Filler.HideFiller();
        HideTargetInformation();
    }

    public void ShowTarget()
    {
        target.enabled = true;
    }
    public void HideTarget()
    {
        target.enabled = false;
    }

    public void ShowTargetInformation(string name, string information = "")
    {
        targetInformation.SetInformation(name, information);
        targetInformation.gameObject.SetActive(true);
    }
    public void HideTargetInformation()
	{
        targetInformation.gameObject.SetActive(false);
    }
}