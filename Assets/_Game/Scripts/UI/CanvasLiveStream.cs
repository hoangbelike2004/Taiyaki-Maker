using UnityEngine;
using UnityEngine.UI;

public class CanvasLiveStream : UICanvas
{
    [SerializeField] Button btnNext;

    void Start()
    {
        btnNext.onClick.AddListener(() =>
        {
            GameController.Instance.StartBaking();
        });
    }
}
