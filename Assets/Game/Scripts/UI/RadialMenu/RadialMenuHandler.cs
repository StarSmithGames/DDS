using DG.Tweening;

using Game.Entities;
using Game.Managers.InputManger;
using Game.Systems.RadialMenu;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class RadialMenuHandler : IInitializable, IDisposable, ITickable
{
	public bool IsOpened => isOpened;
	private bool isOpened = false;

	private List<UIRadialMenuOption> options = new List<UIRadialMenuOption>();

	private UIRadialMenu menu;

	private SignalBus signalBus;
	private RadialMenuSettings settings;
	private UIManager uiManager;
	private UIRadialMenuOption.Factory optionFactory;
	private Player player;

	public RadialMenuHandler(SignalBus signalBus,
		RadialMenuSettings settings,
		UIManager uiManager,
		UIRadialMenuOption.Factory optionFactory,
		Player player)
	{
		this.signalBus = signalBus;
		this.settings = settings;
		this.uiManager = uiManager;
		this.optionFactory = optionFactory;
		this.player = player;

		this.menu = uiManager.RadialMenu;
	}

	public void Initialize()
	{
		uiManager.RadialMenu.SetActive(false);

		signalBus?.Subscribe<SignalRadialMenuOptionChanged>(OnOptionChanged);

		signalBus?.Subscribe<SignalInputUnPressed>(OnInputClicked);
		signalBus?.Subscribe<SignalInputKeyUp>(OnInputKeyUp);
	}

	public void Dispose()
	{
		signalBus?.Unsubscribe<SignalRadialMenuOptionChanged>(OnOptionChanged);

		signalBus?.Unsubscribe<SignalInputUnPressed>(OnInputClicked);
		signalBus?.Unsubscribe<SignalInputKeyUp>(OnInputKeyUp);
	}

	public void Tick()
	{
		if (!isOpened) return;

		Vector3 screenBounds = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
		Vector3 vector = Input.mousePosition - screenBounds;

		float mouseRotation = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
		if (mouseRotation < 0f)
			mouseRotation += 360f;
		float cursorRotation = -(mouseRotation - menu.Cursor.FillAmount * 180f);

		float difference = 9999;
		UIRadialMenuOption nearest = null;
		for (int i = 0; i < options.Count; i++)
		{
			float rotation = options[i].Rotation;

			if (Mathf.Abs(rotation - mouseRotation) < difference)
			{
				nearest = options[i];
				difference = Mathf.Abs(rotation - mouseRotation);
			}
		}

		if (settings.isSnap)
			cursorRotation = -(nearest.Rotation - menu.Cursor.FillAmount * 360f / 2f);
		menu.Cursor.transform.localRotation = Quaternion.Euler(0, 0, cursorRotation);
	}

	public void OpenMenu()
	{
		if (isOpened) return;

		Sequence sequence = DOTween.Sequence();

		sequence
			.AppendCallback(
			() =>
			{
				player.DisableVision();
				player.Freeze();

				menu.transform.localScale = Vector3.zero;
				UpdateRadialMenu();
				menu.SetActive(true);
				isOpened = true;
			})
			.Append(menu.transform.DOScale(1, 0.45f));
	}
	public void CloseMenu()
	{
		if (!isOpened) return;

		Sequence sequence = DOTween.Sequence();

		sequence
			.AppendCallback(() =>
			{
				player.EnableVision();
				player.UnFreeze();
			})
			.Append(menu.transform.DOScale(0, 0.25f))
			.AppendCallback(() =>
			{
				isOpened = false;
				menu.SetActive(false);
			});
	}

	private void UpdateRadialMenu()
	{
		ReSizeOptions();

		UpdateRadius();
	}

	private void UpdateRadius()
	{
		if (!Application.isPlaying) return;

		float fillRadius = settings.NormalCount * 360f;
		menu.Cursor.FillAmount = settings.NormalCount;

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

	private void ReSizeOptions()
	{
		if (!Application.isPlaying) return;

		int diff = options.Count - settings.countOptions;

		if (diff > 0)//remove diff
		{
			for (int i = diff - 1; i >= 0; i--)
			{
				var option = options[i];
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

				options.Add(option);
			}
		}
	}


	//ѕроисходит когда клик, возможно следует удалить
	private void OnOptionChanged(SignalRadialMenuOptionChanged signal)
	{
		CloseMenu();
	}
	private void OnInputClicked(SignalInputUnPressed signal)
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
				OpenMenu();
			}
		}
	}
	private void OnInputKeyUp(SignalInputKeyUp signal)
	{
		if (isOpened)
		{
			if (signal.key == KeyCode.Mouse0)
			{
				CloseMenu();
			}
		}
	}
}