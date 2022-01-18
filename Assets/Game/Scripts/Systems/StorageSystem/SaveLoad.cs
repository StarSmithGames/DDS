using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Game.Systems.StorageSystem
{
	public class SaveLoad
	{
		public static void SaveToPlayerPrefs<T>(T data, string key)
		{
			PlayerPrefs.SetString(key, ConvertToJson(data));
		}

		public static string ConvertToJson(object data)
		{
			return JsonUtility.ToJson(data, true);
		}
		public static T ConvertFromJson<T>(string json)
		{
			return JsonUtility.FromJson<T>(json);
		}

		public static string SerializeObjectToJson(Dictionary<string, object> data)
		{
			return JsonConvert.SerializeObject(data);
		}
		public static Dictionary<string, object> DeserializeObjectFromJson(string json)
		{
			return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
		}




		/// <summary>
		/// Сохранение объекта в Json.
		/// </summary>
		private static void SaveDataToJson<T>(T data, string directory, string fileName)
		{
			string dir = Application.persistentDataPath + directory;

			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			string jsonData = JsonUtility.ToJson(data, true);
			File.WriteAllText(dir + fileName, jsonData);
		}

		/// <summary>
		/// Загрузка объекта из Json в объект.
		/// </summary>
		private static T LoadDataFromJson<T>(string directory, string fileName)
		{
			string fullPath = Application.persistentDataPath + directory + fileName;

			if (File.Exists(fullPath))
			{
				string json = File.ReadAllText(fullPath);
				return JsonUtility.FromJson<T>(json);
			}

			throw new Exception("File doesn't exist");
		}
		private static T LoadDataFromJson<T>(string fullPath)
		{
			if (File.Exists(fullPath))
			{
				string json = File.ReadAllText(fullPath);
				return JsonUtility.FromJson<T>(json);
			}

			throw new Exception("File doesn't exist");
		}
	}
}