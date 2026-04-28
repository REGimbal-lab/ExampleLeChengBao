using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Events")]
    public UnityEvent onClick;
    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;
    public UnityEvent onHoverEnter;
    public UnityEvent onHoverExit;

    // 点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    // 按下事件
    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke();
    }

    // 松开事件
    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke();
    }

    // 鼠标悬停进入事件
    public void OnPointerEnter(PointerEventData eventData)
    {
        onHoverEnter?.Invoke();
    }

    // 鼠标悬停离开事件
    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit?.Invoke();
    }

    public void SetClicked(bool clicked)
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("Clicked", clicked);
        }
    }
}
