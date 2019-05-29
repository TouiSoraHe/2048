using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using CrossPlatformJson;

[Serializable]
public class SaveData
{
    public int BestScore;
    public int Score;
    public int VectoryScore;
    public int[,] Data;

    public SaveData()
    {
    }

    public SaveData(int bestScore, int score, int vectoryScore, int[,] data)
    {
        BestScore = bestScore;
        Score = score;
        VectoryScore = vectoryScore;
        Data = data;
    }

    public SaveData Clone()
    {
        int[,] data = new int[Data.GetLength(0), Data.GetLength(1)];
        Array.Copy(Data, data, Data.Length);
        return new SaveData(BestScore, Score, VectoryScore, data);
    }

    public static void Save(SaveData saveData)
    {
        JavaScriptObject jsonObj = new JavaScriptObject();
        jsonObj.Add("BestScore", new JavaScriptObject(saveData.BestScore));
        jsonObj.Add("Score" , new JavaScriptObject(saveData.Score));
        jsonObj.Add("VectoryScore",new JavaScriptObject(saveData.VectoryScore));
        var data = new JavaScriptObject();
        jsonObj.Add("Data",data);
        for (int x = 0; x < saveData.Data.GetLength(0); x++)
        {
            data.Add(new JavaScriptObject());
            for (int y = 0; y < saveData.Data.GetLength(1); y++)
            {
                data[x].Add(new JavaScriptObject(saveData.Data[x, y]));
            }
        }
        PlayerPrefs.SetString("savedata", jsonObj.ToJson());
    }

    public static SaveData Load()
    {
        string json = PlayerPrefs.GetString("savedata");
        if (!string.IsNullOrEmpty(json))
        {
            JavaScriptObject jsonObj = JavaScriptObjectFactory.CreateJavaScriptObject(json);
            int[,] data = new int[jsonObj["Data"].Count, jsonObj["Data"].Count];
            Util.ForEachValue(data, (c) => 
            {
                data[c.x, c.y] = (int)jsonObj["Data"][c.x][c.y].GetNumber();
                return true;
            });
            return new SaveData((int)jsonObj["BestScore"].GetNumber(), (int)jsonObj["Score"].GetNumber(), (int)jsonObj["VectoryScore"].GetNumber(), data);
        }
        return null;
    }
}
