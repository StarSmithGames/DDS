using UnityEngine;

namespace Game.Systems.RadialMenu
{
    public class UIRadialMenu : MonoBehaviour
    {
        public UIRadialMenuCursor Cursor => cursor;
        [SerializeField] private UIRadialMenuCursor cursor;

        public void SetActive(bool trigger)
		{
            gameObject.SetActive(trigger);
		}
    }
}