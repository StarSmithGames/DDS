using UnityEngine;
using Sirenix.OdinInspector;

[InlineProperty]
[System.Serializable]
public class InteractableSettings
{
    public InteractableType interactableType;
    [ShowIf("interactableType", InteractableType.Hold)]
    public float holdDuration;
}
public enum InteractableType
{
    None            = 0,
    Press           = 1,
    Click           = 2,
    DoubleClick     = 3,
    Hold            = 4,
}