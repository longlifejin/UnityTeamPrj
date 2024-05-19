using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataTableMgr
{
    private static Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

    static DataTableMgr()
    {
        DataTable table = new StringTable();
        table.Load(DataTableIds.String);
        tables.Add(DataTableIds.String, table);

        PlayerDataTable playerDataTable = new PlayerDataTable();
        playerDataTable.Load(DataTableIds.PlayerTable);
        tables.Add(DataTableIds.PlayerTable, playerDataTable);

        BossDataTable bossDataTable = new BossDataTable();
        bossDataTable.Load(DataTableIds.BossTable);
        tables.Add(DataTableIds.BossTable, bossDataTable);

        StageDataTable stageDataTable = new StageDataTable();
        stageDataTable.Load(DataTableIds.StageTable);
        tables.Add(DataTableIds.StageTable, stageDataTable);

        ItemDataTable itemDataTable = new ItemDataTable();
        itemDataTable.Load(DataTableIds.ItemTable);
        tables.Add(DataTableIds.ItemTable, itemDataTable);
    }

    public static StringTable GetStringTable()
    {
        return Get<StringTable>(DataTableIds.String);
    }

    public static T Get<T>(string id) where T : DataTable
    {
        if (!tables.ContainsKey(id))
        {
            Debug.Log("DataTable Key is null");
            return null;
        }
        return tables[id] as T;
    }

}
