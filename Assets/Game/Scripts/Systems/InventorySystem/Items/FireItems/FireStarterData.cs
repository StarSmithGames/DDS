using UnityEngine;

[CreateAssetMenu(menuName = "Game/Inventory/Items/Fire/Starter", fileName = "Starter")]
public class FireStarterData : FireItemData
{
    public bool isMathces = false;

    [Tooltip("Сколько секунд розжигать.")]
    [Min(1f)]
    public float holdTime;

    //[Tooltip("Время на розжиг")]
    //public Times kindleTime;

    //[Tooltip("К времени горения.")]
    //public Times addFireTime;
}