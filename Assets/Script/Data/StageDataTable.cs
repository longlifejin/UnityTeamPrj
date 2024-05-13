using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;


public class StageData
{
    public string Stage_ID { get; set; }
    public string Boss_ID { get; set; }
    public string Stage_Name { get; set; }
    public int Stage_Reward { get; set; }
    public string Stage_Info { get; set; }
    public string Stage_Image { get; set; }
}


public class StageDataTable : DataTable
{
    private Dictionary<string, StageData> table = new Dictionary<string, StageData>();

    public List<string> AllIds
    {
        get
        {
            return table.Keys.ToList();
        }
    }

    public StageData Get(string id)
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
            var records = csvReader.GetRecords<StageData>();
            foreach (var record in records)
            {
                table.Add(record.Stage_ID, record);
            }
        }
    }
}
