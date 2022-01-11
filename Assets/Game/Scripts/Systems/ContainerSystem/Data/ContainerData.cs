using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContainerData", menuName = "ContainerData")]
public class ContainerData : ScriptableObject
{
    public InteractableSettings interact;
    public InteractableSettings inspect;
}