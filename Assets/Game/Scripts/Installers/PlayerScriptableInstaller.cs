using UnityEngine;

using Zenject;

[CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Installers/PlayerInstaller")]
public class PlayerScriptableInstaller : ScriptableObjectInstaller
{
	public override void InstallBindings()
	{
		Player p = FindObjectOfType<Player>();
		Camera c = p.GetComponentInChildren<Camera>();

		Container.Bind<Camera>().FromInstance(c).AsSingle();
		Container.Bind<Player>().FromInstance(p).AsSingle();
	}
}