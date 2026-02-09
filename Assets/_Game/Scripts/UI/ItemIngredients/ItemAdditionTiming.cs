using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemAdditionTiming : GameUnit
{
    [SerializeField] Image iconIngredient;

    [SerializeField] TextMeshProUGUI txtPrice;
    protected Button btnClick;

    protected AdditionTiming ingredientPhase;

    public virtual void SetData(AdditionTiming ingredientPhase, Transform parent)
    {
        this.ingredientPhase = ingredientPhase;
        iconIngredient.sprite = ingredientPhase.icon;
        transform.SetParent(parent);
        if (ingredientPhase.price != 0)
        {
            txtPrice.gameObject.SetActive(true);
            txtPrice.text = ingredientPhase.price.ToString();
        }
        else
        {
            txtPrice.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    void Start()
    {
        btnClick = GetComponent<Button>();
        btnClick.onClick.AddListener(() =>
        {
            OnClickItem();
        });
    }

    public virtual void OnClickItem()
    {
        Observer.OnSellectAdditionTiming?.Invoke(ingredientPhase);
    }
}
