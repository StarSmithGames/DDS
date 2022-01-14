using Game.Systems.InventorySystem.Signals;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UIItemViewer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI itemName;
    [SerializeField] private TMPro.TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Transform itemTypeContent;
    [SerializeField] private Button useButton;
    [SerializeField] private Button actionsButton;
    [SerializeField] private Button dropButton;

    private Item item;

    private SignalBus signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        this.signalBus = signalBus;

        useButton.onClick.AddListener(OnUseClicked);
        actionsButton.onClick.AddListener(OnActionsClicked);
        dropButton.onClick.AddListener(OnDropClicked);
    }

    private void OnDestroy()
    {
        useButton.onClick.RemoveAllListeners();
        actionsButton.onClick.RemoveAllListeners();
        dropButton.onClick.RemoveAllListeners();
    }

    public void SetItem(Item item)
    {
        this.item = item;

        UpdateView();
    }

    private void UpdateView()
    {
        itemName.enabled = item != null;
        itemName.text = item?.ItemData.itemName ?? "";

        itemDescription.enabled = item != null;
        itemDescription.text = item?.ItemData.itemDescription ?? "";

        itemIcon.enabled = item != null;
        itemIcon.sprite = item?.ItemData.itemSprite ?? null;

        useButton.gameObject.SetActive(false);
        actionsButton.gameObject.SetActive(false);
        dropButton.gameObject.SetActive(item != null);
    }

    private void OnUseClicked()
	{

	}
    private void OnActionsClicked()
    {

    }
    private void OnDropClicked()
    {
        signalBus?.Fire(new SignalUIInventoryDrop() { item = item });
        SetItem(null);
    }
}