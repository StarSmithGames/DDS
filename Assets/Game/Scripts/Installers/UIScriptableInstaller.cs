using Game.Entities;
using Game.Signals;
using Game.Systems.InspectorSystem.Signals;
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
			BindBackpack();

			Container.BindFactory<UISlot, UISlot.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(initialSlotFactorySize)
					.FromComponentInNewPrefab(slotPrefab));
		}

		private void BindInspectorWindow()
		{
			Container.DeclareSignal<SignalUIInspectorTake>();
			Container.DeclareSignal<SignalUIInspectorUse>();
			Container.DeclareSignal<SignalUIInspectorLeave>();

			Player p = Container.Resolve<Player>();
			Container.Bind<Transform>().FromInstance(p.ItemViewPoint).WhenInjectedInto<TransformTransition>();
			Container.Bind<TransformTransition>().AsSingle();

			Container.BindInterfacesAndSelfTo<InspectorHandler>().AsSingle();
		}

		private void BindBackpack()
		{
			Container.DeclareSignal<SignalUIInventoryDrop>();
			Container.DeclareSignal<SignalUIInventorySlotClick>();

			Container.Bind<int>().FromInstance(initialSlotFactorySize).WhenInjectedInto<UIInventory>();

			Container.BindInterfacesAndSelfTo<BackpackHandler>().AsSingle();
		}
	}
}