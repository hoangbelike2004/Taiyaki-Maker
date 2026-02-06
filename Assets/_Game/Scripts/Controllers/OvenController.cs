using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public enum CakeProcessStage
{
    None,               // Chưa làm gì
    ChooseCake,         // Chọn loại bánh
    PouringBottomLayer, // Đổ bột lần 1 (Lớp đáy)
    AddingFilling,      // Thêm nhân
    PouringTopLayer,    // Đổ bột lần 2 (Lớp phủ)
    Baking,             // Nướng
    Decorating,         // Trang trí
    Completed           // Hoàn thành
}
public class OvenController : MonoBehaviour
{
    [SerializeField] private Transform tfParentCakeMold, tfParentAdditionTimming, tfParentIngredient;

    [SerializeField] RectTransform rectMold, rectIngredient;//rect cua cai lo

    private CakeMoldSO cakeMoldSO;

    private AdditionTimingSO additionTimingSO;

    private IngredientPhaseSO ingredientPhaseSO;

    private Dictionary<IngredientPhaseType, ItemIngredientPhase> dicItemgredients = new Dictionary<IngredientPhaseType, ItemIngredientPhase>();

    private IngredientPhase ingredientPhase;

    private IngredientPhasePrefab ingredientPhasePrefab;

    private CakeMoldBase cakeMoldBase;//save the sellected value CakeMold(Khuôn bánh)

    private CakeMoldPrefab cakeMoldPrefab;//Khuon

    private List<ItemCakeMold> itemCakeMolds = new List<ItemCakeMold>();//dung de quan ly cac Item khuon banh

    private IngredientPhaseType ingredientPhaseType;//save the sellected value cake

    private AdditionTimingType additionTimingType;//save the sellected value decorating(nếu có)

    private CakeProcessStage currentState;

    private int currentSteps = 0;

    private CanvasBaking canvasBaking;
    void Awake()
    {
        canvasBaking = GetComponent<CanvasBaking>();
        cakeMoldSO = Resources.Load<CakeMoldSO>(GameConstants.KEY_DATA_GAME_CAKE_MOLD);
        additionTimingSO = Resources.Load<AdditionTimingSO>(GameConstants.KEY_DATA_GAME_ADDITION_TIMING);
        ingredientPhaseSO = Resources.Load<IngredientPhaseSO>(GameConstants.KEY_DATA_GAME_INGREDIENT_PHASE);
        //goi ham instance ra các khuôn
    }
    void Start()
    {
        ChangeState();
    }
    void ChangeState()
    {
        if (cakeMoldBase == null) currentState = CakeProcessStage.ChooseCake;
        else
        {
            currentState = cakeMoldBase.steps[currentSteps];
            currentSteps++;
        }
        switch (currentState)
        {
            case CakeProcessStage.ChooseCake:
                canvasBaking.ActiveChooseMold();
                InstanceCakeMold();
                break;
            case CakeProcessStage.PouringBottomLayer:
                cakeMoldPrefab.ActivePouringBottomLayer();
                break;
            case CakeProcessStage.AddingFilling:
                cakeMoldPrefab.DeactivePouringBottomLayer();
                canvasBaking.ActiveIgredient();
                InstanceIngredientPhase();
                break;
            case CakeProcessStage.PouringTopLayer:
                break;
            case CakeProcessStage.Baking:
                break;
            case CakeProcessStage.Decorating:
                break;
            case CakeProcessStage.Completed:
                break;
        }

    }
    void InstanceCakeMold()
    {
        if (cakeMoldSO != null)
        {
            if (itemCakeMolds.Count == 0)
            {
                for (int i = 0; i < cakeMoldSO.cakeMoldNormals.Count; i++)
                {
                    ItemCakeMold item = SimplePool.Spawn<ItemCakeMold>(PoolType.Item_Cake_Mold, Vector3.zero, Quaternion.identity);
                    item.SetData(cakeMoldSO.cakeMoldNormals[i], tfParentCakeMold);
                    itemCakeMolds.Add(item);
                }
                for (int i = 0; i < cakeMoldSO.cakeMoldADSs.Count; i++)
                {
                    ItemCakeMoldAds item = SimplePool.Spawn<ItemCakeMoldAds>(PoolType.Item_Cake_Mold_Ads, Vector3.zero, Quaternion.identity);
                    item.SetData(cakeMoldSO.cakeMoldADSs[i], tfParentCakeMold);
                    itemCakeMolds.Add(item);
                }
            }
            else
            {
                for (int i = 0; i < itemCakeMolds.Count; i++)
                {
                    itemCakeMolds[i].gameObject.SetActive(true);
                }
            }
        }
    }

    void DeactiveCakeMold()
    {
        for (int i = 0; i < itemCakeMolds.Count; i++)
        {
            itemCakeMolds[i].gameObject.SetActive(false);
        }
    }
    void SellectCakeMold(CakeMoldBase cakeMoldBase)//chon khuon banh
    {
        if (this.cakeMoldBase != null)
        {
            SimplePool.Despawn(cakeMoldPrefab);
            this.cakeMoldBase = null;
        }
        this.cakeMoldBase = cakeMoldBase;
        cakeMoldPrefab = SimplePool.Spawn<CakeMoldPrefab>(cakeMoldBase.prefabType, Vector3.zero, Quaternion.identity);
        cakeMoldPrefab.SetParent(rectMold);
        canvasBaking.DeactiveOverlayBtnNext();
    }

    void SellectIngredientPhase(IngredientPhase ingredientPhase)
    {
        if (this.ingredientPhase != null)
        {
            SimplePool.Despawn(ingredientPhasePrefab);
            this.ingredientPhase = null;
        }
        this.ingredientPhase = ingredientPhase;
        ingredientPhasePrefab = SimplePool.Spawn<IngredientPhasePrefab>(ingredientPhase.typePrefab, Vector3.zero, Quaternion.identity);
        ingredientPhasePrefab.SetParent(cakeMoldPrefab.posIngredient);
    }

    void InstanceIngredientPhase()
    {
        if (dicItemgredients.Count == 0)
        {
            for (int i = 0; i < ingredientPhaseSO.ingredientPhases.Count; i++)
            {
                ItemIngredientPhase item = SimplePool.Spawn<ItemIngredientPhase>(PoolType.Item_Ingredient_Phase, Vector3.zero, Quaternion.identity);
                item.SetData(ingredientPhaseSO.ingredientPhases[i], tfParentIngredient);
                dicItemgredients.Add(ingredientPhaseSO.ingredientPhases[i].phaseType, item);
                item.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < cakeMoldBase.ingredientTypes.Count; i++)
        {
            dicItemgredients[cakeMoldBase.ingredientTypes[i]].gameObject.SetActive(true);
        }
    }

    void DeactiveIngredientPhase()
    {
        for (int i = 0; i < cakeMoldBase.ingredientTypes.Count; i++)
        {
            dicItemgredients[cakeMoldBase.ingredientTypes[i]].gameObject.SetActive(false);
        }
        canvasBaking.DeactiveIgredient();
    }

    void InstancAdditionTiming()
    {
        CakeMoldADS cakeMoldADS = cakeMoldBase as CakeMoldADS;
        if (cakeMoldADS == null) return;
        for (int i = 0; i < cakeMoldADS.additionTimingTypes.Count; i++)
        {
            //khoi tạo những phan trang tri banh
        }
    }

    void DeactiveAdditionTiming()
    {
        CakeMoldADS cakeMoldADS = cakeMoldBase as CakeMoldADS;
        if (cakeMoldADS == null) return;
        for (int i = 0; i < cakeMoldADS.additionTimingTypes.Count; i++)
        {
            //khoi tạo những phan trang tri banh
        }
    }

    void Complete()
    {
        cakeMoldBase = null;
        cakeMoldPrefab = null;
        currentSteps = 0;
        currentState = CakeProcessStage.None;
    }
    void OnEnable()
    {
        Observer.OnSellectedCakeMold += SellectCakeMold;
        Observer.OnSellectIngredientPhase += SellectIngredientPhase;
        Observer.OnDeactiveAddingFilling += DeactiveIngredientPhase;
        Observer.OnChangeStage += ChangeState;
        Observer.OnEndStateChooseMold += DeactiveCakeMold;
    }

    void OnDisable()
    {
        Observer.OnSellectedCakeMold -= SellectCakeMold;
        Observer.OnSellectIngredientPhase -= SellectIngredientPhase;
        Observer.OnDeactiveAddingFilling -= DeactiveIngredientPhase;
        Observer.OnChangeStage -= ChangeState;
        Observer.OnEndStateChooseMold -= DeactiveCakeMold;
    }
}
