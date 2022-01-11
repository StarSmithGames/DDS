using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using static UnityEngine.GraphicsBuffer;

public class UIManager : MonoBehaviour
{
    public UITarget Target => target;
    [SerializeField] private UITarget target;

    [Space]
    [SerializeField] private UIControl control;
    public UIControl Control => control;

    [Space]
    [SerializeField] private List<WindowBase> windows = new List<WindowBase>();

    private void Awake()
	{
        Control.ButtonA.Hide();
        Control.PlayerInspect.Hide();

        Target.HideTarget();
        Target.HideFiller();

		for (int i = 0; i < windows.Count; i++)
		{
            Hide(windows[i]);
		}
    }

    public void Show<T>() where T : IWindow
	{
        Show(Get<T>());
    }
    public void Hide<T>() where T : IWindow
    {
        Hide(Get<T>());
    }

    public void Show(IWindow window)
    {
        (window as WindowBase).gameObject.SetActive(true);
    }
    public void Hide(IWindow window)
    {
        (window as WindowBase).gameObject.SetActive(false);
    }

    public IWindow Get<T>() where T : IWindow
	{
        return windows.Where((window) => window is T).FirstOrDefault();
    }
}

[System.Serializable]
public class UIControl
{
    public UIPlayerLook PlayerLook => playerLook;
    [SerializeField] private UIPlayerLook playerLook;

    public UIPlayerMove PlayerMove => playerMove;
    [SerializeField] private UIPlayerMove playerMove;

    public UIPlayerInspect PlayerInspect => playerInspect;
    [SerializeField] private UIPlayerInspect playerInspect;

    public PointerInteractableButton ButtonA => buttonA;
    [SerializeField] private PointerInteractableButton buttonA;

    [SerializeField] private PointerInteractableButton buttonB;
    [SerializeField] private PointerInteractableButton buttonY;
    [SerializeField] private PointerInteractableButton buttonX;

    public void EnableButtons()
	{
        buttonA.interactable = true;
    }
    public void DisableButtons()
	{
        buttonA.interactable = false;
    }
}
[System.Serializable]
public class UITarget
{
    [SerializeField] private Image target;
    [SerializeField] private Image filler;

    public void ShowTarget()
    {
        target.enabled = true;
    }
    public void HideTarget()
    {
        target.enabled = false;
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