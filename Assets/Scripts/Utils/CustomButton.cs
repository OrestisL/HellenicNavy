using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onClickDown = new UnityEvent();
    public UnityEvent onClickUp = new UnityEvent();
    public UnityEvent onEnter = new UnityEvent();
    public UnityEvent onExit = new UnityEvent();
    public float delay = 0.5f;
    private float m_lastPressTime = 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_lastPressTime + delay > Time.unscaledTime)
        {
            onClickDown?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_lastPressTime + delay > Time.unscaledTime)
        {
            onClickUp?.Invoke();
        }
    }
}
