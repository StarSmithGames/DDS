using Game.Signals;

using System.Linq;

using UnityEngine;

using Zenject;

public class UIWindowsManager : MonoBehaviour
{
    public UIWindows WorldWindows => worldWindows;
    [SerializeField] private UIWindows worldWindows;
    public UIWindows BackpackWindows => worldWindows;
    [SerializeField] private UIWindows backpackWindows;

    private SignalBus signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
	{
        this.signalBus = signalBus;

        signalBus?.Subscribe<SignalUIWindowsBack>(OnWindowsBack);
    }

	private void OnDestroy()
	{
        signalBus?.Unsubscribe<SignalUIWindowsBack>(OnWindowsBack);
    }
    public bool IsAnyWindowShowing()
    {
        return WorldWindows.IsAnyShowing() || BackpackWindows.IsAnyShowing();
    }
    public bool IsAllHided()
	{
        return WorldWindows.IsAllHided() || BackpackWindows.IsAllHided();
    }

    public void Show<T>() where T : IWindow
    {
        UIWindows root = Check<T>();
        root.Show<T>();

        root.gameObject.SetActive(true);
    }
    public void Hide<T>() where T : IWindow
    {
        var w = Check<T>();
        w.Hide<T>();

		if (w.IsAllHided())
		{
            w.gameObject.SetActive(false);
        }
    }

    public T GetAs<T>() where T : class, IWindow
    {
        return Check<T>().GetAs<T>();
    }

    public void ShowWorld<T>() where T : IWindow
    {
        worldWindows.Show<T>();

        if (!worldWindows.gameObject.activeSelf)
        {
            worldWindows.gameObject.SetActive(true);
        }
    }

    public void ShowBackpack<T>() where T : IWindow
    {
        backpackWindows.Show<T>();

        if (!backpackWindows.gameObject.activeSelf)
        {
            backpackWindows.gameObject.SetActive(true);
        }
    }

    public void HideAllWindows()
    {
        WorldWindows.HideAllWindows();
        BackpackWindows.HideAllWindows();
    }

    private UIWindows Check<T>() where T : IWindow
    {
        if (worldWindows.IsContains<T>()) return worldWindows;
        if (backpackWindows.IsContains<T>()) return backpackWindows;

        throw new System.Exception("UIWINDOWS CHEK ERROR");
	}

    private void OnWindowsBack(SignalUIWindowsBack signal)
	{
        signal.root.HideAllWindows();
        signal.root.gameObject.SetActive(false);
    }
}