using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;


public class SaveGameData : MonoBehaviour
{
    public int gainedHp;
    public int gainedAtk;
    public int gainedCritical;
    public int gold;
    public int lastCleardStage;
    public int atkItemIndex;
    public int hpItemIndex;
    public int criticalItemIndex;
    public string currentStage;

    public void Save()
    {
        Debug.Log("Save Game Data");

        gainedHp = Player.Instance.gainedHp;
        gainedAtk = Player.Instance.gainedAtk;
        gold = Player.Instance.gold;
        atkItemIndex = Player.Instance.atkItemIndex;
        hpItemIndex = Player.Instance.hpItemIndex;
        criticalItemIndex = Player.Instance.criticalItemIndex;
        currentStage = Player.Instance.currentStage;

        lastCleardStage = Player.Instance.stageClear.FindLastIndex(stage => stage);

        SaveDataV1 data = new SaveDataV1
        {
            gainedHp = gainedHp,
            gainedAtk = gainedAtk,
            gainedCritical = gainedCritical,
            gold = gold,
            lastCleardStage = lastCleardStage,
            atkItemIndex = atkItemIndex,
            hpItemIndex = hpItemIndex,
            criticalItemIndex = criticalItemIndex,
        };

        SaveLoadSystem.CurrSaveData = data;
        //SaveLoadSystem.Save();
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "SaveGameData.sav");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new GameDataJsonConverter());

            SaveDataV1 data = JsonConvert.DeserializeObject<SaveDataV1>(json, settings);

            Player.Instance.gainedHp = data.gainedHp;
            Player.Instance.gainedAtk = data.gainedAtk;
            Player.Instance.gainedCritical = data.gainedCritical;
            Player.Instance.gold = data.gold;
            Player.Instance.atkItemIndex = data.atkItemIndex;
            Player.Instance.hpItemIndex = data.hpItemIndex;
            Player.Instance.criticalItemIndex = data.criticalItemIndex;

            Player.Instance.stageClear.Clear();
            for (int i = 0; i <= data.lastCleardStage; i++)
            {
                Player.Instance.stageClear.Add(true);
            }
            for (int i = data.lastCleardStage + 1; i < 8; i++)
            {
                Player.Instance.stageClear.Add(false);
            }
        }
    }
}
