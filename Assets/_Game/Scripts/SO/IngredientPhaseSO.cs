using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientPhaseSO", menuName = "Scriptable Objects/IngredientPhaseSO")]
public class IngredientPhaseSO : ScriptableObject
{
    public List<IngredientPhase> ingredientPhases = new List<IngredientPhase>();

    public List<FillingPart> fillingParts = new List<FillingPart>();

    public IngredientPhase GetIngredientPhaseByType(IngredientPhaseType type)
    {
        return ingredientPhases.Find(t => t.phaseType == type);
    }

    public IngredientPhasePrefab GetIngredientPhasePrefab(IngredientPhaseType type, CakeMoldType cakeMoldType)
    {
        FillingPart fillingPart = fillingParts.Find(x => x.typeMold == cakeMoldType && x.phaseType == type);
        return fillingPart.prefab;
    }
}
