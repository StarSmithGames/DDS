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

		public bool IsEmpty { get; private set; }

		public RadialMenuOptionData Data => data;
		private RadialMenuOptionData data;


		private UnityAction action;

		private SignalBus signalBus;

		[Inject]
		private void Construct(SignalBus signalBus)
		{
			this.signalBus = signalBus;

			button.onClick.AddListener(OnButtonClicked);
		}

		private void OnDestroy()
		{
			button.onClick.RemoveAllListeners();

			onButtonClicked = null;
		}

		public void SetData(RadialMenuOptionData data)
		{
			this.data = data;

			UpdateOption();
		}

		private void UpdateOption()
		{
			IsEmpty = data == null;

			if (data == null)
			{
				icon.enabled = false;
				button.interactable = false;
				
				return;
			}

			icon.sprite = data.GetIcon();
			icon.enabled = icon.sprite != null;

			button.interactable = !data.IsEmpty();

			if (data.IsCanSetColor())
			{
				icon.color = data.IsEmpty() ? disabledColor : accentColor;
			}
			else
			{
				icon.color = Color.white;
			}
		}

		public bool InvokeAction()
		{
			if (Data == null) return false;
			
			OnButtonClicked();
			
			return true;
		}

		public void Select()
		{
			transform.DOScale(1.35f, 0.1f);

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
			if (button.interactable)
			{
				if (data.IsCanSetColor())
				{
					icon.color = accentColor;
				}
			}

			if (transform.localScale != Vector3.one)
			{
				transform.DOScale(1f, 0.1f);
			}
		}


		private void OnButtonClicked()
		{
			onButtonClicked?.Invoke(this);
		}

		public class Factory : PlaceholderFactory<UIRadialMenuOption> { }
	}
}