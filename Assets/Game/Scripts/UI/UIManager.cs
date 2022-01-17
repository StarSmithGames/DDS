using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UIManager : MonoBehaviour
{
    public Button MenuButton => menuButton;
    [SerializeField] private Button menuButton;
    public Button ViewButton => viewButton;
    [SerializeField] private Button viewButton;
    [Space]
    [SerializeField] private UITargets targets;
    public UITargets Targets => targets;
    public UIControls Controls => controls;
    [SerializeField] private UIControls controls;

    public UIStatus Status => status;
    [SerializeField] private UIStatus status;

    public UIWindowsManager WindowsManager => windowsManager;
    [SerializeField] private UIWindowsManager windowsManager;

    private SignalBus signalBus;

    [Inject]
    private void Construct(SignalBus signalBus, GlobalSettings globalSettings)
	{
        this.signalBus = signalBus;

        Controls.ButtonA.Hide();

        Targets.HideAll();

        WindowsManager.HideAllWindows();

        if(globalSettings.projectSettings.platform == PlatformType.Desktop)
		{
            menuButton.gameObject.SetActive(false);
            viewButton.gameObject.SetActive(false);
        }
    }
}