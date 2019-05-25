using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game2048View : IDisposable
{
    private Transform root;
    private int count;
    private Item[,] items;
    private List<Item> backgroundItem;
    private Dictionary<Vector2Int, Item> coordinate2Item ;
    private Dictionary<Vector2Int, Item> coordinate2ItemTemp;
    private Vector2 itemSize;

    public Game2048View(Transform root, Vector2 size, int count)
    {
        this.root = root;
        this.count = count;
        itemSize = new Vector2(size.x / count, size.y / count);
        items = new Item[count, count];
        coordinate2Item = new Dictionary<Vector2Int, Item>();
        coordinate2ItemTemp = new Dictionary<Vector2Int, Item>();
        backgroundItem = new List<Item>();
        for (int x = 0; x < count; x++)
        {
            for (int y = 0; y < count; y++)
            {
                backgroundItem.Add(CreateItem(new Vector2Int(x, y)));
            }
        }
    }

    public void Refresh(TransformInfo[,] transformInfos)
    {
        //将随机生成的方块产生出来，对需要移动的方块进行移动，对AfterValue==0（需要销毁）的方块进行销毁，对值发生改变的方块修改值
        Util.ForEachValue(transformInfos, (c) => 
        {
            TransformInfo info = transformInfos[c.x, c.y];
            //如果一个方块既没有移动，数值也没发生变化则代表该方法没发生过变化，直接跳过
            if (info.Distance == 0 && info.AfterValue == info.BeforeValue) return true;
            if (info.BeforeValue == 0)
            {
                coordinate2Item[c] = CreateItem(c);
            }
            Item item = coordinate2Item[c];
            item.Move(info.MoveDirection, info.Distance);
            if (info.AfterValue == 0)
            {
                DestroyItem(item);
            }
            else if(info.AfterValue != info.BeforeValue)
            {
                item.SetValue(info.AfterValue);
            }
            return true;
        });
        //修改变换后方块的坐标值，以便下次变换能找到该方块
        coordinate2ItemTemp.Clear();
        foreach (var item in coordinate2Item)
        {
            TransformInfo info = transformInfos[item.Key.x, item.Key.y];
            //如果一个方块变换的目标值为0，代表该方块已经被销毁了，不需要在保存了
            if (info.AfterValue != 0)
            {
                Vector2Int target = item.Key + Dirction(info.MoveDirection, info.Distance);
                coordinate2ItemTemp[target] = item.Value;
            }
        }
        coordinate2Item.Clear();
        foreach (var item in coordinate2ItemTemp)
        {
            coordinate2Item.Add(item.Key, item.Value);
        }
    }

    public void GameFail()
    {
        Debug.Log("游戏失败");
    }

    public void Victory()
    {
        Debug.Log("游戏胜利");
    }

    private Item CreateItem(Vector2Int coordinate)
    {
        Item item = ItemPool.Instance.GetItem();
        item.transform.SetParent(root,false);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(itemSize.x, itemSize.y);
        rect.localPosition = new Vector3(coordinate.y * itemSize.x, coordinate.x * itemSize.y * -1, 0);
        item.SetValue(0);
        return item;
    }

    private Vector2Int Dirction(MoveDirection direction,int distance)
    {
        if (distance == 0) return new Vector2Int(0,0);
        switch (direction)
        {
            case MoveDirection.Left:
                return new Vector2Int(0, -1 * distance);
            case MoveDirection.Right:
                return new Vector2Int(0, 1 * distance);
            case MoveDirection.Up:
                return new Vector2Int(-1 * distance, 0);
            case MoveDirection.Down:
                return new Vector2Int(1 * distance, 0);
            default:
                return new Vector2Int(0, 0);
        }
    }

    private void DestroyItem(Item item)
    {
        ItemPool.Instance.RemoveItem(item);
    }

    public void Dispose()
    {
        if (coordinate2Item != null)
        {
            foreach (var item in coordinate2Item)
            {
                DestroyItem(item.Value);
            }
            foreach (var item in backgroundItem)
            {
                DestroyItem(item);
            }
        }
    }
}
