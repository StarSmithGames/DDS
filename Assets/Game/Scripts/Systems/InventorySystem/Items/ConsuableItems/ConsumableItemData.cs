using Sirenix.OdinInspector;
using UnityEngine;

public class ConsumableItemData : ItemData
{
    [Tooltip("����������� ��������� ���������� ������� � ��������.")]
    [SuffixLabel("Kcal")]
    [Range(0, 2500)]
    public float calories = 0f;

    [Tooltip("������� �� ��������, ������� ��������� ����� ������ ��� 100% ��������.")]
    [SuffixLabel("%", true)]
    [Range(-100f, 100f)]
    public float hydration = 0f;

    //public bool isHaveEffects = false;

    //[ShowIf("isHaveEffects")]
    //public List<EffectSD> effects = new List<EffectSD>();
}