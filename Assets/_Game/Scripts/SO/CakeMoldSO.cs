using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CakeMoldSO", menuName = "Scriptable Objects/CakeMoldSO")]
public class CakeMoldSO : ScriptableObject
{
    public List<CakeMoldNormal> cakeMoldNormals = new List<CakeMoldNormal>();

    public List<CakeMoldADS> cakeMoldADSs = new List<CakeMoldADS>();
}
