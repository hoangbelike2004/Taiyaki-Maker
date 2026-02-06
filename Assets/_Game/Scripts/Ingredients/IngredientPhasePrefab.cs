using UnityEngine;

public class IngredientPhasePrefab : GameUnit
{
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
}
