using Game.Systems.RadialMenu;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

public class UIManager : MonoBehaviour
{
    public UIRadialMenu RadialMenu => radialMenu;
    [SerializeField] private UIRadialMenu radialMenu;
    public UIRadialMenuButton RadialMenuButton => radialMenuButton;
    [SerializeField] private UIRadialMenuButton radialMenuButton;

    [Space]
    [SerializeField] private Button menuButton;
    public Button MenuButton => menuButton;
    
    public Button BackpackButton => backpackButton;
    [SerializeField] private Button backpackButton;
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
            radialMenuButton.gameObject.SetActive(false);
            backpackButton.gameObject.SetActive(false);
            Controls.PlayerLook.gameObject.SetActive(false);
            Controls.PlayerMove.gameObject.SetActive(false);
        }
    }
}