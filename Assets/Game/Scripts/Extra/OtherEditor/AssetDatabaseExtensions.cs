#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;

public static class AssetDatabaseExtensions
{
	public static T GetAsset<T>(string path) where T : ScriptableObject
	{
		return AssetDatabase.LoadAssetAtPath<T>("Assets/Game/Resources/Assets/" + path);
	}
}
#endif