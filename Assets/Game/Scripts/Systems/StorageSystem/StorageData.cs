namespace Game.Systems.StorageSystem
{
	/// <summary>
	/// Часть хранимых данных.
	/// </summary>
	/// <typeparam name="T">Любой класс данных data</typeparam>
	public class StorageData<T> : IStorageData<T>
	{
		private Database database;
		private string key;

		public StorageData(Database database, string key)
		{
			this.database = database;
			this.key = key;
		}

		public string GetDataKey()
		{
			return key + "_key";
		}
		public T GetData()
		{
			return database.Get<T>(GetDataKey());
		}
		public void SetData(T data)
		{
			database.Set(GetDataKey(), data);
		}
	}
}