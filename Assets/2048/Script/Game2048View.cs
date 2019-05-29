using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Game2048View : IDisposable
{
    /// <summary>
    /// 方块所在的根节点
    /// </summary>
    private Transform root;

    /// <summary>
    /// 生成方块的长宽数量
    /// </summary>
    private int count;

    /// <summary>
    /// 仅仅用作背景方块
    /// </summary>
    private List<Item> backgroundItem;

    /// <summary>
    /// 通过坐标轴找到对应的方块
    /// </summary>
    private Dictionary<Vector2Int, Item> coordinate2Item;

    /// <summary>
    /// 方块变换后,更新坐标时的临时空间
    /// </summary>
    private Dictionary<Vector2Int, Item> coordinate2ItemTemp;

    /// <summary>
    /// 方块的大小
    /// </summary>
    private Vector2 itemSize;

    public void Init(Transform root, Vector2 size, int count)
    {
        Dispose();
        this.root = root;
        this.count = count;
        itemSize = new Vector2(size.x / count, size.y / count);
        coordinate2Item = new Dictionary<Vector2Int, Item>();
        coordinate2ItemTemp = new Dictionary<Vector2Int, Item>();
        backgroundItem = new List<Item>();
        for (int x = 0; x < count; x++)
        {
            for (int y = 0; y < count; y++)
            {
                backgroundItem.Add(Item.CreateItem(new Vector2Int(x, y), root, itemSize));
            }
        }
    }

    public void Refresh(TransformInfo[,] transformInfos,int score)
    {
        //将随机生成的方块产生出来，对需要移动的方块进行移动，对AfterValue==0（需要销毁）的方块进行销毁，对值发生改变的方块修改值
        Util.ForEachValue(transformInfos, (c) =>
        {
            TransformInfo info = transformInfos[c.x, c.y];
            //如果一个方块既没有移动，数值也没发生变化则代表该方法没发生过变化，直接跳过
            if (info.Distance == 0 && info.AfterValue == info.BeforeValue) return true;
            if (info.BeforeValue == 0)
            {
                coordinate2Item[c] = Item.CreateItem(c, root, itemSize);
            }
            int? distance = null;
            MoveDirection? moveDirection = null;
            if (info.Distance > 0)
            {
                distance = info.Distance;
                moveDirection = info.MoveDirection;
            }
            int? value = null;
            if (info.AfterValue != info.BeforeValue)
            {
                value = info.AfterValue;
            }
            coordinate2Item[c].PlayAnimation(moveDirection, distance, value);
            return true;
        });
        UpdateCoordinate(transformInfos);
    }

    private void UpdateCoordinate(TransformInfo[,] transformInfos)
    {
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

    private Vector2Int Dirction(MoveDirection direction, int distance)
    {
        if (distance == 0) return new Vector2Int(0, 0);
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

    public void Dispose()
    {
        if (coordinate2Item != null)
        {
            foreach (var item in coordinate2Item)
            {
                Item.DestroyItem(item.Value);
            }
            coordinate2Item.Clear();
        }
        if (backgroundItem != null)
        {
            foreach (var item in backgroundItem)
            {
                Item.DestroyItem(item);
            }
            backgroundItem.Clear();
        }
    }
}
