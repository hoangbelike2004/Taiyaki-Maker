using UnityEngine;
using UnityEngine.UI;
public class ItemIngredientPhase : GameUnit
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
    }

    void Start()
    {
        btnClick = GetComponent<Button>();
        btnClick.onClick.AddListener(() =>
        {
            //Xu ly logic chon khuon
        });
    }
}
