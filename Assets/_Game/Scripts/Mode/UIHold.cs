using UnityEngine;
using UnityEngine.EventSystems;

public class UIHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private UIRadialReveal uIRadialReveal;
    void Awake()
    {
        uIRadialReveal = transform.GetChild(0).GetComponent<UIRadialReveal>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (uIRadialReveal != null)
        {
            uIRadialReveal.StarPouring();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (uIRadialReveal != null)
        {
            uIRadialReveal.EndPouring();
        }
    }
}
