using UnityEngine;
using UnityEngine.Events;

public static class Observer
{
    public static UnityAction<CakeMoldBase> OnSellectedCakeMold;

    public static UnityAction OnChangeStage;

    public static UnityAction OnEndStateChooseMold;

    public static UnityAction OnEndStateAdditionTiming;

    public static UnityAction OnEndStateAddingFilling;

    public static UnityAction OnEndPouringTopLayer;

    public static UnityAction OnEndPouringBottomLayer;

    public static UnityAction OnDeactiveItemAddingFilling;

    public static UnityAction<bool> OnDespawnCakeMoldPrefab;

    public static UnityAction<bool> OnActiveCakeMoldPrefab;//dung de active khuon khi chay animation;

    public static UnityAction<IngredientPhase> OnSellectIngredientPhase;

    public static UnityAction<AdditionTiming> OnSellectAdditionTiming;

    public static UnityAction<UIDraggable> OnRemoveDraggable;

    public static UnityAction<bool> OnDraggableCake;
}
