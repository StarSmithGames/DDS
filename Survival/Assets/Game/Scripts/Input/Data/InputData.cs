using UnityEngine;

[CreateAssetMenu(menuName = "Input", fileName = "InputData")]
public class InputData : ScriptableObject
{
    public KeyboardSettings keyboard;
    public MobileSettings mobile;
}