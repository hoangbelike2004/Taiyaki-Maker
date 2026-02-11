using System;
using System.Collections.Generic;
using UnityEngine;


public enum CommentCardType
{
    None,
    Cmt_1,
    Cmt_2,
    Cmt_3,
    Cmt_4,
    Cmt_5,
}
[Serializable]
public class CommentCardItem
{
    public CommentCardType type = CommentCardType.None;

    public Sprite icon;
    public List<string> cmts = new List<string>();
}

