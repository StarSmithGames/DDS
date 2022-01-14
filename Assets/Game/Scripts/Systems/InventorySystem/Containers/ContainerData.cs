using UnityEngine;

[CreateAssetMenu(fileName = "ContainerData", menuName = "ContainerData")]
public class ContainerData : ScriptableObject
{
    public InteractableSettings interact;
    public InteractableSettings inspect;
    [Space]
    public InventorySettings inventory;
}