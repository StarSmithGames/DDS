using UnityEngine;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class UIPlayerInventoryWindow : WindowBase
	{
		public UIInventory Inventory => inventory;
		[SerializeField] private UIInventory inventory;

		public UIInventory Container => container;
		[SerializeField] private UIInventory container;

		public UIItemViewer ItemViewer => itemViewer;
		[SerializeField] private UIItemViewer itemViewer;

		private SignalBus signalBus;

		[Inject]
		private void Construct(SignalBus signalBus)
		{
			this.signalBus = signalBus;
		}
	}
}