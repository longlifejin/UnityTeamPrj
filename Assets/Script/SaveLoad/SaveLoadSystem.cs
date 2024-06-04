using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public static class SaveLoadSystem 
{
    private static string SaveDirectory
    {
        get
        {
            return $"{Application.persistentDataPath}/Save";
        }
    }

    public static int SaveDataVersion { get; private set; } = 1;

    private static readonly string[] SaveFileName =
    {
        "SaveGameData.sav",
        "Save1.sav",
        "Save2.sav",
        "Save3.sav"
    };

    static SaveLoadSystem()
    {
        //if(!Load())
        //{
        //    CurrSaveData = new SaveDataV1();
        //    Save();
        //}
    }

    public static SaveDataV1 CurrSaveData { get; set; }

    public static bool Save(int slot = 0)
    {
        if (slot < 0 || slot >= SaveFileName.Length)
        {
            return false;
        }

        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }

        CurrSaveData = new SaveDataV1
        {
            gainedHp = Player.Instance.gainedHp,
            gainedAtk = Player.Instance.gainedAtk,
            gainedCritical = Player.Instance.gainedCritical,
            gold = Player.Instance.gold,
            lastCleardStage = Player.Instance.stageClear.FindLastIndex(stage => stage),
            atkItemIndex = Player.Instance.atkItemIndex,
            hpItemIndex = Player.Instance.hpItemIndex,
            criticalItemIndex = Player.Instance.criticalItemIndex,
        };

        var path = Path.Combine(SaveDirectory, SaveFileName[slot]);

        using (var writer = new JsonTextWriter(new StreamWriter(path)))
        {
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All,
                Converters = { new GameDataJsonConverter() }
            };
            serializer.Serialize(writer, CurrSaveData);
        }
        return true;
    }

    public static bool Load(int slot = 0)
    {
        if (slot < 0 || slot >= SaveFileName.Length)
        {
            return false;
        }
        var path = Path.Combine(SaveDirectory, SaveFileName[slot]);
        if (!File.Exists(path))
        {
            return false;
        }

        SaveData data = null;
        using (var reader = new JsonTextReader(new StreamReader(path)))
        {
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All,
                Converters = { new GameDataJsonConverter() }
            };
            data = serializer.Deserialize<SaveDataV1>(reader);
        }

        while (data.Version < SaveDataVersion)
        {
            data = data.VersionUp();
        }
        CurrSaveData = data as SaveDataV1;

        Player.Instance.gainedHp = CurrSaveData.gainedHp;
        Player.Instance.gainedAtk = CurrSaveData.gainedAtk;
        Player.Instance.gainedCritical = CurrSaveData.gainedCritical;
        Player.Instance.gold = CurrSaveData.gold;
        Player.Instance.atkItemIndex = CurrSaveData.atkItemIndex;
        Player.Instance.hpItemIndex = CurrSaveData.hpItemIndex;
        Player.Instance.criticalItemIndex = CurrSaveData.criticalItemIndex;

        Player.Instance.stageClear.Clear();
        for (int i = 0; i <= CurrSaveData.lastCleardStage; i++)
        {
            Player.Instance.stageClear.Add(true);
        }
        for (int i = CurrSaveData.lastCleardStage + 1; i < 8; i++)
        {
            Player.Instance.stageClear.Add(false);
        }

        return true;
    }

}
