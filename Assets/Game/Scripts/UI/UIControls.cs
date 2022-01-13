using UnityEngine;

public class UIControls : MonoBehaviour
{
    public UIPlayerLook PlayerLook => playerLook;
    [SerializeField] private UIPlayerLook playerLook;

    public UIPlayerMove PlayerMove => playerMove;
    [SerializeField] private UIPlayerMove playerMove;

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
