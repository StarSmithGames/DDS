using UnityEngine;

[CreateAssetMenu(menuName = "Game/Inventory/Items/Fire/Starter", fileName = "Starter")]
public class FireStarterData : FireItemData
{
    public bool isMathces = false;

    [Tooltip("������� ������ ���������.")]
    [Min(1f)]
    public float holdTime;

    //[Tooltip("����� �� ������")]
    //public Times kindleTime;

    //[Tooltip("� ������� �������.")]
    //public Times addFireTime;
}