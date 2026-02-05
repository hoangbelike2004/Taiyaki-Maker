using UnityEngine;
using UnityEngine.UI;

public class CanvasBaking : UIManager
{
    [SerializeField] Button btnNextChoose;
    [SerializeField] RectTransform rectChooseMold;

    void Start()
    {
        btnNextChoose.onClick.AddListener(() =>
        {
            Observer.OnChangeStage?.Invoke();
            DeactiveChooseMold();
        });
    }

    public void DeactiveChooseMold()
    {
        rectChooseMold.gameObject.SetActive(false);
    }
}
