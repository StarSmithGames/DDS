using UnityEngine;

using Zenject;

public class BootstrapMonoInstaller : MonoInstaller
{
	[SerializeField] private GlobalSettings inputData;

	public override void InstallBindings()
	{
		SignalBusInstaller.Install(Container);

		Container.Bind<AsyncManager>().FromNewComponentOnNewGameObject().AsSingle();

		Container.DeclareSignal<SignalInputClicked>();
		Container.DeclareSignal<SignalInputPressed>();
		Container.DeclareSignal<SignalInputUnPressed>();
		Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();

		Container.Bind<GlobalSettings>().FromInstance(inputData);

#if UNITY_EDITOR
		UnityEditor.EditorSettings.unityRemoteDevice = inputData.projectSettings.platform == PlatformType.Mobile ? "Any Android Device" : "None";
#endif

		if (inputData.projectSettings.platform == PlatformType.Mobile)
		{
			Container.Bind<IInput>().To<MobileInput>().AsSingle();
		}
		else
		{
			Container.Bind<IInput>().To<KeyboardInput>().AsSingle();
		}
	}
}