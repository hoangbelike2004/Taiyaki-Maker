using UnityEngine;
using UnityEngine.UI;

public class CakeMoldPrefab : GameUnit
{
    public Transform posIngredient;
    [SerializeField] RectTransform rectPouringBottomLayer, rectPouringTopLayer, rectMoldLeft, rectMoldRight;

    private Image imageCake;//compoment để chứa thành phẩm khi ra lò
    private RectTransform rect;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (rectMoldRight != null)
        {
            imageCake = rectMoldRight.GetChild(0).GetComponent<Image>();
        }
        else
        {
            imageCake = rectMoldLeft.GetChild(0).GetComponent<Image>();
        }
    }
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localScale = Vector3.one;
        rect.anchoredPosition = Vector3.zero;
    }
    public void ActivePouringBottomLayer()
    {
        if (rectPouringBottomLayer != null)
        {
            rectPouringBottomLayer.gameObject.SetActive(true);
        }
    }

    public void ActivePouringTopLayer()
    {
        if (rectPouringTopLayer != null)
        {
            rectPouringTopLayer.gameObject.SetActive(true);
        }
    }
    public void DeactivePouringBottomLayer()
    {
        if (rectPouringBottomLayer != null) rectPouringBottomLayer.GetComponent<Image>().enabled = false;
    }

    public void DeactivePouringTopLayer()
    {
        if (rectPouringTopLayer != null) rectPouringTopLayer.GetComponent<Image>().enabled = false;
    }
    // khi đây nắp lại thì deactive bọn này đi và reset lại như cũ
    public void DeactivePouringLayer(bool isleft)
    {
        if (rectPouringBottomLayer != null && isleft)
        {
            rectPouringBottomLayer.GetComponent<Image>().enabled = true;
            rectPouringBottomLayer.gameObject.SetActive(false);
        }
        if (rectPouringTopLayer != null && !isleft)
        {
            rectPouringTopLayer.GetComponent<Image>().enabled = true;
            rectPouringTopLayer.gameObject.SetActive(false);
        }
    }
    public void DeactiveModlCake(bool isleft)
    {
        if (rectMoldLeft != null && isleft) rectMoldLeft.gameObject.SetActive(false);
        else if (rectMoldRight != null && !isleft) rectMoldRight.gameObject.SetActive(false);
    }
    public void ActiveMoldCake(bool isleft)
    {
        if (rectMoldLeft != null && isleft)
        {
            rectMoldLeft.gameObject.SetActive(true);
        }
        if (rectMoldRight != null && !isleft)
        {
            rectMoldRight.gameObject.SetActive(true);
        }
    }

    public void SetCakeFinished(Sprite sprite)
    {
        imageCake.gameObject.SetActive(true);
        imageCake.sprite = sprite;
    }
    void OnEnable()
    {
        Observer.OnEndPouringTopLayer += DeactivePouringTopLayer;
        Observer.OnEndPouringBottomLayer += DeactivePouringBottomLayer;
    }
    void OnDisable()
    {
        Observer.OnEndPouringTopLayer -= DeactivePouringTopLayer;
        Observer.OnEndPouringBottomLayer -= DeactivePouringBottomLayer;
        if (imageCake != null) imageCake.gameObject.SetActive(false);
        DeactivePouringLayer(true);
        DeactivePouringLayer(false);
    }
}
