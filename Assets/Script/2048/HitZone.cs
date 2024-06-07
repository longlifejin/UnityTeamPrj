using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecificPosition", menuName = "SpecificPosition", order = 1) ]
public class HitZone : ScriptableObject
{
    public List<Vector2Int> specificPos;
    
}
