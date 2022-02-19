using UnityEditor;

using UnityEngine;

namespace Game.Editor.GDocs
{
    /// <summary>
    /// Утилит класс для помощи в работе с google sheets.
    /// </summary>
    public class GDocsUtils
    {
        private static string key = "client_secrets.json";
        private static string KeyPath => Application.dataPath + "/Game/Configurations/Google/" + key;

        public static Table GetTable(string sheetName)
        {
            TableData data = GetAsset<TableData>(sheetName);
            return new Table(data, KeyPath);
        }

        public static T GetAsset<T>(string path) where T : ScriptableObject
        {
            return AssetDatabase.LoadAssetAtPath<T>("Assets/Game/Configurations/Google/" + path + ".asset");
        }
    }
}