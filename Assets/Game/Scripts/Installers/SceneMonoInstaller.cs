using UnityEngine;
using Zenject;

public class SceneMonoInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
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