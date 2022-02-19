using DG.Tweening;

using Game.Entities;
using Game.Managers.InputManger;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

using Zenject;
using Game.Systems.BuildingSystem;
using Game.Systems.InventorySystem;
using Game.Systems.PassTimeSystem;

namespace Game.Systems.RadialMenu
{
	public class RadialMenuHandler : IInitializable, IDisposable, ITickable
	{
		public bool IsOpened => isOpened;
		private bool isOpened = false;

		private bool isOutOfZone = false;

		private UIRadialMenu menu;
		private List<UIRadialMenuOption> options = new List<UIRadialMenuOption>();
		private UIRadialMenuOption nearestOption = null;

		private SignalBus signalBus;
		private RadialMenuSettings settings;
		private InputManager inputManager;
		private UIManager uiManager;
		private UIRadialMenuOption.Factory optionFactory;
		private Player player;
		private BuildingHandler buildingHandler;
		private PassTimeHandler passTimeHandler;

		public RadialMenuHandler(SignalBus signalBus,
			RadialMenuSettings settings,
			InputManager inputManager,
			UIManager uiManager,
			UIRadialMenuOption.Factory optionFactory,
			Player player,
			BuildingSystem.BuildingHandler buildingHandler,
			PassTimeHandler passTimeHandler)
		{
			this.signalBus = signalBus;
			this.settings = settings;
			this.inputManager = inputManager;
			this.uiManager = uiManager;
			this.optionFactory = optionFactory;
			this.player = player;
			this.buildingHandler = buildingHandler;
			this.passTimeHandler = passTimeHandler;

			this.menu = uiManager.RadialMenu;
		}

		public void Initialize()
		{
			uiManager.RadialMenu.SetActive(false);

			signalBus?.Subscribe<SignalRadialMenuButton>(OnRadialMenuButtonClicked);
			signalBus?.Subscribe<SignalInputUnPressed>(OnInputUnPressed);

			signalBus?.Subscribe<SignalInputUp>(OnInputUp);
		}

		public void Dispose()
		{
			signalBus?.Unsubscribe<SignalRadialMenuButton>(OnRadialMenuButtonClicked);
			signalBus?.Unsubscribe<SignalInputUnPressed>(OnInputUnPressed);

			signalBus?.Unsubscribe<SignalInputUp>(OnInputUp);
		}

		public void Tick()
		{
			if (!isOpened) return;

			Vector3 screenBounds = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
			Vector3 direction = inputManager.InputPosition - screenBounds;

			bool isInner = direction.magnitude <= 130f;
			bool isOutter = direction.magnitude >= 600f;
			isOutOfZone = isInner || isOutter;

			float mouseRotation = Mathf.Atan2(direction.x, direction.y) * 57.29578f;
			if (mouseRotation < 0f) mouseRotation += 360f;

			//Нахождение ближайшей опции
			if(!(isOutOfZone))
			{
				float difference = 9999;
				for (int i = 0; i < options.Count; i++)
				{
					if (options[i].IsEmpty) continue;
					if (options[i].Data.IsEmpty()) continue;

					float rotation = options[i].Rotation;

					if (Mathf.Abs(rotation - mouseRotation) < difference)
					{
						nearestOption = options[i];
						difference = Mathf.Abs(rotation - mouseRotation);
					}
				}
			}

			//Поворот курсора и снап
			menu.Cursor.FillAmount = isOutOfZone ? 0 : 1f / (float)options.Count;
			float cursorRotation = -(mouseRotation - menu.Cursor.FillAmount * 180f);
			if(nearestOption != null)
			{
				if (settings.isSnap) cursorRotation = -(nearestOption.Rotation - menu.Cursor.FillAmount * 180f);
				menu.Cursor.transform.localRotation = Quaternion.Euler(0, 0, cursorRotation);
			}


			//Наклон меню
			if (settings.useTilt)
			{
				if (isOutOfZone)
				{
					menu.transform.localRotation = Quaternion.identity;
				}
				else
				{
					float x = direction.x / screenBounds.x;
					float y = direction.y / screenBounds.y;
					menu.transform.localRotation = Quaternion.Euler(new Vector3(x, y, 0) * -settings.tiltAmount);
				}
			}

			//Выбор опции - перекраска
			if (isOutOfZone)
			{
				for (int i = 0; i < options.Count; i++)
				{
					options[i].Diselect();
				}
			}
			else
			{
				for (int i = 0; i < options.Count; i++)
				{
					if (options[i] == nearestOption)
					{
						options[i].Select();
					}
					else
					{
						options[i].Diselect();
					}
				}
			}
		}

		public void OpenMenu()
		{
			if (isOpened) return;

			uiManager.RadialMenuButton.SetIcon(false);

			Sequence sequence = DOTween.Sequence();

			sequence
				.AppendCallback(
				() =>
				{
					player.DisableVision();
					player.Freeze();

					menu.transform.localScale = Vector3.zero;
					SetMenu(settings.primaryMenu);
					menu.SetActive(true);
					isOpened = true;
				})
				.Append(menu.transform.DOScale(1, settings.animationIn));
		}
		public void CloseMenu()
		{
			if (!isOpened) return;

			uiManager.RadialMenuButton.SetIcon(true);

			Sequence sequence = DOTween.Sequence();

			sequence
				.AppendCallback(() =>
				{
					player.EnableVision();
					player.UnFreeze();
				})
				.Append(menu.transform.DOScale(0, settings.animationOut))
				.AppendCallback(() =>
				{
					menu.SetActive(false);
					isOpened = false;
				});
		}

		private void UpdateRadialMenu(int optionsCount)
		{
			ReSizeOptions(optionsCount);
			UpdateOptions();
			ResetOptions();
		}

		private void UpdateOptions()
		{
			float normalCount = 1f / (float)options.Count;
			float fillRadius = normalCount * 360f;
			menu.Cursor.FillAmount = normalCount;

			//y=sin(angle)
			//x=cos(angle)
			float prevRotation = 0;
			for (int i = 0; i < options.Count; i++)
			{
				float rotation = prevRotation + fillRadius / 2;
				prevRotation = rotation + fillRadius / 2;

				options[i].Rotation = rotation;
				options[i].transform.localPosition = new Vector2(settings.maxRadius * Mathf.Cos((rotation - 90) * Mathf.Deg2Rad), -settings.maxRadius * Mathf.Sin((rotation - 90) * Mathf.Deg2Rad));
			}
		}
		private void ReSizeOptions(int optionsCount)
		{
			int diff = options.Count - optionsCount;

			if (diff > 0)//remove diff
			{
				for (int i = diff - 1; i >= 0; i--)
				{
					var option = options[i];
					option.onButtonClicked -= OnOptionClicked;
					options.Remove(option);
					option.DespawnIt();
				}
			}
			else//add diff
			{
				for (int i = 0; i < -diff; i++)
				{
					UIRadialMenuOption option = optionFactory.Create();
					option.transform.SetParent(menu.transform);
					option.transform.position = Vector3.zero;
					option.transform.localScale = Vector3.one;

					option.onButtonClicked += OnOptionClicked;

					options.Add(option);
				}
			}
		}
		private void ResetOptions()
		{
			for (int i = 0; i < options.Count; i++)
			{
				options[i].SetData(null);
			}
		}

		private void SetMenu(RadialMenuData menu)
		{
			UpdateRadialMenu(menu.options.Count);

			for (int i = 0; i < menu.options.Count; i++)
			{
				options[i].SetData(menu.options[i].optionType == RadialMenuOptionType.None ? null : menu.options[i]);
			}
		}

		private void OnOptionClicked(UIRadialMenuOption option)
		{
			RadialMenuOptionType type = option.Data.optionType;

			switch (type)
			{
				case RadialMenuOptionType.GoToDrink:
				case RadialMenuOptionType.GoToFood:
				{
					List<Item> items = player.Status.Inventory.Items
						.Where((item) => type == RadialMenuOptionType.GoToDrink ? item.IsConsumableDrink : item.IsConsumableFood).ToList();

					if (items.Count > 0)
					{
						int itemsCount = items.Count > 12 ? 12 : items.Count;
						UpdateRadialMenu(12);

						//if > 12 берём 7-back 6-right
						for (int i = 0; i < options.Count; i++)
						{
							if (i < itemsCount)
							{
								options[i].SetData(new RadialMenuOptionData()
								{ 
									optionType = RadialMenuOptionType.Drink,
									item = items[i],
								});
							}
							else
							{
								options[i].SetData(null);
							}
						}
					}

					break;
				}
				case RadialMenuOptionType.GoTo:
				{
					SetMenu(option.Data.radialMenu);
					break;
				}

				case RadialMenuOptionType.Drink:
				case RadialMenuOptionType.Eat:
				{
					CloseMenu();
					Debug.LogError("Drink - Eat");

					break;
				}

				case RadialMenuOptionType.Build:
				{
					CloseMenu();
					buildingHandler.SetBlueprint(option.Data.blueprint);
					break;
				}

				case RadialMenuOptionType.OpenPassTime:
				{
					CloseMenu();
					passTimeHandler.OpenPassTime(PassTimeType.OnlyPassTime);
					break;
				}
			}
		}

		private void TryInvokeOption()
		{
			if (nearestOption != null)
			{
				if (!nearestOption.InvokeAction())
				{
					CloseMenu();
				}
			}
			else
			{
				CloseMenu();
			}
		}


		private void OnRadialMenuButtonClicked()
		{
			if (IsOpened)
			{
				CloseMenu();
			}
			else
			{
				OpenMenu();
			}
		}
		private void OnInputUnPressed(SignalInputUnPressed signal)
		{
			if (signal.input == InputType.Escape && isOpened)
			{
				CloseMenu();
			}
			else if (signal.input == InputType.RadialMenu)
			{
				if (isOpened)
				{
					CloseMenu();
				}
				else
				{
					if (uiManager.WindowsManager.IsAllHided())
					{
						OpenMenu();
					}
				}
			}
		}
		private void OnInputUp()
		{
			if (isOutOfZone)
			{
				CloseMenu();
			}

			if (isOpened && !isOutOfZone)
			{
				TryInvokeOption();
			}
		}
	}
}