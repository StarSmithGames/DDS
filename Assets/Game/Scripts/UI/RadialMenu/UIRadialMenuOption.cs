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
		[OnValueChanged("OnValueChanged")]
		[SerializeField] private Color accentColor;
		[SerializeField] private Color selectedColor;
		[SerializeField] private Color disabledColor;

		[SerializeField] private Button button;
		[SerializeField] private Image icon;

		public float Rotation { get; set; }

		public bool IsEmpty => isEmpty;
		private bool isEmpty = false;

		private bool isCanSetColor = true;

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
		}

		public void SetOption(Sprite sprite, UnityAction action = null, bool isInteractable = true, bool isCanSetColor = true)
		{
			isEmpty = sprite == null;
			icon.enabled = !isEmpty;
			icon.sprite = sprite;

			this.action = action;

			button.interactable = isInteractable;
			
			this.isCanSetColor = isCanSetColor;
			if (isCanSetColor)
			{
				icon.color = isInteractable ? accentColor : disabledColor;
			}
			else
			{
				icon.color = Color.white;
			}
		}

		public bool InvokeAction()
		{
			if(action == null)
				return false;

			action.Invoke();
			return true;
		}

		public void Select()
		{
			if (button.interactable)
			{
				if (isCanSetColor)
				{
					icon.color = selectedColor;
				}

				transform.DOScale(1.35f, 0.1f);
			}
			else
			{
				if (isCanSetColor)
				{
					icon.color = disabledColor;
				}
			}
		}

		public void UnSelect()
		{
			if (button.interactable)
			{
				if (isCanSetColor)
				{
					icon.color = accentColor;
				}

				if(transform.localScale != Vector3.one)
				{
					transform.DOScale(1f, 0.1f);
				}
			}
		}

		private void OnButtonClicked()
		{
		}

		private void OnValueChanged()
		{
			if(icon != null)
			{
				icon.color = accentColor;
			}
		}


		public class Factory : PlaceholderFactory<UIRadialMenuOption> { }
	}
}