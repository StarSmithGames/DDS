using Game.Editor.GDocs;
using Game.Editor;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Editor.GDocs
{
    [GoogleTaskAttribute("Configuration")]
    public class GDocsImportConfiguration
    {
        private static string tableName = "Main";
        private static string sheetName = "Dashboard";

        private static void OnGUI(GDocsEditorWindow manager)
        {
            if (GUILayout.Button("Import"))
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

            editorTasks.Add(new EditorTaskFunction(() => { ImportDashboard(); }));

            return editorTasks.ToArray();
        }

        private static void ImportDashboard()
        {
			try
			{
				var table = GDocsUtils.GetTable(tableName).GetSheet(sheetName);
				for (int i = 0; i < table.Count; i++)
				{
					for (int j = 0; j < table[i].Count; j++)
					{
						Debug.LogError(table[i][j].ToString());
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogError("Something wrong: " + e.Message);
			}
		}
    }
}