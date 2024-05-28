using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveData
{
    public int Version { get; protected set; }

    public abstract SaveData VersionUp();
}

public class SaveDataV1 : SaveData
{
    public int gainedHp;
    public int gainedAtk;
    public int gainedCritical;
    public int gold;
    public int lastCleardStage;
    public int atkItemIndex;
    public int hpItemIndex;


    public SaveDataV1()
    {
        Version = 1;
    }

    public override SaveData VersionUp()
    {
        return null;
    }
}

