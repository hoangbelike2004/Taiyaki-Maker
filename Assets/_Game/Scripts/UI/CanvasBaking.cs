using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBaking : UICanvas
{
    [SerializeField] Button btnNextChoose, btnNextAddition, btnNextToMukbang;
    [SerializeField] RectTransform rectChooseMold, rectIngredient, recAdditionTiming, rectGriller, rectShowComplete, rectMukBang;
    [SerializeField] List<RectTransform> rectCakes = new List<RectTransform>();

    [SerializeField] Image iconCakeOfShowComplete;
    private Transform tfOverlaybtnNextChoose, tfOverlaybtnNextAddition;
    private bool isNext = false;
    void Awake()
    {
        tfOverlaybtnNextChoose = btnNextChoose.transform.GetChild(0);
        tfOverlaybtnNextAddition = btnNextAddition.transform.GetChild(0);
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
        btnNextAddition.onClick.AddListener(() =>
        {
            if (!isNext) return;
            Observer.OnChangeStage?.Invoke();
            Observer.OnEndStateAdditionTiming?.Invoke();
            DeactiveAdditionTiming();
        });
        btnNextToMukbang.onClick.AddListener(() =>
        {
            rectShowComplete.gameObject.SetActive(false);
            ChangeMukbang(true);
        });
    }
    public void DeactiveOverlayBtnNext()
    {
        if (tfOverlaybtnNextChoose.gameObject.activeSelf)
        {
            tfOverlaybtnNextChoose.gameObject.SetActive(false);
            isNext = true;
        }
    }
    void DeactiveChooseMold()
    {
        rectChooseMold.gameObject.SetActive(false);
    }

    public void ActiveChooseMold()
    {
        rectGriller.gameObject.SetActive(true);
        tfOverlaybtnNextChoose.gameObject.SetActive(true);
        isNext = false;
        rectChooseMold.gameObject.SetActive(true);
    }

    public void DeactiveGriller(Sprite sprite)
    {
        rectGriller.gameObject.SetActive(false);
        ChangeShowComplete(sprite);
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

    //AdditionTiming

    public void ActiveAdditionTiming()
    {
        tfOverlaybtnNextAddition.gameObject.SetActive(true);
        isNext = false;
        recAdditionTiming.gameObject.SetActive(true);
    }

    public void DeactiveAdditionTiming()
    {
        recAdditionTiming.gameObject.SetActive(false);
    }
    public void DeactiveOverlayBtnNextAddition()
    {
        if (tfOverlaybtnNextAddition.gameObject.activeSelf)
        {
            tfOverlaybtnNextAddition.gameObject.SetActive(false);
            isNext = true;
        }
    }

    //Show Complete
    public void ChangeShowComplete(Sprite sprite)
    {
        rectShowComplete.gameObject.SetActive(true);
        iconCakeOfShowComplete.sprite = sprite;
    }
    //Show Mukbang

    public void ChangeMukbang(bool isActive)
    {
        rectMukBang.gameObject.SetActive(isActive);
    }

    public void InitMukbang(List<UIDraggable> uIDraggables)
    {
        for (int i = 0; i < uIDraggables.Count; i++)
        {
            uIDraggables[i].transform.SetParent(rectMukBang.transform);
            uIDraggables[i].Setparent(rectCakes[i]);
        }
    }
}
