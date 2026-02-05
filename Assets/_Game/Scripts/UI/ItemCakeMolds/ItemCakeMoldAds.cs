using UnityEngine;
using UnityEngine.UI;

public class ItemCakeMoldAds : ItemCakeMold
{
    [SerializeField] Image iconAds;

    public override void SetData(CakeMoldBase cakeMoldBase, Transform parent)
    {
        base.SetData(cakeMoldBase, parent);
        iconAds.gameObject.SetActive(true);
    }

    void Start()
    {
        btnClick = GetComponent<Button>();
        btnClick.onClick.AddListener(() =>
        {
            //bat xem quang cao moi mo
            Observer.OnSellectedCakeMold?.Invoke(cakeMoldBase);
            //Xu ly logic chon khuon
        });
    }
}
