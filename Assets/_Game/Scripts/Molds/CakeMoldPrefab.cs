using UnityEngine;
using UnityEngine.UI;

public class CakeMoldPrefab : GameUnit
{
    public Transform posIngredient;
    [SerializeField] RectTransform rectPouringBottomLayer, rectPouringTopLayer;
    private RectTransform rect;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
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
    public void DeactivePouringLayer()
    {
        if (rectPouringBottomLayer != null)
        {
            rectPouringBottomLayer.GetComponent<Image>().enabled = true;
            rectPouringBottomLayer.gameObject.SetActive(false);
        }
        if (rectPouringTopLayer != null)
        {
            rectPouringTopLayer.GetComponent<Image>().enabled = true;
            rectPouringTopLayer.gameObject.SetActive(false);
        }
    }
    void OnDisable()
    {
        DeactivePouringLayer();
    }
}
