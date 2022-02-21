using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
using DG.Tweening;

public class UIDeathWindow : WindowBase
{
	[SerializeField] private CanvasGroup canvasGroup;

	[Inject]
	private void Construct(UIManager uiManager)
	{
		uiManager.WindowsManager.Register(this);
	}

	public override void Show()
	{
		canvasGroup.alpha = 0;
		base.Show();
		canvasGroup.DOFade(1, 1);
	}
}