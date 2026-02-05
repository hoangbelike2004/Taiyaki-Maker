using UnityEngine;
using UnityEngine.UI;

public class CanvasBaking : UIManager
{
    [SerializeField] Button btnNextChoose;
    [SerializeField] RectTransform rectChooseMold;

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
    void OnEnable()
    {
        tfOverlaybtnNext.gameObject.SetActive(true);
        isNext = false;
    }
    void OnDisable()
    {

    }
}
