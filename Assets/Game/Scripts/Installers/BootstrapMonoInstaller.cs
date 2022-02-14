using Game.Managers.InputManger;

using UnityEngine;

using Zenject;

public class BootstrapMonoInstaller : MonoInstaller
{
	[SerializeField] private GlobalSettings inputData;

	public override void InstallBindings()
	{
		SignalBusInstaller.Install(Container);

		Container.Bind<AsyncManager>().FromNewComponentOnNewGameObject().AsSingle();

		BindInputManager();

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

	private void BindInputManager()
	{
		Container.DeclareSignal<SignalInputPressed>();
		Container.DeclareSignal<SignalInputUnPressed>();

		Container.DeclareSignal<SignalInputKeyUp>();
		Container.DeclareSignal<SignalInputKeyDown>();

		Container.DeclareSignal<SignalInputUp>();
		Container.DeclareSignal<SignalInputDown>();

		Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
	}
}