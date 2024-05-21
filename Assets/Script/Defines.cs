using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stage
{
    first = 3001,
    second, 
    third, 
    fourth, 
    fifth, 
    sixth, 
    seventh, 
    eightth
}

public static class DataTableIds
{
    public static readonly string PlayerTable = "PlayerDataTable";
    public static readonly string BossTable = "BossDataTable";
    public static readonly string StageTable = "StageDataTable";
    public static readonly string ItemTable = "ItemDataTable";
    public static readonly string String = "StringDataTable";
    public static readonly string playerID = "1001";
    public static string stageID = "3001";

}

public static class AnimatorIds
{
    public static readonly string playerChargingAni = "PlayerCharging";
    public static readonly string playerAtkAni = "PlayerAttack";
    public static readonly string playerDamagedAni = "PlayerDamaged";
    public static readonly string playerDieAni = "PlayerDie";
    public static readonly string playerIdleAni = "PlayerIdle";
    public static readonly string bossAtkAni = "BossAttack";
    public static readonly string bossSpecialAtkAni = "BossSpecialAttack";
    public static readonly string bossDamagedAni = "BossDamaged";
    public static readonly string bossDiedAni = "BossDie";
    public static readonly string bossIdledAni = "BossIdle";
}