using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "GlobalSettings", fileName = "GlobalData")]
public class GlobalSettings : ScriptableObject
{
    [InlineProperty]
    public ProjectSettings projectSettings;
    [Space]
    public KeyboardSettings keyboard;
    public MobileSettings mobile;
    [Space]
    public VisionSettings vision;
    [Space]
    [Tooltip("Используется для ItemInspectorHandler")]
    public ItemInspectorHandler.ItemInspectorSettings itemInspector;
    [Tooltip("Используется для ItemViewer")]
    public ItemViewer.TransitionSettings transitionSettings;
}

[System.Serializable]
public class ProjectSettings
{
    public bool isMobile = true;
}