using Game.Systems.InventorySystem;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

public class UIPlayerBackpackWindow : WindowBase
{
	public UnityAction onCanceled;

	public UIPlayerInventoryWindow InventoryWindow => inventoryWindow;

	[SerializeField] private Button cancel;

    [SerializeField] private UIPlayerInventoryWindow inventoryWindow;

	[Inject]
    private void Construct(UIManager uiManager)
	{
		uiManager.WindowsManager.Register(this);

		cancel.onClick.AddListener(OnCanceled);
	}

	private void OnDestroy()
	{
		cancel.onClick.RemoveAllListeners();
		onCanceled = null;
	}

	public override void Show()
	{
		inventoryWindow.Show();
		base.Show();
	}

	private void OnCanceled()
	{
		onCanceled?.Invoke();
	}
}