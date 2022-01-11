using UnityEngine;

using Zenject;

[CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Installers/PlayerInstaller")]
public class PlayerScriptableInstaller : ScriptableObjectInstaller
{
	public override void InstallBindings()
	{
		Container.Bind<Player>().FromInstance(FindObjectOfType<Player>()).AsSingle();
	}
}