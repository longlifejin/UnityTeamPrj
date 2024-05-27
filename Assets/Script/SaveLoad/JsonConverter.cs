using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataJsonConverter : JsonConverter<SaveDataV1>
{
    public override SaveDataV1 ReadJson(JsonReader reader, Type objectType, SaveDataV1 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var data = new SaveDataV1();
        JObject jobj = JObject.Load(reader);
        data.gainedHp = (int)(jobj["GainedHp"]);
        data.gainedAtk = (int)(jobj["GainedAtk"]);
        data.gold = (int)(jobj["Gold"]);
        data.lastCleardStage = (int)jobj["LastClearedStage"];
        data.atkItemIndex = (int)(jobj["AtkItemIndex"]);
        data.hpItemIndex = (int)(jobj["HpItemIndex"]);
        return data;
    }

    public override void WriteJson(JsonWriter writer, SaveDataV1 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("GainedHp");
        writer.WriteValue(value.gainedHp);
        writer.WritePropertyName("GainedAtk");
        writer.WriteValue(value.gainedAtk);
        writer.WritePropertyName("Gold");
        writer.WriteValue(value.gold);
        writer.WritePropertyName("LastClearedStage");
        writer.WriteValue(value.lastCleardStage);
        writer.WritePropertyName("AtkItemIndex");
        writer.WriteValue(value.atkItemIndex);
        writer.WritePropertyName("HpItemIndex");
        writer.WriteValue(value.hpItemIndex);
        writer.WriteEndObject();
    }
}
