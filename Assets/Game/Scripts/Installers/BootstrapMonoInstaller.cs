using UnityEngine;

using Zenject;

public class BootstrapMonoInstaller : MonoInstaller
{
	[SerializeField] private GlobalSettings inputData;

	public override void InstallBindings()
	{
		SignalBusInstaller.Install(Container);

		Container.Bind<AsyncManager>().FromNewComponentOnNewGameObject().AsSingle();

		Container.Bind<GlobalSettings>().FromInstance(inputData);

#if UNITY_EDITOR
		UnityEditor.EditorSettings.unityRemoteDevice = inputData.projectSettings.isMobile ? "Any Android Device" : "None";
#endif

		if (inputData.projectSettings.isMobile)
		{
			Container.Bind<IInput>().To<MobileInput>().AsSingle();
		}
		else
		{
			Container.Bind<IInput>().To<KeyboardInput>().AsSingle();
		}
	}
}