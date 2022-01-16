using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class Player : MonoBehaviour
	{
		[SerializeField] private PlayerController playerController;
		[SerializeField] private CameraController cameraController;
		[SerializeField] private CameraVision cameraVision;

		public Transform ItemViewPoint => itemViewPoint;
		[SerializeField] private Transform itemViewPoint;

		public IInventory Inventory { get; private set; }

		private UIManager uiManager;

		[Inject]
		private void Construct(UIManager uiManager, PlayerSettings settings)
		{
			this.uiManager = uiManager;

			Inventory = new Inventory(settings.inventory);
		}

		public void Freeze()
		{
			playerController.IsJumpLocked = true;
			playerController.IsMoveLocked = true;
			cameraController.IsLookLocked = true;

			uiManager.Controls.PlayerLook.Hide();
			uiManager.Controls.PlayerMove.Hide();
		}
		public void UnFreeze()
		{
			playerController.IsJumpLocked = false;
			playerController.IsMoveLocked = false;
			cameraController.IsLookLocked = false;

			uiManager.Controls.PlayerLook.Show();
			uiManager.Controls.PlayerMove.Show();
		}

		public void EnableVision()
		{
			cameraVision.PauseVision();
		}

		public void DisableVision()
		{
			cameraVision.UnPauseVision();
		}
	}
	[System.Serializable]
	public class PlayerSettings
	{
		public InventorySettings inventory;
	}
}