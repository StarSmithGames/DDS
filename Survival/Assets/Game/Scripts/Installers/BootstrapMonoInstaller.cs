using UnityEngine;

using Zenject;

public class BootstrapMonoInstaller : MonoInstaller
{
	[SerializeField] private ProjectSettings projectSettings;
	[SerializeField] private InputData inputData;

	public override void InstallBindings()
	{
		SignalBusInstaller.Install(Container);

#if UNITY_EDITOR
		UnityEditor.EditorSettings.unityRemoteDevice = projectSettings.isMobile ? "Any Android Device" : "None";
#endif

		Container.Bind<ProjectSettings>().FromInstance(projectSettings);

		Container.Bind<InputData>().FromInstance(inputData).WhenInjectedInto<IInput>();//IInput bind in SceneMonoInstaller cuz require UIManager
	}
}

[System.Serializable]
public class ProjectSettings
{
	public bool isMobile = true;
}