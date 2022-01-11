using UnityEngine;

using Zenject;

public class BootstrapMonoInstaller : MonoInstaller
{
	[SerializeField] private ProjectSettings projectSettings;
	[SerializeField] private InputSettings inputData;

	public override void InstallBindings()
	{
		SignalBusInstaller.Install(Container);

		Container.Bind<AsyncManager>().FromNewComponentOnNewGameObject().AsSingle();

#if UNITY_EDITOR
		UnityEditor.EditorSettings.unityRemoteDevice = projectSettings.isMobile ? "Any Android Device" : "None";
#endif

		Container.Bind<ProjectSettings>().FromInstance(projectSettings);

		Container.Bind<InputSettings>().FromInstance(inputData);
	}
}

[System.Serializable]
public class ProjectSettings
{
	public bool isMobile = true;
}