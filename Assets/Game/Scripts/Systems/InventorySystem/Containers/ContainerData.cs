using UnityEngine;

[CreateAssetMenu(fileName = "ContainerData", menuName = "ContainerData")]
public class ContainerData : ScriptableObject
{
    public InteractionSettings interact;
    public InteractionSettings inspect;
    [Space]
    public InventorySettings inventory;
}