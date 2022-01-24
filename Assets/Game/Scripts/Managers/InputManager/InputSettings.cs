using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[System.Serializable]
public class InputSettings
{
    public KeyboardSettings keyboard;
    public MobileSettings mobile;
}
public enum InputType
{
    None,

    Escape,

    Interaction,
    
    Inventory,

    RadialMenu,

    BuildingAccept,
    BuildingReject,

    IgnitionStartFire,
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
    [TableList]
    public List<KeyCodeBind> keyCodeBinds = new List<KeyCodeBind>();

    //If this is enabled, Unity's internal input smoothing is bypassed;
    public bool useRawInput = true;
}
[System.Serializable]
public class KeyCodeBind
{
    public KeyCode keyCode;
    public List<InputType> inputType = new List<InputType>();
}

[System.Serializable]
public class MobileSettings
{
    public bool invertHorizontalInput = false;
    public bool invertVerticalInput = false;

    public float inputMultiplier = 0.01f;
}