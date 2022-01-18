using System.Collections;
using System.Collections.Generic;
using UnityEditor;

using UnityEngine;

namespace Game.Systems.StorageSystem
{
	/// <summary>
	/// Хранилище данных.
	/// </summary>
	public class Storage
	{
		public Database Database => database;
		private Database database;

		//public IStorageData<CommonDictionaryData> CommonDictionaryData { get; private set; }
		//public IStorageData<ReaderData.Settings> ReaderData_Settings { get; private set; }

		public Storage(DefaultData data)
		{
			database = new Database();

			Initialization();

			//Set storage to default
			//CommonDictionaryData.SetData(new CommonDictionaryData());
			//ReaderData_Settings.SetData(data.readerDefaultSettings.GetData());
		}

		public Storage(string json)
		{
			database = new Database();
			Database.LoadJson(json);

			Initialization();
		}

		private void Initialization()
		{
			//CommonDictionaryData = new StorageData<CommonDictionaryData>(database, "common_dictionary_data");
			//ReaderData_Settings = new StorageData<ReaderData.Settings>(database, "reader_data");
		}

		[System.Serializable]
		public class Reference
		{
			public string displayName;
			public string fileName;
		}
	}

	[System.Serializable]
	public class DefaultData
	{
		//public ReaderData.Settings readerDefaultSettings;
	}
}