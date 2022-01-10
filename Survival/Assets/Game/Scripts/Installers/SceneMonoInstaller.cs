using UnityEngine;
using Zenject;

public class SceneMonoInstaller : MonoInstaller
{
	[SerializeField] private UIManager uiManager;

	public override void InstallBindings()
	{
		Container.Bind<UIManager>().FromInstance(uiManager);

		Container.Install<PlayerInstaller>();

		BindInput();
	}

	private void BindInput()
	{
		ProjectSettings projectSettings = Container.Resolve<ProjectSettings>();

		if (projectSettings.isMobile)
		{
			Container.Bind<IInput>().To<MobileInput>().AsSingle();
		}
		else
		{
			Container.Bind<IInput>().To<KeyboardInput>().AsSingle();
		}
	}
}