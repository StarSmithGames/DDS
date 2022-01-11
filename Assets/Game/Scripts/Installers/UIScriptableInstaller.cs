using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "UIInstaller", menuName = "Installers/UIInstaller")]
public class UIScriptableInstaller : ScriptableObjectInstaller
{
	public override void InstallBindings()
	{
		Container.Bind<UIManager>().FromInstance(FindObjectOfType<UIManager>()).AsSingle();

		BindInspectorWindow();
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
}