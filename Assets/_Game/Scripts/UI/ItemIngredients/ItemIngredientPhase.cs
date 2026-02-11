using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ItemIngredientPhase : GameUnit
{
    [SerializeField] Image iconIngredient;

    [SerializeField] TextMeshProUGUI txtPrice;
    protected Button btnClick;

    protected IngredientPhase ingredientPhase;

    public virtual void SetData(IngredientPhase ingredientPhase, Transform parent)
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

        transform.localScale = Vector3.one;
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
        Observer.OnSellectIngredientPhase?.Invoke(ingredientPhase);
    }
}
