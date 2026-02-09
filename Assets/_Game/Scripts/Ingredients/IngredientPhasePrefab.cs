using UnityEngine;
using UnityEngine.UI;

public class IngredientPhasePrefab : MonoBehaviour
{
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        transform.GetChild(0).GetComponent<Image>().enabled = true;
        transform.localScale = Vector3.one;
    }

    public void Deactive()
    {
        transform.GetChild(0).GetComponent<Image>().enabled = false;
    }

    void OnEnable()
    {
        Observer.OnEndStateAddingFilling += Deactive;
    }
    void OnDisable()
    {
        Observer.OnEndStateAddingFilling -= Deactive;
    }
}
