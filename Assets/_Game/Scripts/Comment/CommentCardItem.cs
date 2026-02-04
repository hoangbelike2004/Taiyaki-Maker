using System;
using System.Collections.Generic;
using UnityEngine;


public enum CommentCardType
{
    None,
}
[Serializable]
public class CommentCardItem
{
    public CommentCardType type = CommentCardType.None;

    public Sprite icon;
    public List<string> cmts = new List<string>();
}

