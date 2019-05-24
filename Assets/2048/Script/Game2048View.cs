using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game2048View
{
    private GridLayoutGroup grid;
    private int count;

    public Game2048View(GridLayoutGroup grid, int count)
    {
        this.grid = grid;
        this.count = count;
        RectTransform rectTransform = grid.GetComponent<RectTransform>();
        grid.cellSize = new Vector2(rectTransform.sizeDelta.x / count, rectTransform.sizeDelta.y / count);
        for (int i = 0; i < count * count; i++)
        {
            GameObject.Instantiate(Resources.Load("Prefab/Item"), grid.transform);
        }
    }

    public void Refresh(int[,] data,TransformInfo[,] transformInfos)
    {
        int i = 0;
        int j = 0;
        foreach (Transform item in grid.transform)
        {
            item.GetComponent<Item>().SetValue(data[i,j]);
            j++;
            if (j == count)
            {
                j = 0;
                i++;
            }
        }
        for (int x = 0; x < transformInfos.GetLength(0); x++)
        {
            string str = "";
            for (int y = 0; y < transformInfos.GetLength(1); y++)
            {
                TransformInfo item = transformInfos[x, y];
                str += "\t" + item.BeforeValue;
                if (item.Distance > 0)
                {
                    switch (item.MoveDirection)
                    {
                        case MoveDirection.Left:
                            str += "←";
                            break;
                        case MoveDirection.Right:
                            str += "→";
                            break;
                        case MoveDirection.Up:
                            str += "↑";
                            break;
                        case MoveDirection.Down:
                            str += "↓";
                            break;
                        default:
                            break;
                    }
                    str += item.Distance;
                }
                if (item.AfterValue != item.BeforeValue)
                {
                    if (item.BeforeValue < item.AfterValue)
                    {
                        str += "变大" + item.AfterValue;
                    }
                    else
                    {
                        str += "消除" + item.AfterValue;
                    }
                }
                str += "\t";
            }
            Debug.Log(str + "\n");
        }
        Debug.Log("-------------------------------------------------------------------------\n");
    }

    public void GameFail()
    {
        Debug.Log("游戏失败");
    }

    public void Victory()
    {
        Debug.Log("游戏胜利");
    }
}
