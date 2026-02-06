using UnityEngine;
using UnityEngine.UI;

public class CanvasBaking : UIManager
{
    [SerializeField] Button btnNextChoose;
    [SerializeField] RectTransform rectChooseMold, rectIngredient, recAdditionTiming;

    private Transform tfOverlaybtnNext;
    private bool isNext = false;
    void Awake()
    {
        tfOverlaybtnNext = btnNextChoose.transform.GetChild(0);
    }
    void Start()
    {
        btnNextChoose.onClick.AddListener(() =>
        {
            if (!isNext) return;
            Observer.OnChangeStage?.Invoke();
            Observer.OnEndStateChooseMold?.Invoke();
            DeactiveChooseMold();
        });
    }
    public void DeactiveOverlayBtnNext()
    {
        if (tfOverlaybtnNext.gameObject.activeSelf)
        {
            tfOverlaybtnNext.gameObject.SetActive(false);
            isNext = true;
        }
    }
    void DeactiveChooseMold()
    {
        rectChooseMold.gameObject.SetActive(false);
    }

    public void ActiveChooseMold()
    {
        tfOverlaybtnNext.gameObject.SetActive(true);
        isNext = false;
        rectChooseMold.gameObject.SetActive(true);
    }


    //Ingredient
    public void ActiveIgredient()
    {
        rectIngredient.gameObject.SetActive(true);
    }

    public void DeactiveIgredient()
    {
        rectIngredient.gameObject.SetActive(false);
    }
}
