using UnityEngine;

public class WindowBase : MonoBehaviour, IWindow
{
	private bool isShowing = false;
	public bool IsShowing => isShowing;

	public virtual void Show()
	{
		gameObject.SetActive(true);

		isShowing = true;
	}

	public virtual void Hide()
	{
		gameObject.SetActive(false);

		isShowing = false;
	}
}