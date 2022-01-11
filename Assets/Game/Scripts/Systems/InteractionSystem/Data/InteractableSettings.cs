using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class InteractableSettings
{
    public InteractableType interactableType;
    [ShowIf("interactableType", InteractableType.Hold)]
    public float holdDuration;
}
public enum InteractableType
{
    None,
    Press,
    Click,
    DoubleClick,
    Hold,
}