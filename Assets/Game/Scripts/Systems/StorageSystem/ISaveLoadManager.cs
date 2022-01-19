namespace Game.Systems.StorageSystem
{
	public interface ISaveLoadManager
	{
		void Save();

		// Get currently selected storage
		Storage GetStorage();
	}
}