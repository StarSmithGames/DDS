using UnityEngine;
using UnityEngine.EventSystems;

public class PointerField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected bool isEnable = true;
    public virtual bool IsEnable { get => isEnable; set => isEnable = value; }
    public virtual bool IsPressed { get; set; }

    public Vector2 Direction { 
        get 
        {
            if (!IsPressed)
            {
                direction = Vector2.zero;
            }
            return direction;
        }
        private set => direction = value;
    }
    private Vector2 direction;

    private int pointerId;
    private Vector2 pointerOld;

    private void Update()
    {
        if (!IsEnable) return;
        if (IsPressed)
        {
            Vector2 position;
            if (pointerId >= 0 && pointerId < Input.touches.Length)
            {
                position = Input.touches[pointerId].position;
            }
            else
            {
                position = Input.mousePosition;
            }

            Direction = (position - pointerOld).normalized;
            pointerOld = position;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsEnable) return;

        IsPressed = true;

        pointerId = eventData.pointerId;
        pointerOld = eventData.position;
    }

	public void OnPointerUp(PointerEventData eventData)
	{
        IsPressed = false;

        if (!IsEnable) return;
    }
}