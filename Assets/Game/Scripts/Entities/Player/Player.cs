using UnityEngine;

using Zenject;

public class Player : MonoBehaviour
{
	[SerializeField] private PlayerController playerController;
	[SerializeField] private CameraController cameraController;
	
	public Transform ItemViewPoint => itemViewPoint;
	[SerializeField] private Transform itemViewPoint;

	private UIManager uiManager;

	[Inject]
	private void Construct(UIManager uiManager)
	{
		this.uiManager = uiManager;
	}

	public void Freeze()
	{
		playerController.IsJumpLocked = true;
		playerController.IsMoveLocked = true;
		cameraController.IsLookLocked = true;

		uiManager.Control.PlayerLook.Hide();
		uiManager.Control.PlayerMove.Hide();
	}
	public void UnFreeze()
	{
		playerController.IsJumpLocked = false;
		playerController.IsMoveLocked = false;
		cameraController.IsLookLocked = false;

		uiManager.Control.PlayerLook.Show();
		uiManager.Control.PlayerMove.Show();
	}
}