using Game.Entities;
using Game.Signals;
using Game.Systems.IgnitionSystem;
using Game.Systems.InventorySystem;
using Game.Systems.InventorySystem.Inspector;
using Game.Systems.InventorySystem.Transactor;

using UnityEngine;
using Zenject;

namespace Game.Installers
{
	[CreateAssetMenu(fileName = "UIInstaller", menuName = "Installers/UIInstaller")]
	public class UIInstaller : ScriptableObjectInstaller<UIInstaller>
	{
		[SerializeField] private UIInventorySlot slotPrefab;
		[SerializeField] private int initialSlotFactorySize = 32;//5 * 4 + 3 * 4

		public override void InstallBindings()
		{
			Container.Bind<UIManager>().FromInstance(FindObjectOfType<UIManager>()).AsSingle();

			Container.DeclareSignal<SignalUIWindowsBack>();

			BindInspector();
			BindTransactor();
			BindBackpack();
			BindIgnition();

			Container.BindFactory<UIInventorySlot, UIInventorySlot.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(initialSlotFactorySize)
					.FromComponentInNewPrefab(slotPrefab));
		}

		private void BindInspector()
		{
			Container.DeclareSignal<SignalUIInspectorTake>();
			Container.DeclareSignal<SignalUIInspectorUse>();
			Container.DeclareSignal<SignalUIInspectorLeave>();

			Player p = Container.Resolve<Player>();
			Container.Bind<Transform>().FromInstance(p.ItemViewPoint).WhenInjectedInto<TransformTransition>();
			Container.Bind<TransformTransition>().AsSingle();

			Container.BindInterfacesAndSelfTo<InspectorHandler>().AsSingle();
		}

		private void BindTransactor()
		{
			Container.DeclareSignal<SignalUITransactorAll>();
			Container.DeclareSignal<SignalUITransactorGive>();
			Container.DeclareSignal<SignalUITransactorBack>();

			Container.BindInterfacesAndSelfTo<TransactorHandler>().AsSingle();
		}

		private void BindBackpack()
		{
			Container.DeclareSignal<SignalUIInventoryDrop>();
			Container.DeclareSignal<SignalUIInventorySlotClick>();

			Container.Bind<int>().FromInstance(initialSlotFactorySize).WhenInjectedInto<UIInventory>();

			Container.BindInterfacesAndSelfTo<BackpackHandler>().AsSingle();
		}

		private void BindIgnition()
		{
			Container.DeclareSignal<SignalUIIgnitionBack>();
			Container.DeclareSignal<SignalUIIgnitionStartFire>();
			Container.DeclareSignal<SignalUIIgnitionSlotItemChanged>();


			Container.BindInterfacesAndSelfTo<IgnitionHandler>().AsSingle();
		}
	}
}