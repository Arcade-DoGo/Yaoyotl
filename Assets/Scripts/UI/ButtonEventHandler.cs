using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;
public class ButtonEventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;
    public void OnPointerDown(PointerEventData eventData) => onPointerDown?.Invoke();
    public void OnPointerUp(PointerEventData eventData) => onPointerUp?.Invoke();
}