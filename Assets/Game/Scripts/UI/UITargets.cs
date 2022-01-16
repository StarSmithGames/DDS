﻿using UnityEngine;
using UnityEngine.UI;

public class UITargets : MonoBehaviour
{
    [SerializeField] private Image target;
    [SerializeField] private Image filler;
    [SerializeField] private UITargetInformation targetInformation;

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

    public void ShowFiller()
    {
        SetFiller(0);
        filler.enabled = true;
    }
    public void SetFiller(float value)
    {
        filler.fillAmount = value;
    }
    public void HideFiller()
    {
        filler.enabled = false;
    }

    
}