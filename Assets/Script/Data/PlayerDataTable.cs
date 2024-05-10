using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayerData
{
    public string Player_ID {  get; set; }
    public string Player_Atk { get; set; }
    public string Player_Hp { get; set; }
    public string Player_Image { get; set; }

    public override string ToString()
    {
        return $"{Player_ID} / {Player_Atk} / {Player_Hp} / {Player_Image}";
    }
}

public class PlayerDataTable : DataTable
{
    private Dictionary<string, PlayerData> table = new Dictionary<string, PlayerData>();

    

    public List<string> AllIds
    {
        get
        {
            return table.Keys.ToList();
        }
    }

    public PlayerData Get(string id)
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
            var records = csvReader.GetRecords<PlayerData>();
            foreach (var record in records)
            {
                table.Add(record.Player_ID, record);
            }
        }
    }
}