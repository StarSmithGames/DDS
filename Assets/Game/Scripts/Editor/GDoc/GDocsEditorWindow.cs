using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace Game.Editor.GDocs
{
    /// <summary>
    /// Главное окно google api.
    /// </summary>
    public class GDocsEditorWindow : EditorWindowTaskable
    {
        private static Type[] loadedTypes = { };

        [MenuItem("Tools/Google/Manager")]
        private static void OpenManager()
        {
            GetWindow(typeof(GDocsEditorWindow), true, "GoogleManager");
            loadedTypes = GetTypesWithHelpAttribute(Assembly.GetExecutingAssembly()).ToArray();
        }

        [MenuItem("Tools/Google/Open in browser")]
        private static void OpenURL()
        {
            Application.OpenURL("https://drive.google.com/drive/u/0/folders/1nE0j1WFDurd8Zp0EM9rKcxHAoESggbzv");
        }

        private static IEnumerable<Type> GetTypesWithHelpAttribute(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(GoogleTaskAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        public void OnGUI()
        {
            if (loadedTypes == null || loadedTypes.Length == 0)
            {
                loadedTypes = GetTypesWithHelpAttribute(Assembly.GetExecutingAssembly()).ToArray();
            }

            if (!IsInProgress)
            {
                foreach (var t in loadedTypes)
                {
                    var method = t.GetMethod("OnGUI", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    if (method != null)
                    {
                        method.Invoke(null, new object[] { this });
                    }
                }
            }
        }
    }
}