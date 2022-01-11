using UnityEngine;

[CreateAssetMenu(menuName = "Input", fileName = "InputData")]
public class InputSettings : ScriptableObject
{
    public VisionSettings vision;
    [Space]
    public KeyboardSettings keyboard;
    public MobileSettings mobile;
    [Space]
    [Tooltip("Используется для ItemViewer")]
    public ItemViewer.TransitionSettings transitionSettings;
}