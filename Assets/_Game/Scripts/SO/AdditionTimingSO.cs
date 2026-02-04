using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdditionTimingSO", menuName = "Scriptable Objects/AdditionTimingSO")]
public class AdditionTimingSO : ScriptableObject
{
    public List<AdditionTiming> additionTimings = new List<AdditionTiming>();

    public AdditionTiming GetAdditionTimingByType(AdditionTimingType type)
    {
        return additionTimings.Find(t => t.timingType == type);
    }
}
