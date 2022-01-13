using Game.Entities;
using Game.Signals;
using Game.Systems.InventorySystem.Signals;

using UnityEngine;
using Zenject;

namespace Game.Installers
{
	[CreateAssetMenu(fileName = "UIInstaller", menuName = "Installers/UIInstaller")]
	public class UIScriptableInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private UISlot slotPrefab;
		[SerializeField] private int initialSlotFactorySize = 20;//5*4

		public override void InstallBindings()
		{
			Container.Bind<UIManager>().FromInstance(FindObjectOfType<UIManager>()).AsSingle();

			Container.DeclareSignal<SignalUIWindowsBack>();

			BindInspectorWindow();
			BindContainerInventoryWindow();
			BindPlayerContainerWindow();

			Container.DeclareSignal<SignalUISlotClick>();

			Container.BindFactory<UISlot, UISlot.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(initialSlotFactorySize)
					.FromComponentInNewPrefab(slotPrefab));
		}

		private void BindInspectorWindow()
		{
			Container.DeclareSignal<SignalUITakeItem>();
			Container.DeclareSignal<SignalUIDropItem>();

			Player p = Container.Resolve<Player>();
			Container.Bind<Transform>().FromInstance(p.ItemViewPoint).WhenInjectedInto<ItemViewer>();
			Container.Bind<ItemViewer>().AsSingle();

			Container.BindInterfacesAndSelfTo<ItemInspectorHandler>().AsSingle();
		}

		private void BindPlayerContainerWindow()
		{
			Container.Bind<int>().FromInstance(initialSlotFactorySize).WhenInjectedInto<UIInventory>();

			Container.BindInterfacesAndSelfTo<PlayerInventoryHandler>().AsSingle();
		}

		private void BindContainerInventoryWindow()
		{
			Container.BindInterfacesAndSelfTo<ContainerInventoryHandler>().AsSingle();
		}
	}
}