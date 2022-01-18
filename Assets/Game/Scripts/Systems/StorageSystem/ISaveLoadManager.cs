namespace Game.Systems.StorageSystem
{
	public interface ISaveLoadManager
	{
		// Start is called before the first f// save storage
		void Save();

		// Get currently selected storage
		Storage GetStorage();
	}
}