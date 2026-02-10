using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public void DeactiveRight()
    {
        Observer.OnDespawnCakeMoldPrefab?.Invoke(false);
    }

    public void DeactiveLeft()
    {
        Observer.OnDespawnCakeMoldPrefab?.Invoke(true);
    }

    public void ActiveRight()
    {
        Observer.OnActiveCakeMoldPrefab?.Invoke(false);
    }

    public void ActiveLeft()
    {
        Observer.OnActiveCakeMoldPrefab?.Invoke(true);
    }
}
