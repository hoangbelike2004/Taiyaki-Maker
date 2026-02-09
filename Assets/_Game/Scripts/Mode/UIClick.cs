using UnityEngine;
using UnityEngine.EventSystems;

public class UIClick : MonoBehaviour, IPointerClickHandler
{
    int currentClick = 0;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OvenController.Instance.Stage == CakeProcessStage.Baking)
        {
            currentClick++;
            if (currentClick == 1)
            {
                Observer.OnDespawnCakeMoldPrefab?.Invoke();
            }
            if (currentClick >= 5)
            {
                Observer.OnChangeStage?.Invoke();
                currentClick = 0;
            }
        }
    }
}
