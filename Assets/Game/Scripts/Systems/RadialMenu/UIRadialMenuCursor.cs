using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.RadialMenu
{
	public class UIRadialMenuCursor : MonoBehaviour
	{
		public float FillAmount { get => filler.fillAmount; set => filler.fillAmount = value; }

		public Image Filler => filler;
		[SerializeField] private Image filler;
	}
}