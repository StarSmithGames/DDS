using UnityEngine;

namespace Game.Systems.InventorySystem
{
    [CreateAssetMenu(menuName = "Game/Inventory/Items/Fire/Starter", fileName = "Starter")]
    public class FireStarterData : FireItemData
    {
        [Tooltip("����� ��������� �� ������.")]
        public TimeSystem.Time ignitionTime = new TimeSystem.Time() { Seconds = 300 };
        [Min(1f)]
        [Tooltip("������� ������ ���������.")]
        public float holdTime;
        [Space]
        [Tooltip("���������� ����� � ������� ����.")]
		public TimeSystem.Time addFireTime;
	}
}