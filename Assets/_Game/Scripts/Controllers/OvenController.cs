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
public class OvenController : Singleton<OvenController>
{
    [SerializeField] private Transform tfParentCakeMold, tfParentAdditionTimming, tfParentIngredient;

    [SerializeField] RectTransform rectMold;//rect cua cai lo

    public CakeProcessStage Stage => currentState;

    private CakeMoldSO cakeMoldSO;

    private AdditionTimingSO additionTimingSO;

    [Tooltip("Nhan banh")]
    private IngredientPhaseSO ingredientPhaseSO;

    private Dictionary<IngredientPhaseType, ItemIngredientPhase> dicItemgredients = new Dictionary<IngredientPhaseType, ItemIngredientPhase>();

    private IngredientPhase ingredientPhase;

    private IngredientPhasePrefab ingredientPhasePrefab;

    [Tooltip("Trang tri banh")]

    private AdditionTiming additionTiming;

    private Dictionary<AdditionTimingType, ItemAdditionTiming> itemAdditionTimings = new Dictionary<AdditionTimingType, ItemAdditionTiming>();

    [Tooltip("Khuon banh")]

    private CakeMoldBase cakeMoldBase;//save the sellected value CakeMold(Khuôn bánh)

    private CakeMoldPrefab cakeMoldPrefab;//Khuon

    private List<ItemCakeMold> itemCakeMolds = new List<ItemCakeMold>();//dung de quan ly cac Item khuon banh

    private CakeProcessStage currentState;

    private int currentSteps = 0;

    private CanvasBaking canvasBaking;

    private CakeFinishSO cakeFinishSO;

    private CakeBase cakeBase;

    private List<UIDraggable> uIDraggables = new List<UIDraggable>();
    void Awake()
    {
        canvasBaking = GetComponent<CanvasBaking>();
        cakeMoldSO = Resources.Load<CakeMoldSO>(GameConstants.KEY_DATA_GAME_CAKE_MOLD);
        additionTimingSO = Resources.Load<AdditionTimingSO>(GameConstants.KEY_DATA_GAME_ADDITION_TIMING);
        ingredientPhaseSO = Resources.Load<IngredientPhaseSO>(GameConstants.KEY_DATA_GAME_INGREDIENT_PHASE);
        cakeFinishSO = Resources.Load<CakeFinishSO>(GameConstants.KEY_DATA_GAME_CAKE_FINISH);
        //goi ham instance ra các khuôn
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
                canvasBaking.ActiveIgredient();
                InstanceIngredientPhase();
                break;
            case CakeProcessStage.PouringTopLayer:
                cakeMoldPrefab.ActivePouringTopLayer();
                break;
            case CakeProcessStage.Baking:
                break;
            case CakeProcessStage.Decorating:
                canvasBaking.ActiveAdditionTiming();
                InstancAdditionTiming();
                break;
            case CakeProcessStage.Completed:
                Complete();
                break;
        }

    }
    //khởi tạo các item khuôn bánh
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
            if (ingredientPhasePrefab != null) Destroy(ingredientPhasePrefab.gameObject);
            //SimplePool.Despawn(ingredientPhasePrefab);
            this.ingredientPhase = null;
        }
        this.ingredientPhase = ingredientPhase;
        // ingredientPhasePrefab = SimplePool.Spawn<IngredientPhasePrefab>(ingredientPhase.typePrefab, Vector3.zero, Quaternion.identity);
        ingredientPhasePrefab = Instantiate(ingredientPhaseSO.GetIngredientPhasePrefab(ingredientPhase.phaseType, cakeMoldBase.moldType));
        ingredientPhasePrefab.SetParent(cakeMoldPrefab.posIngredient);
    }


    //khởi tạo các item nhân bánh
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

    //dung de deactive khi chay animation Cover
    void DespawnFlour(bool isleft)
    {
        cakeMoldPrefab.DeactivePouringLayer(isleft);
        cakeMoldPrefab.DeactiveModlCake(isleft);
        if (ingredientPhasePrefab != null && isleft) Destroy(ingredientPhasePrefab.gameObject);
    }


    //dung de active khi chay animation Open
    public void ActiveMold(bool isleft)
    {
        cakeMoldPrefab.ActiveMoldCake(isleft);
        if (isleft)
        {
            AdditionTimingType additionTimingType = additionTiming == null ? AdditionTimingType.None : additionTiming.timingType;

            IngredientPhaseType ingredientPhaseType = ingredientPhase == null ? IngredientPhaseType.None : ingredientPhase.phaseType;

            cakeBase = cakeFinishSO.GetCakeBaseWhenNormal(cakeMoldBase.moldType, ingredientPhaseType);

            cakeMoldPrefab.SetCakeFinished(cakeBase.iconCakeMold);
        }
    }


    //khởi tạo phần trang trí
    void InstancAdditionTiming()
    {
        CakeMoldADS cakeMoldADS = cakeMoldBase as CakeMoldADS;
        if (cakeMoldADS == null) return;
        if (itemAdditionTimings.Count == 0)
        {
            for (int i = 0; i < additionTimingSO.additionTimings.Count; i++)
            {
                ItemAdditionTiming item = SimplePool.Spawn<ItemAdditionTiming>(PoolType.Item_Addition_Timing, Vector3.zero, Quaternion.identity);
                item.SetData(additionTimingSO.additionTimings[i], tfParentAdditionTimming);
                itemAdditionTimings.Add(additionTimingSO.additionTimings[i].timingType, item);
            }
        }
        if (itemAdditionTimings.Count != 0)
        {
            for (int i = 0; i < cakeMoldADS.additionTimingTypes.Count; i++)
            {
                itemAdditionTimings[cakeMoldADS.additionTimingTypes[i]].gameObject.SetActive(true);
            }
        }
    }

    void SellectAdditionTiming(AdditionTiming additionTiming)
    {
        this.additionTiming = additionTiming;
        canvasBaking.DeactiveOverlayBtnNextAddition();
    }

    void DeactiveAdditionTiming()
    {
        CakeMoldADS cakeMoldADS = cakeMoldBase as CakeMoldADS;
        if (cakeMoldADS == null) return;
        for (int i = 0; i < cakeMoldADS.additionTimingTypes.Count; i++)
        {
            itemAdditionTimings[cakeMoldADS.additionTimingTypes[i]].gameObject.SetActive(false);
        }
    }

    void Complete()
    {
        if (additionTiming != null)
        {
            AdditionTimingType additionTimingType = additionTiming == null ? AdditionTimingType.None : additionTiming.timingType;

            IngredientPhaseType ingredientPhaseType = ingredientPhase == null ? IngredientPhaseType.None : ingredientPhase.phaseType;

            cakeBase = cakeFinishSO.GetCakeBaseWhenAdditionTiming(cakeMoldBase.moldType, additionTimingType, ingredientPhaseType);
        }
        canvasBaking.DeactiveGriller(cakeBase.iconCake);
        UIDraggable uIDraggable = cakeBase.prefab;
        for (int i = 0; i < 5; i++)
        {
            UIDraggable draggable = Instantiate(uIDraggable);
            uIDraggables.Add(draggable);
        }
        canvasBaking.InitMukbang(uIDraggables);
    }
    void RemoveUIDraggableWhenMukbang(UIDraggable uIDraggable)
    {
        if (uIDraggables.Count == 0) return;
        if (uIDraggables.Contains(uIDraggable))
        {
            uIDraggables.Remove(uIDraggable);
            if (uIDraggables.Count == 0)
            {
                MukbangComplete();
                canvasBaking.ChangeMukbang(false);
                GameController.Instance.EndBaking();
            }
        }

    }

    public void MukbangComplete()
    {
        cakeMoldBase = null;
        additionTiming = null;
        ingredientPhase = null;
        SimplePool.Despawn(cakeMoldPrefab);
        cakeMoldPrefab = null;
        currentSteps = 0;
        currentState = CakeProcessStage.None;
    }
    void OnEnable()
    {
        Observer.OnSellectedCakeMold += SellectCakeMold;
        Observer.OnSellectIngredientPhase += SellectIngredientPhase;
        Observer.OnSellectAdditionTiming += SellectAdditionTiming;
        Observer.OnDeactiveItemAddingFilling += DeactiveIngredientPhase;
        Observer.OnChangeStage += ChangeState;
        Observer.OnEndStateChooseMold += DeactiveCakeMold;
        Observer.OnDespawnCakeMoldPrefab += DespawnFlour;
        Observer.OnEndStateAdditionTiming += DeactiveAdditionTiming;
        Observer.OnActiveCakeMoldPrefab += ActiveMold;
        Observer.OnRemoveDraggable += RemoveUIDraggableWhenMukbang;
    }

    void OnDisable()
    {
        Observer.OnSellectedCakeMold -= SellectCakeMold;
        Observer.OnSellectIngredientPhase -= SellectIngredientPhase;
        Observer.OnSellectAdditionTiming -= SellectAdditionTiming;
        Observer.OnDeactiveItemAddingFilling -= DeactiveIngredientPhase;
        Observer.OnChangeStage -= ChangeState;
        Observer.OnEndStateChooseMold -= DeactiveCakeMold;
        Observer.OnDespawnCakeMoldPrefab -= DespawnFlour;
        Observer.OnEndStateAdditionTiming -= DeactiveAdditionTiming;
        Observer.OnActiveCakeMoldPrefab -= ActiveMold;
        Observer.OnRemoveDraggable -= RemoveUIDraggableWhenMukbang;
    }
}
