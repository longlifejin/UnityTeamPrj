using System;
using UnityEngine;

[Serializable]
public class HitZoneCell
{
    public bool isActive;
}

[Serializable]
public class HitZoneRow
{
    public HitZoneCell[] cells = new HitZoneCell[4];
}