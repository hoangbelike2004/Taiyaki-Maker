using UnityEngine;

public class GameController : Singleton<GameController>
{

    void Start()
    {
        UIManager.Instance.OpenUI<CanvasLiveStream>();
    }
    public void StartBaking()
    {
        UIManager.Instance.OpenUI<CanvasBaking>();
        Observer.OnChangeStage?.Invoke();
        UIManager.Instance.CloseUI<CanvasLiveStream>(0);
    }

    public void EndBaking()
    {
        UIManager.Instance.CloseUI<CanvasBaking>(0);
        UIManager.Instance.OpenUI<CanvasLiveStream>();
    }
}
