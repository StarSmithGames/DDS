namespace Game.Systems.StorageSystem
{
	public interface IStorageData<T>
	{
		T GetData();
		void SetData(T data);
	}
}