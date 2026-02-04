using UnityEngine;

[CreateAssetMenu(fileName = "GameSetting", menuName = "Scriptable Objects/GameSetting")]
public class GameSetting : ScriptableObject
{
    public bool isMusic = true;

    public bool isSound = true;

    public bool isVibration = true;
}
