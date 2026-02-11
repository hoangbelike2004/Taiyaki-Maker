using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComentSO", menuName = "Scriptable Objects/ComentSO")]
public class ComentSO : ScriptableObject
{
    public List<CommentCardItem> commentCards = new List<CommentCardItem>();

    public CommentCardItem GetCommentCardItem()
    {
        int rnd = Random.Range(0, commentCards.Count);
        return commentCards[rnd];
    }
}
