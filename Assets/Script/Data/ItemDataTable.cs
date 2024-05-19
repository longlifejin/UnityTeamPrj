using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemData
{
    public readonly string icon_path = "image/{0}";

    public string Item_ID { get; set; }
    public int Item_type { get; set; }
    public string Item_Name { get; set;}
    public int Value { get; set;}
    public int Gold { get; set;}
    public int Stack { get; set;}
    public string Item_Info { get; set;}
    public string Item_Image { get; set;}

    public string GetName
    {
        get
        {
            return DataTableMgr.Get<StringTable>(DataTableIds.String).Get(Item_Name);
        }
    }

    public string GetInfo
    {
        get
        {
            return DataTableMgr.Get<StringTable>(DataTableIds.String).Get(Item_Info);
        }
    }

    public Texture GetImage
    {
        get
        {
            return Resources.Load<Texture>(string.Format(icon_path, Item_Image));
        }
    }
}


public class ItemDataTable : DataTable
{
    private Dictionary<string, ItemData> table = new Dictionary<string, ItemData>();

    public List<string> AllIds
    {
        get
        {
            return table.Keys.ToList();
        }
    }

    public ItemData Get(string id)
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
            var records = csvReader.GetRecords<ItemData>();
            foreach (var record in records)
            {
                table.Add(record.Item_ID, record);
            }
        }
    }
}
