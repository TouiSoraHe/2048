using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformInfo
{
    public MoveDirection MoveDirection { get; set; }
    public int Distance { get; set; }
    public int BeforeValue { get; set; }
    public int AfterValue { get; set; }

    public TransformInfo()
    {
    }

    public TransformInfo(MoveDirection moveDirection, int distance, int beforeValue, int afterValue)
    {
        MoveDirection = moveDirection;
        Distance = distance;
        BeforeValue = beforeValue;
        AfterValue = afterValue;
    }

    public TransformInfo Clone()
    {
        return new TransformInfo(MoveDirection, Distance, BeforeValue, AfterValue);
    }
}

public class Game2048Data : IDisposable
{
    private int[,] value;
    private TransformInfo[,] transformInfo;
    private int victoryScore;
    private int score = 0;

    public event Action<TransformInfo[,],int> onValueChange;
    public event Action onGameFail;
    public event Action onVictory;

    public void Init(int count, int victoryScore)
    {
        this.value = new int[count, count];
        this.transformInfo = new TransformInfo[count, count];
        this.victoryScore = victoryScore;
        score = 0;
        for (int x = 0; x < value.GetLength(0); x++)
        {
            for (int y = 0; y < value.GetLength(1); y++)
            {
                value[x,y] = 0;
            }
        }
        RandomlyGenerated(2);
    }

    public void Init(int[,] data, int victoryScore, int score)
    {
        this.value = new int[data.GetLength(0), data.GetLength(1)];
        this.transformInfo = new TransformInfo[data.GetLength(0), data.GetLength(1)];
        this.victoryScore = victoryScore;
        this.score = score;
        InitTransformInfo(MoveDirection.Left);
        Util.ForEachValue(data,(c)=> 
        {
            value[c.x, c.y] = data[c.x, c.y];
            transformInfo[c.x, c.y].AfterValue = value[c.x, c.y];
            return true;
        });
        ValueChangeCallBack();
    }

    public void Move(MoveDirection direction)
    {
        //初始化变换信息
        InitTransformInfo(direction);
        //通过转置数组,反转子数组,让整个数组的方向全部统一
        //转置数组
        TransposeWithDirection(direction);
        //子数组反转
        ReverseWithDirection(direction);
        //对方向统一后的数组进行合并
        MergeValue();
        //子数组反转回来
        ReverseWithDirection(direction);
        //数组转置回来
        TransposeWithDirection(direction);
        //如果移动了则回调并随机生成新的方块
        if (IsMoved())
        {
            ValueChangeCallBack();
            RandomlyGenerated(1);
        }
        if (IsVictory())
        {
            onVictory?.Invoke();
        }
        else if (IsFail())
        {
            onGameFail?.Invoke();
        }
    }

    private bool IsVictory()
    {
        for (int x = 0; x < value.GetLength(0); x++)
        {
            for (int y = 0; y < value.GetLength(1); y++)
            {
                if (value[x, y] == victoryScore)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsFail()
    {
        for (int x = 0; x < value.GetLength(0); x++)
        {
            for (int y = 0; y < value.GetLength(1); y++)
            {
                if (value[x, y] == 0)
                {
                    return false;
                }
                if (y < value.GetLength(1) - 1 && value[x, y] == value[x, y + 1])
                {
                    return false;
                }
                if (x < value.GetLength(0) - 1 && value[x, y] == value[x+1, y])
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void InitTransformInfo(MoveDirection direction)
    {
        Util.ForEachValue(transformInfo, (c) =>
        {
            if (transformInfo[c.x, c.y] == null)
            {
                transformInfo[c.x, c.y] = new TransformInfo();
            }
            transformInfo[c.x, c.y].MoveDirection = direction;
            transformInfo[c.x, c.y].Distance = 0;
            transformInfo[c.x, c.y].BeforeValue = value[c.x, c.y];
            transformInfo[c.x, c.y].AfterValue = value[c.x, c.y];
            return true;
        });
    }

    /// <summary>
    /// 根据移动的方向决定是否转置数组
    /// </summary>
    /// <param name="direction"></param>
    private void TransposeWithDirection(MoveDirection direction)
    {
        if (direction == MoveDirection.Down || direction == MoveDirection.Up)
        {
            Util.Transpose(value);
            Util.Transpose(transformInfo);
        }
    }

    /// <summary>
    /// 根据移动的方向决定是否反转子数组
    /// </summary>
    /// <param name="direction"></param>
    private void ReverseWithDirection(MoveDirection direction)
    {
        if (direction == MoveDirection.Down || direction == MoveDirection.Right)
        {
            for (int x = 0; x < value.GetLength(0); x++)
            {
                int length = value.GetLength(1);
                for (int y = 0; y < length / 2; y++)
                {
                    Util.Swap(ref value[x, y], ref value[x, length - 1 - y]);
                    Util.Swap(ref transformInfo[x, y], ref transformInfo[x, length - 1 - y]);
                }
            }
        }
    }

    /// <summary>
    /// 合并value
    /// </summary>
    private void MergeValue()
    {

        for (int x = 0; x < value.GetLength(0); x++)
        {
            List<int> tempValue = new List<int>();
            List<TransformInfo> transformInfos = new List<TransformInfo>();
            //提取非0的值
            int zeroCount = 0;
            for (int y = 0; y < value.GetLength(1); y++)
            {
                if (value[x, y] != 0)
                {
                    tempValue.Add(value[x, y]);
                    transformInfo[x, y].Distance = zeroCount;
                    transformInfos.Add(transformInfo[x, y]);
                    value[x, y] = 0;
                }
                else
                {
                    zeroCount++;
                }
            }
            //判断相邻,对相邻且相同的值合并
            int mergeCount = 0;
            for (int y = 1; y < tempValue.Count; y++)
            {
                if (tempValue[y] == tempValue[y - 1])
                {
                    score += tempValue[y - 1];
                    tempValue[y - 1] *= 2;
                    tempValue[y] = 0;
                    transformInfos[y].AfterValue = tempValue[y];
                    transformInfos[y - 1].AfterValue = tempValue[y - 1];
                    mergeCount++;
                }
                transformInfos[y].Distance += mergeCount;
            }
            //合并后的值还原到原数组中
            for (int z = 0, y = 0; z < tempValue.Count; z++)
            {
                if (tempValue[z] != 0)
                {
                    value[x, y] = tempValue[z];
                    y++;
                }
            }
        }
    }

    private bool IsMoved()
    {
        bool moved = false;
        Util.ForEachValue(transformInfo, (c) =>
        {
            if (transformInfo[c.x, c.y].Distance > 0)
            {
                moved = true;
                return false;
            }
            return true;
        });
        return moved;
    }

    /// <summary>
    /// 在空白区域随机生成2或者4
    /// </summary>
    /// <returns></returns>
    private void RandomlyGenerated(int count)
    {
        InitTransformInfo(MoveDirection.Left);
        List<Vector2Int> axis = new List<Vector2Int>();
        Util.ForEachValue(value,(coordinate) =>
        {
            if (value[coordinate.x,coordinate.y] == 0)
            {
                axis.Add(coordinate);
            }
            return true;
        });
        for (int i = 0; i < count && axis.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, axis.Count);
            int x = axis[index].x;
            int y = axis[index].y;
            value[x, y] = UnityEngine.Random.Range(1, 3) * 2;
            transformInfo[x, y].AfterValue = value[x, y];
            axis.RemoveAt(index);
        }
        ValueChangeCallBack();
    }

    private void ValueChangeCallBack()
    {
        onValueChange?.Invoke(transformInfo,score);
    }

    public void Dispose()
    {
        
    }
}
