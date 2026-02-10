using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CakeFinishSO", menuName = "Scriptable Objects/CakeFinishSO")]
public class CakeFinishSO : ScriptableObject
{
    public List<CakeBase> cakeBases = new List<CakeBase>();
    //lay loai banh khi chua trang tri
    public CakeBase GetCakeBaseWhenNormal(CakeMoldType cakeMoldType, IngredientPhaseType ingredientPhaseType)
    {
        return cakeBases.Find(x => x.moldType == cakeMoldType && x.ingredientPhaseType == ingredientPhaseType);
    }

    //lay banh khi da trang tri
    public CakeBase GetCakeBaseWhenAdditionTiming(CakeMoldType cakeMoldType, AdditionTimingType additionTimingType, IngredientPhaseType ingredientPhaseType)
    {
        return cakeBases.Find(x => x.moldType == cakeMoldType && x.additionTimingType == additionTimingType && x.ingredientPhaseType == ingredientPhaseType);
    }
}
