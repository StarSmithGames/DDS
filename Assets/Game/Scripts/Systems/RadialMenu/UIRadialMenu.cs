using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.RadialMenu
{
    public class UIRadialMenu : MonoBehaviour
    {
        [OnValueChanged("OnValueChanged")]
        [SerializeField] private Color accentColor;

        [OnValueChanged("OnValueChanged")]
        [SerializeField] private Color backgroundColor;

        public UIRadialMenuCursor Cursor => cursor;
        [SerializeField] private UIRadialMenuCursor cursor;

        public Image Background => background;
        [SerializeField] private Image background;

        public void SetActive(bool trigger)
		{
            gameObject.SetActive(trigger);
		}

        private void OnValueChanged()
		{
            if(cursor != null)
			{
                cursor.Filler.color = accentColor;
            }
            if(background != null)
			{
                background.color = backgroundColor;
            }
        }
    }
}