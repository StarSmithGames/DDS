using UnityEngine;

public class UIControls : MonoBehaviour
{
    public UIPlayerLook PlayerLook => playerLook;
    [SerializeField] private UIPlayerLook playerLook;

    public UIPlayerMove PlayerMove => playerMove;
    [SerializeField] private UIPlayerMove playerMove;

    public UIInteractionButton ButtonA => buttonA;
    [SerializeField] private UIInteractionButton buttonA;

    [SerializeField] private UIInteractionButton buttonB;
    [SerializeField] private UIInteractionButton buttonY;
    [SerializeField] private UIInteractionButton buttonX;

    public void EnableButtons()
	{
        buttonA.interactable = true;
    }
    public void DisableButtons()
	{
        buttonA.interactable = false;
    }
}
