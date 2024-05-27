using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;


public class BossData
{
    public string Boss_ID { get; set; }
    public string Boss_Name { get; set; }
    public int Boss_Atk { get; set; }
    public int Boss_Hp { get; set; }
    public int Boss_ATKtime { get; set; }
    public int Boss_patternA { get; set; }
    public int Boss_patternB { get; set; }
    public int Boss_patternC { get; set; }
    public int Boss_patternD { get; set; }
    public int Boss_patternE { get; set; }

    public string GetName
    {
        
        get
        {
            return DataTableMgr.Get<StringTable>(DataTableIds.String).Get(Boss_Name);
        }
    }
}

public class BossDataTable : DataTable
{
    private Dictionary<string, BossData> table = new Dictionary<string, BossData>();

    public List<string> AllIds
    {
        get
        {
            return table.Keys.ToList();
        }
    }

    public BossData Get(string id)
    {
        if (!table.ContainsKey(id))
            return null;

        return table[id];
    }

    public override void Load(string path)
    {

        var textAsset = Resources.Load<TextAsset>(path);

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<BossData>();
            foreach (var record in records)
            {
                table.Add(record.Boss_ID, record);
            }
        }
    }
}
