using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

public class UIPassTimeTab : MonoBehaviour
{
	public UnityAction<UIPassTimeTab> onPassTimeClicked;

	[SerializeField] private TMPro.TextMeshPro time;
	[SerializeField] private TMPro.TextMeshPro warmthBonus;
	[SerializeField] private Button left;
	[SerializeField] private Button right;
	[SerializeField] private Button passTime;

	[Inject]
	private void Construct()
	{
		passTime.onClick.AddListener(OnPassTimeClicked);
	}

	private void OnDestroy()
	{
		passTime.onClick.RemoveAllListeners();
	}

	private void OnPassTimeClicked()
	{
		onPassTimeClicked?.Invoke(this);
	}
}