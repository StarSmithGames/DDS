using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "GlobalSettings", fileName = "GlobalData")]
public class GlobalSettings : ScriptableObject
{
    [InlineProperty]
    public ProjectSettings projectSettings;
    [Space]
    public InputSettings input;
    [Space]
    public CameraSettings cameraSettings;
    public VisionSettings visionSettings;
    [Space]
    [Tooltip("������������ ��� ItemInspectorHandler")]
    public InspectorHandler.Settings inspectorSettings;
    [Tooltip("������������ ��� ItemViewer")]
    public TransformTransition.Settings transitionSettings;
}
[System.Serializable]
public class ProjectSettings
{
    //public BuildTarget buildTarget;
    public PlatformType platform;
}
public enum PlatformType
{
    Desktop = 0,
    Mobile = 1,
    Consoles = 2,
    ExtendedReality = 3,
}