using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.Editor.GDocs
{
    [CreateAssetMenu(fileName = "TableData", menuName = "Google/Docs/Table")]
    public class TableData : ScriptableObject
    {
        public string id;
        public string url;
    }
}