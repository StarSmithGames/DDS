using UnityEngine;

using Sirenix.OdinInspector;

[System.Serializable]
public class Item
{
	[ShowInInspector]
	public System.Guid ID { get; protected set; }

	public bool useRandom = false;
	public ItemData ItemData => data;
	[Required]
	[OnValueChanged("OnDataChanged")]
	[SerializeField] private ItemData data;

	[HideIf("@IsInfinityStack || useRandom")]
	[MinValue("MinimumStackSize"), MaxValue("MaximumStackSize")]
	[SerializeField] private int currentStackSize = 1;
	public int CurrentStackSize
	{
		get => currentStackSize;
		set
		{
			currentStackSize = value;
		}
	}
	public int MaximumStackSize => data?.stackSize ?? 1;
	public int MinimumStackSize => 1;

	[HideIf("@!IsFloatingWeight || useRandom")]
	[MinValue("MinimumWeight"), MaxValue("MaximumWeight")]
	[SuffixLabel("kg", true)]
	[SerializeField] private float currentWeight = 0.01f;
	public float CurrentWeight
	{
		get => currentWeight;
		set
		{
			currentWeight = value;
		}
	}
	public float CurrentWeightRounded => (float)System.Math.Round(CurrentWeight, 2);
	public float MaximumWeight {
		get
		{
			if (IsInfinityWeight) return 99.99f;
			return data?.weight ?? 99.99f;
		}
	}
	public float MinimumWeight
	{
		get
		{
			if (data?.isFloatingWeight ?? false) return data.baseWeight;
			return 0.01f;
		}
	}

	[ShowIf("@IsBreakable && !useRandom")]
	[MinValue("MinimumDurability"), MaxValue("MaximumDurability")]
	[SuffixLabel("%", true)]
	[SerializeField] private float currentDurability = 100f;
	public float CurrentDurability
	{
		get => currentDurability;
		set
		{
			currentDurability = value;
		}
	}
	public float MaximumDurability => 100f;
	public float MinimumDurability => 0;

	[ShowIf("@IsConsumable && !useRandom")]
	[BoxGroup("Consumble")]
	[MinValue("MinimumCalories"), MaxValue("MaximumCalories")]
	[SerializeField] private float currentCalories;
	public float CurrentCalories
	{
		get => currentCalories;
		set
		{
			currentCalories = value;
		}
	}
	public float MaximumCalories => IsConsumable ? (data as ConsumableItemData).calories : 0;//�������� ����������� ���� �� �������
	public float MinimumCalories => 0f;

	[ShowIf("@IsWeapon && !useRandom")]
	[BoxGroup("Weapon")]
	[MinValue("MinimumMagaizneCapacity"), MaxValue("MaxMagaizneCapacity")]
	[SerializeField] private int currentMagazineCapacity;
	public int CurrentMagazineCapacity
	{
		get => currentMagazineCapacity;
		set
		{
			currentMagazineCapacity = value;
		}
	}
	public int MaxMagaizneCapacity => 15;
	public int MinimumMagaizneCapacity => 0;


	public bool IsConsumable => data is ConsumableItemData;
	public bool IsWeapon => false;

	public Item()
	{
		ID = System.Guid.NewGuid();
	}

	public Item GenerateItem()
	{
		return null;
	}

	private string Tittle => data?.itemName ?? "";

	private bool IsInfinityStack => data?.isInfinityStack ?? false;
	private bool IsInfinityWeight => data?.isInfinityWeight ?? false;
	private bool IsFloatingWeight => data?.isFloatingWeight ?? false;
	private bool IsBreakable => data?.isBreakable ?? false;

	private void OnDataChanged()
	{
		currentWeight = MaximumWeight;
		currentCalories = MaximumCalories;
	}
}