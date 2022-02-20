using Game.Editor.GDocs;
using Game.Systems.LocalizationSystem;

using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditor.VersionControl;

using UnityEngine;

using static Game.Editor.GDocs.Table;

namespace Game.Editor.GDocs
{
    [GoogleTaskAttribute("Localization")]
    public class GDocsImportLocalization
    {
        private static string tableName = "Main";
        private static string sheetName = "Localization";

		private static void OnGUI(GDocsEditorWindow manager)
        {
            if (GUILayout.Button("Import Localization"))
            {
				var tasks = GetImport();
				foreach (var task in tasks)
				{
					manager.PushTask(task);
				}
			}
        }

        private static EditorTask[] GetImport()
        {
            var editorTasks = new List<EditorTask>();

            editorTasks.Add(new EditorTaskFunction(() => { ImportLocalization(); }));

            return editorTasks.ToArray();
        }

		private static void ImportLocalization()
		{
			try
			{
				var table = GDocsUtils.GetTable(tableName);

				Dictionary<string, LocalizationData> localizations = new Dictionary<string, LocalizationData>();

				void AddLocalization(IList<CellEntry> entry, string lang)
				{
					var id = table.GetElementValue(entry, "id");

					if (!id.StartsWith("#") && !string.IsNullOrEmpty(id))
					{
						if (!localizations.ContainsKey(lang))
						{
							localizations.Add(lang, new LocalizationData());
						}
						if (localizations.TryGetValue(lang, out LocalizationData data))
						{
							data.Id = lang;
							data.Keys.Add(id);
							data.Values.Add(table.GetElementValue(entry, lang));
						}
					}
				}

				var headers = table.GetHeaders(sheetName);

				table.Map(sheetName, (entry) =>
				{
					for (int i = 1; i < headers.Count; i++)
					{
						AddLocalization(entry, headers[i]);
					}
				});


				if (!Directory.Exists("Assets/Game/Localization"))
				{
					Directory.CreateDirectory("Assets/Game/Localization");
				}

				List<LocalizationData> datas = new List<LocalizationData>();

				foreach (var item in localizations)
				{
					var assetPath = $"Assets/Game/Localization/{item.Key}.asset";

					var asset = item.Value;
					asset.name = item.Key;

					var existingSettings = AssetDatabase.LoadAssetAtPath<LocalizationAsset>(assetPath);
					if (existingSettings == null)
					{
						AssetDatabase.CreateAsset(asset, assetPath);
					}
					else
					{
						EditorUtility.CopySerialized(asset, existingSettings);
					}

					datas.Add(asset);
				}

				AssetDatabaseExtensions.GetAsset<LocalizationInstaller>("Installers/LocalizationInstaller.asset").localizations = datas;

				AssetDatabase.SaveAssets();
			}
			catch (Exception e)
			{
				Debug.LogError("Something wrong: " + e.Message);
			}
		}
	}
}