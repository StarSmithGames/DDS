using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
	[System.Serializable]
	public class Item
	{
		public UnityAction OnItemChanged;

		public bool useRandom = false;
		public ItemData ItemData => data;
		[Required]
		[OnValueChanged("OnDataChanged")]
		[SerializeField] private ItemData data;

		[ShowIf("@IsStackable && !useRandom")]
		[MinValue("MinimumStackSize"), MaxValue("MaximumStackSize")]
		[SerializeField] private int currentStackSize = 1;
		public int CurrentStackSize
		{
			get => currentStackSize;
			set
			{
				currentStackSize = value;

				OnItemChanged?.Invoke();
			}
		}
		public int MaximumStackSize
		{
			get
			{
				if (IsInfinityStack) return 999;
				return data?.stackSize ?? 1;
			}
		}
		public int MinimumStackSize => 1;

		public bool IsStackSizeFull => CurrentStackSize == MaximumStackSize;
		public bool IsStackSizeEmpty => CurrentStackSize == 0;

		/// <summary>
		/// ������� ����� ������������ ���������������� � ������� �������� �����. ������ ���������� ������������ � �����.
		/// </summary>
		public int StackDifference => MaximumStackSize - CurrentStackSize;

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

				OnItemChanged?.Invoke();
			}
		}
		public float CurrentWeightRounded => (float)System.Math.Round(CurrentWeight, 2);
		public float MaximumWeight
		{
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

				OnItemChanged?.Invoke();
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

				OnItemChanged?.Invoke();
			}
		}
		public float MaximumCalories => IsConsumable ? (data as ConsumableItemData).calories : 0;//TODO: �������� ����������� ���� �� �������
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

				OnItemChanged?.Invoke();
			}
		}
		public int MaxMagaizneCapacity => 15;
		public int MinimumMagaizneCapacity => 0;

		public bool IsConsumable => data == null ? false : data is ConsumableItemData;
		public bool IsConsumableDrink => IsConsumable ? data is DrinkItemData : false;

		public bool IsFireItem => data == null ? false : data is FireItemData;
		public bool IsFireStarter => IsFireItem ? data is FireStarterData : false;
		public bool IsFireFuel=> IsFireItem ? data is FireFuelData : false;
		public bool IsFireTinder => IsFireItem ? data is FireTinderData : false;
		public bool IsFireAccelerant => IsFireItem ? data is FireAccelerantData : false;

		public bool IsWeapon => false;

		public Item() { }

		public Item GenerateItem()//rnd item
		{
			return null;
		}

		public Item Copy()
		{
			Item item = new Item();

			item.data = data;
			item.currentStackSize = CurrentStackSize;
			item.currentWeight = CurrentWeight;
			item.currentDurability = CurrentDurability;
			item.currentCalories = CurrentCalories;//
			item.currentMagazineCapacity = CurrentMagazineCapacity;//

			return item;
		}

		private string Tittle
		{
			get
			{
				if (data != null && data.localizations.Count > 0)
				{
					return data.GetLocalization(SystemLanguage.English).itemName;
				}

				return "";
			}
		}
		private bool IsStackable => data?.isStackable ?? false;
		private bool IsInfinityStack => data?.isInfinityStack ?? false;
		private bool IsInfinityWeight => data?.isInfinityWeight ?? false;
		private bool IsFloatingWeight => data?.isFloatingWeight ?? false;
		private bool IsBreakable => data?.isBreakable ?? false;

		private void OnDataChanged()
		{
			currentStackSize = MaximumStackSize;
			currentWeight = MaximumWeight;
			currentCalories = MaximumCalories;
			currentMagazineCapacity = MaxMagaizneCapacity;
		}
	}
}