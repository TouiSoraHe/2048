using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game2048Mgr : IDisposable
{
    public event Action<bool> onGameOver;

    private Game2048Data data;
    private Game2048View view;
    private bool init = false;
    private bool gameOver = true;
    private SaveData saveData;
    private Stack<SaveData> undoList;
    private Transform root;
    private Vector2 size;

    public int Score
    {
        get
        {
            return saveData?.Score ?? 0;
        }
    }

    public int BestScore
    {
        get
        {
            return saveData?.BestScore ?? 0;
        }
    }

    public bool CanUndo
    {
        get
        {
            return (undoList?.Count ?? 0) > 1;
        }
    }

    public Game2048Mgr()
    {
        view = new Game2048View();
        data = new Game2048Data();
        AddEvent();
        saveData = SaveData.Load();
    }

    public void NewGame(int count, int victoryScore, Transform root, Vector2 size)
    {
        if (saveData == null)
        {
            saveData = new SaveData(0, 0, victoryScore, new int[count, count]);
        }
        else
        {
            saveData.Score = 0;
            saveData.VectoryScore = victoryScore;
            saveData.Data = new int[count, count];
        }
        this.root = root;
        this.size = size;
        undoList = new Stack<SaveData>();
        init = true;
        gameOver = false;
        view.Init(root, size, count);
        data.Init(count, victoryScore);
    }

    public bool LoadGame(Transform root, Vector2 size)
    {
        if (saveData == null)
        {
            return false;
        }
        this.root = root;
        this.size = size;
        undoList = new Stack<SaveData>();
        init = true;
        gameOver = false;
        view.Init(root, size, saveData.Data.GetLength(0));
        data.Init(saveData.Data, saveData.VectoryScore, saveData.Score);
        return true;
    }

    public bool Undo()
    {
        if (undoList.Count <= 1) return false;
        //每次data.onValueChange事件触发都会修改SaveData的值并将其压入撤销堆栈中,
        //data.onValueChange触发条件为:data.Init调用时,移动时,移动后随机生成数字时
        //所以撤销堆栈里面的数据始终是:init的数据->move的数据->随机生成数字->move的数据->随机生成数字->move的数据->随机生成数字->...
        //最顶层的两个数据始终是最后一次移动操作的结果,如果要回退到上一步,则应该是需要第三层的数据,所以将前两层数据弹出丢弃,第三层数据用来
        //重新初始化data,然后该数据又会因为init操作触发onValueChange事件并重新压入撤销堆栈
        undoList.Pop();
        undoList.Pop();
        saveData = undoList.Pop();
        init = true;
        gameOver = false;
        view.Init(root, size, saveData.Data.GetLength(0));
        data.Init(saveData.Data, saveData.VectoryScore, saveData.Score);
        return true;

    }

    private void AddEvent()
    {
        data.onValueChange += view.Refresh;
        data.onValueChange += OnValueChange;
        data.onGameFail += OnGameFail;
        data.onVictory += OnVictory;
    }

    private void RemoveEvent()
    {
        data.onValueChange -= view.Refresh;
        data.onValueChange -= OnValueChange;
        data.onGameFail -= OnGameFail;
        data.onVictory -= OnVictory;
    }

    private void OnValueChange(TransformInfo[,] transformInfos, int score)
    {
        Util.ForEachValue(transformInfos, (c) => 
        {
            saveData.Data[c.x,c.y] = transformInfos[c.x, c.y].AfterValue;
            saveData.Score = score;
            saveData.BestScore = saveData.BestScore > score ? saveData.BestScore : score;
            return true;
        });
        SaveData.Save(saveData);
        undoList.Push(saveData.Clone());
    }

    private void OnVictory()
    {
        gameOver = true;
        init = false;
        onGameOver?.Invoke(true);
    }

    private void OnGameFail()
    {
        gameOver = true;
        init = false;
        onGameOver?.Invoke(false);
    }

    public void Dispose()
    {
        init = false;
        RemoveEvent();
        data.Dispose();
        view.Dispose();
    }

    public void Move(MoveDirection direction)
    {
        if (!init) return;
        if (!gameOver)
        {
            data.Move(direction);
        }
    }
}
