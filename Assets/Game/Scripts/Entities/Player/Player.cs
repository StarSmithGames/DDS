using Game.Systems.InventorySystem;

using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class Player : MonoBehaviour, IEntity
	{
		[SerializeField] private PlayerController playerController;
		[SerializeField] private CameraController cameraController;
		[SerializeField] private CameraVision cameraVision;
		public Transform ItemViewPoint => itemViewPoint;
		[SerializeField] private Transform itemViewPoint;

		public IStatus Status { get; private set; }

		private UIManager uiManager;

		[Inject]
		private void Construct(UIManager uiManager, IStatus status)
		{
			this.uiManager = uiManager;

			Debug.LogError(status != null);
			Status = status;
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
			cameraVision.UnPauseVision();
		}

		public void DisableVision()
		{
			cameraVision.PauseVision();
		}

		public void StartObserve()
		{
			throw new System.NotImplementedException();
		}

		public void Observe()
		{
			throw new System.NotImplementedException();
		}

		public void EndObserve()
		{
			throw new System.NotImplementedException();
		}
	}
	[System.Serializable]
	public class PlayerSettings
	{
		public StatsSettings stats;
		public ResistancesSettings resistances;
		public InventorySettings inventory;
	}
}