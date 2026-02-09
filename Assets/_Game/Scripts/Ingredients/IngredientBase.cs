using System;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBase
{
    public Sprite icon;

    public bool isAds = false;

    public float price;

    public PoolType typePrefab;
}
[System.Serializable]
public class AdditionTiming : IngredientBase//Trang tri
{
    public AdditionTimingType timingType;
}
[Serializable]
public struct FillingPart
{
    public IngredientPhaseType phaseType;
    
    public CakeMoldType typeMold;

    public IngredientPhasePrefab prefab;
}
[System.Serializable]
public class IngredientPhase : IngredientBase//Nhan banh
{
    public IngredientPhaseType phaseType;
}
