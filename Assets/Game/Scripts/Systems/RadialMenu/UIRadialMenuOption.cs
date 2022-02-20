using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using Zenject;
using UnityEngine.Events;

namespace Game.Systems.RadialMenu
{
	public class UIRadialMenuOption : PoolableObject
	{
		public UnityAction<UIRadialMenuOption> onButtonClicked;

		[OnValueChanged("OnValueChanged")]
		[SerializeField] private Color accentColor;
		[SerializeField] private Color selectedColor;
		[SerializeField] private Color disabledColor;

		[SerializeField] private Button button;
		[SerializeField] private Image icon;

		public float Rotation { get; set; }

		public bool IsNull { get; private set; }
		public bool IsEmpty { get; private set; }
		public bool IsValid { get; private set; }

		public RadialMenuOptionData Data => data;
		private RadialMenuOptionData data;

		[Inject]
		private void Construct()
		{
			button.onClick.AddListener(OnButtonClicked);
		}

		private void OnDestroy()
		{
			button.onClick.RemoveAllListeners();

			onButtonClicked = null;
		}

		public void SetData(RadialMenuOptionData data, bool isValid = false)
		{
			this.data = data;

			IsNull = data == null;
			IsEmpty = data?.IsEmpty() ?? true;
			IsValid = isValid;

			UpdateOption();
		}

		private void UpdateOption()
		{
			if (IsNull)
			{
				icon.enabled = false;
				button.interactable = false;
				
				return;
			}

			icon.sprite = data.GetIcon();
			icon.enabled = icon.sprite != null;

			button.interactable = !IsEmpty && IsValid;

			if (data.IsCanSetColor())
			{
				icon.color = IsEmpty || !IsValid ? disabledColor : accentColor;
			}
			else
			{
				icon.color = Color.white;
			}
		}

		public void Select()
		{
			transform.DOScale(1.35f, 0.1f);

			if (IsNull) return;

			if (button.interactable)
			{
				if (data.IsCanSetColor())
				{
					icon.color = selectedColor;
				}
			}
			else
			{
				if (data.IsCanSetColor())
				{
					icon.color = disabledColor;
				}
			}
		}
		public void Diselect()
		{
			if (transform.localScale != Vector3.one)
			{
				transform.DOScale(1f, 0.1f);
			}

			if (IsNull) return;

			if (button.interactable)
			{
				if (data.IsCanSetColor())
				{
					icon.color = accentColor;
				}
			}
		}

		public bool InvokeAction()
		{
			if (IsNull) return false;

			OnButtonClicked();

			return true;
		}

		private void OnButtonClicked()
		{
			onButtonClicked?.Invoke(this);
		}

		public class Factory : PlaceholderFactory<UIRadialMenuOption> { }
	}
}