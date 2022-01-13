using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private Button viewButton;
    [Space]
    [SerializeField] private UITargets targets;
    public UITargets Targets => targets;
    public UIControls Controls => controls;
    [SerializeField] private UIControls controls;

    public UIWindowsManager WindowsManager => windowsManager;
    [SerializeField] private UIWindowsManager windowsManager;

    private SignalBus signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
	{
        this.signalBus = signalBus;

        Controls.ButtonA.Hide();

        Targets.HideTarget();
        Targets.HideFiller();

        WindowsManager.HideAllWindows();

        menuButton.onClick.AddListener(OnMenuClicked);
        viewButton.onClick.AddListener(OnViewClicked);
    }

    private void OnMenuClicked()
	{

	}
    private void OnViewClicked()
    {
        WindowsManager.Show<UIPlayerInventoryWindow>();
    }
}