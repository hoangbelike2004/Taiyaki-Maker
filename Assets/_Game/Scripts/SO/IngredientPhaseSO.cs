using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientPhaseSO", menuName = "Scriptable Objects/IngredientPhaseSO")]
public class IngredientPhaseSO : ScriptableObject
{
    public List<IngredientPhase> ingredientPhases = new List<IngredientPhase>();

    public IngredientPhase GetIngredientPhaseByType(IngredientPhaseType type)
    {
        return ingredientPhases.Find(t => t.phaseType == type);
    }
}
