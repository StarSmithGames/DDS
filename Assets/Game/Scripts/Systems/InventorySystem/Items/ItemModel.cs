using Game.Systems.InventorySystem.Inspector;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

namespace Game.Systems.InventorySystem
{
	/// <summary>
	/// Физическое представление Item.
	/// </summary>
	public class ItemModel : PoolableObject, IEntity, IInteractable
	{
		[SerializeField] private Item item;
		public Item Item => item;

		[SerializeField] private Collider coll;

		private InspectorHandler itemInspector;
		private UIManager uiManager;
		private LocalizationSystem.LocalizationSystem localization;

		[Inject]
		private void Construct(InspectorHandler itemInspector, UIManager uiManager, LocalizationSystem.LocalizationSystem localization)
		{
			this.itemInspector = itemInspector;
			this.uiManager = uiManager;
			this.localization = localization;
		}

		public void Interact()
		{
			itemInspector.SetItem(this);
		}

		public void Enable(bool trigger)
		{
			if (coll == null) coll = GetComponent<Collider>();
			coll.enabled = trigger;
		}

		public void StartObserve()
		{
			uiManager.Targets.ShowTargetInformation(item.ItemData.ItemName);
		}
		public void Observe()
		{
		}
		public void EndObserve()
		{
			uiManager.Targets.HideTargetInformation();
		}

		[Button]
		private void SavePositionAndRoation()
		{
			//Item.ItemData.prefabPossitionOffsetView = transform.position;
		}
	}
}