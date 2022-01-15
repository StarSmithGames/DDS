using UnityEngine;
using Sirenix.OdinInspector;

[InlineProperty]
[System.Serializable]
public class InteractionSettings
{
    public InteractionType interactionType;
    [ShowIf("interactionType", InteractionType.Hold)]
    public float holdDuration;
}
public enum InteractionType
{
    None            = 0,
    Press           = 1,
    Click           = 2,
    DoubleClick     = 3,
    Hold            = 4,
}