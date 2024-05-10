using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class StringTable : DataTable
{
    private class Data
    {
        public string String_ID { get; set; }
        public string Text {  get; set; }
    }

    private Dictionary<string,string> table = new Dictionary<string,string>();


    public override void Load(string path)
    {
        var textAsset = Resources.Load<TextAsset>(path);

        using (var reader = new StringReader(textAsset.text))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csvReader.GetRecords<Data>();
            foreach (var record in records)
            {
                table.Add(record.String_ID, record.Text);
            }
        }
    }

    public string Get(string id)
    {
        if (!table.ContainsKey(id))
        {
            Debug.Log("StringTable Key is null");
            return null;
        }

        return table[id];
    }
}
