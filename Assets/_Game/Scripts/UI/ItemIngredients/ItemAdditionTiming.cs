using UnityEngine;
using UnityEngine.UI;

public class ItemAdditionTiming : GameUnit
{
    [SerializeField] Image iconIngredient;

    [SerializeField] Text txtPrice;
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

    }
}
