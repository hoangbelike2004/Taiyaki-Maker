using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public List<PlayerData> playerDatas = new List<PlayerData>();

    public void SetPlayerData()
    {
        for (int i = playerDatas.Count - 1; i >= 0; i--)
        {
            if (playerDatas[i].playerState == PlayerState.Complete)
            {
                playerDatas[i + 1].playerState = PlayerState.Open;
            }
        }
    }

    public PlayerData GetPlayerData()
    {
        for (int i = 0; i < playerDatas.Count; i++)
        {
            if (playerDatas[i].playerState == PlayerState.Open)
            {
                return playerDatas[i];
            }
        }
        return null;
    }
}
