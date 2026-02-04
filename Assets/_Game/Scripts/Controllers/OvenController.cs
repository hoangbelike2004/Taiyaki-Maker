using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OvenController : MonoBehaviour
{
    private CakeMoldSO cakeMoldSO;

    private Transform tfParentCakeMold;

    private CakeMoldBase cakeMoldBase;//save the sellected value CakeMold(Khuôn bánh)
    void Awake()
    {
        cakeMoldSO = Resources.Load<CakeMoldSO>(GameConstants.KEY_DATA_GAME_CAKE_MOLD);
        //goi ham instance ra các khuôn
    }
    //hàm nay gọi truoc InstanceCakeMold
    private void SetParentMold(Transform transform)
    {
        tfParentCakeMold = transform;
    }
    void InstanceCakeMold()
    {
        if (cakeMoldSO != null)
        {
            for (int i = 0; i < cakeMoldSO.cakeMoldNormals.Count; i++)
            {
                //instance
            }
            for (int i = 0; i < cakeMoldSO.cakeMoldADSs.Count; i++)
            {
                //instance
            }
        }
    }
    void SellectCakeMold(CakeMoldBase cakeMoldBase)
    {
        this.cakeMoldBase = cakeMoldBase;
    }

    void OnEnable()
    {
        Observer.OnSellectedCakeMold += SellectCakeMold;
    }

    void OnDisable()
    {
        Observer.OnSellectedCakeMold -= SellectCakeMold;
    }
}
