using UnityEngine;
using UnityEngine.UI;

public class ItemCakeMold : GameUnit
{
    [SerializeField] Image iconMold;
    protected Button btnClick;

    protected CakeMoldBase cakeMoldBase;

    public virtual void SetData(CakeMoldBase cakeMoldBase, Transform parent)
    {
        this.cakeMoldBase = cakeMoldBase;
        iconMold.sprite = cakeMoldBase.icon;
        transform.SetParent(parent);
        transform.localScale = Vector3.one;
    }

    void Start()
    {
        btnClick = GetComponent<Button>();
        btnClick.onClick.AddListener(() =>
        {
            Observer.OnSellectedCakeMold?.Invoke(cakeMoldBase);
            //Xu ly logic chon khuon
        });
    }
}
