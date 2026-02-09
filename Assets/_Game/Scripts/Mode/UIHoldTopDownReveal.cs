using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoldTopDownReveal : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private UITopDownReveal uITopDownReveal;

    void Awake()
    {
        uITopDownReveal = transform.parent.GetComponent<UITopDownReveal>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (uITopDownReveal != null)
        {
            Observer.OnDeactiveItemAddingFilling?.Invoke();
            uITopDownReveal.StartRevealProcess();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (uITopDownReveal != null)
        {
            uITopDownReveal.EndRevealProcess();
        }
    }
}
