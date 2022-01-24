using Game.Signals;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UIWindows : MonoBehaviour
{
    [SerializeField] private Button backButton;
    public List<WindowBase> Windows => windows;
    [SerializeField] private List<WindowBase> windows = new List<WindowBase>();

    private SignalBus signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
	{
        this.signalBus = signalBus;

        backButton?.onClick.AddListener(OnBack);
    }

    private void OnDestroy()
    {
        backButton?.onClick.RemoveAllListeners();
    }

    public bool IsContains<T>() where T : IWindow
    {
        return windows.OfType<T>().Any();
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

    public void HideAllWindows()
    {
        for (int i = 0; i < windows.Count; i++)
        {
            Hide(windows[i]);
        }
    }

    public IWindow Get<T>() where T : IWindow
    {
        return windows.Where((window) => window is T).FirstOrDefault();
    }

    public T GetAs<T>() where T : class, IWindow
    {
        return Get<T>() as T;
    }

    public bool IsAllHided()
	{
		for (int i = 0; i < windows.Count; i++)
		{
            if (windows[i].gameObject.activeSelf) return false;
		}
        return true;
	}



    private void OnBack()
    {
        signalBus?.Fire(new SignalUIWindowsBack() { root = this });
	}
}