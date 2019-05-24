using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game2048View
{
    private Transform root;
    private int count;
    private Item[,] items;

    public Game2048View(Transform root,Vector2 size, int count)
    {
        this.root = root;
        this.count = count;
        float width = size.x / count;
        float height = size.y / count;
        items = new Item[count, count];
        for (int x = 0; x < count; x++)
        {
            for (int y = 0; y < count; y++)
            {
                GameObject item = Object.Instantiate(Resources.Load<GameObject>("Prefab/Item"), root.transform);
                items[x, y] = item.GetComponent<Item>();
                items[x, y].name = x + "," + y;
                RectTransform rect = items[x, y].GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(width, height);
                rect.localPosition = new Vector3(y * width, x * height*-1, 0);                
            }
        }
    }

    public void Refresh(int[,] data,TransformInfo[,] transformInfos)
    {
        Util.ForEachValue(items, (c) => 
        {
            items[c.x, c.y].SetValue(data[c.x, c.y]);
            return true;
        });
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
