using UnityEngine;
using UnityEngine.EventSystems;

public class UIClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Animator anim;
    [SerializeField] private float timeClicked;
    int currentClick = 0;

    bool isClick = true;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OvenController.Instance.Stage == CakeProcessStage.Baking && isClick)
        {
            isClick = false;
            currentClick++;
            ChangeAnimation(currentClick);
            if (currentClick >= 5)
            {
                Invoke(nameof(CompleteBaking), timeClicked + 1);
            }
            else Invoke(nameof(SetIsClick), timeClicked);
        }
    }

    public void ChangeAnimation(int value)
    {
        anim.SetInteger(GameConstants.KEY_ANIMATION, value);
    }

    public void SetIsClick()
    {
        isClick = true;
    }

    public void CompleteBaking()
    {
        currentClick = 0;
        isClick = true;
        Observer.OnChangeStage?.Invoke();
    }
}
