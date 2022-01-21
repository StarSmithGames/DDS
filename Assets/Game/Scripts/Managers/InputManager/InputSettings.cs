using UnityEngine;

[System.Serializable]
public class InputSettings
{
    public KeyboardSettings keyboard;
    public MobileSettings mobile;
}
public enum InputType
{
    Escape,

    Interaction,
    
    Inventory,

    RadialMenu,

    BuildingAccept,
    BuildingReject,
}

[System.Serializable]
public class KeyboardSettings
{
    public string mouseHorizontalAxis = "Mouse X";
    public string mouseVerticalAxis = "Mouse Y";

    public bool invertHorizontalInput = false;
    public bool invertVerticalInput = false;

    public float mouseInputMultiplier = 0.01f;

    [Space]
    public string horizontalInputAxis = "Horizontal";
    public string verticalInputAxis = "Vertical";

    public bool useJump = true;
    public KeyCode jumpKey = KeyCode.Space;
    [Space]
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode inventoryKey = KeyCode.I;
    public KeyCode radialMenuKey = KeyCode.Tab;
    [Space]
    public KeyCode buildingAcceptKey = KeyCode.Mouse0;
    public KeyCode buildingRejectKey = KeyCode.Mouse1;

    //If this is enabled, Unity's internal input smoothing is bypassed;
    public bool useRawInput = true;
}

[System.Serializable]
public class MobileSettings
{
    public bool invertHorizontalInput = false;
    public bool invertVerticalInput = false;

    public float inputMultiplier = 0.01f;
}