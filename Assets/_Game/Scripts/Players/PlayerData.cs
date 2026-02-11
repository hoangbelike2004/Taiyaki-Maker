using System;
using UnityEngine;


public enum PlayerType
{
    None,
    Character_1,
    Character_2,
}

public enum PlayerState
{
    Lock,
    Open,
    Complete,
}
[Serializable]
public class PlayerData
{
    public PlayerType type;

    public PlayerState playerState;
    public float bodyWeight;

    public PoolType typePrefab;
}
