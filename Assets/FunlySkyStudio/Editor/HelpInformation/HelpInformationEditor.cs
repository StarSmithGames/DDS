using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Funly.SkyStudio;

[CustomEditor(typeof(HelpInformation))]
public class HelpInformationEditor : Editor
{

  [MenuItem("Window/Sky Studio/Help/Join our Discord server...")]
  private static void OpenDiscordChat()
  {
    Application.OpenURL("http://bit.ly/2GteOFN");
  }

  [MenuItem("Window/Sky Studio/Help/Video Tutorials...")]
  private static void OpenVideoTutorials()
  {
    Application.OpenURL("http://bit.ly/2GpFVl2");
  }

  [MenuItem("Window/Sky Studio/Help/Review Sky Studio...")]
  private static void OpenSkyStudioStorePage()
  {
    Application.OpenURL("http://bit.ly/2GvkjUv");
  }

  public override void OnInspectorGUI()
  {
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PrefixLabel("Tutorial Videos");
    bool didClick = GUILayout.Button(new GUIContent("Open Tutorials..."));
    if (didClick) {
      OpenVideoTutorials();
    }
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PrefixLabel("Discord Chat");
    didClick = GUILayout.Button(new GUIContent("Join Discord for help..."));
    if (didClick) {
      OpenDiscordChat();
    }
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PrefixLabel("Love Sky Studio?");
    didClick = GUILayout.Button(new GUIContent("Please Write a Review..."));
    if (didClick) {
      OpenSkyStudioStorePage();
    }
    EditorGUILayout.EndHorizontal();
  }
}
