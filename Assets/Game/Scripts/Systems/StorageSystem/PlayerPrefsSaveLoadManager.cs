using System;
using UnityEngine;

using Zenject;

namespace Game.Systems.StorageSystem
{
	public class PlayerPrefsSaveLoadManager : ISaveLoadManager, IInitializable, IDisposable
	{
		private SignalBus signalBus;
		private Settings settings;

		private DefaultData defaultData;
		private Storage activeStorage;

		public PlayerPrefsSaveLoadManager(SignalBus signalBus,
			DefaultData defaultData, Settings settings)
		{
			this.signalBus = signalBus;
			this.defaultData = defaultData;
			this.settings = settings;

			Load();
		}
		public void Initialize()
		{
		}
		public void Dispose()
		{
			Save();
		}

		public void Save()
		{
			string preferenceName = settings.preferenceName;
			PlayerPrefs.SetString(preferenceName, activeStorage.Database.GetJson());
			PlayerPrefs.Save();

			//signalBus?.Fire(new StorageSaveSignal() { storage = activeStorage });

			Debug.Log($"[SAVE] Save storage to pref: {preferenceName}");
		}

		public void Load()
		{
			string preferenceName = settings.preferenceName;

			if (PlayerPrefs.HasKey(preferenceName))
			{
				string json = PlayerPrefs.GetString(preferenceName);

				activeStorage = new Storage(json);
			}
			else//first time
			{
				activeStorage = new Storage(defaultData);

				Debug.Log($"[SAVE] Create new save");

				Save();
			}

			//signalBus?.Fire(new StorageLoadSignal() { storage = activeStorage });

			Debug.Log($"[LOAD] Load storage from pref: {preferenceName}");
		}

		public Storage GetStorage()
		{
			return activeStorage;
		}

		[System.Serializable]
		public class Settings
		{
			public string preferenceName = "save_data";
			public string storageDisplayName = "Profile";
			public string storageFileName = "Profile.dat";
		}
	}
}