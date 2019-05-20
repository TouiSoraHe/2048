using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game2048Data
{
    private int[,] value;

    public event Action<int[,]> onValueChange;

    public Game2048Data(int count)
    {
        this.value = new int[count,count];
    }

    public void Init()
    {
        for (int x = 0; x < value.GetLength(0); x++)
        {
            for (int y = 0; y < value.GetLength(1); y++)
            {
                value[x,y] = 0;
            }
        }
        RandomlyGenerated();
        ValueChangeCallBack();
    }

    public void Move(MoveDirection direction)
    {
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
        RandomlyGenerated();
        ValueChangeCallBack();
    }

    /// <summary>
    /// 根据移动的方向决定是否转置数组
    /// </summary>
    /// <param name="direction"></param>
    private void TransposeWithDirection(MoveDirection direction)
    {
        if (direction == MoveDirection.Down || direction == MoveDirection.Up)
        {
            value = Transpose(value);
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
                    int temp = value[x, y];
                    value[x, y] = value[x, length - 1 - y];
                    value[x, length - 1 - y] = temp;
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
            for (int y = 0; y < value.GetLength(1); y++)
            {

            }
            List<int> tempValue = new List<int>();
            //提取非0的值
            for (int y = 0; y < value.GetLength(1); y++)
            {
                if (value[x, y] != 0)
                {
                    tempValue.Add(value[x, y]);
                    value[x, y] = 0;
                }
            }
            //判断相邻,对相邻且相同的值合并
            for (int y = 1; y < tempValue.Count; y++)
            {
                if (tempValue[y] == tempValue[y - 1])
                {
                    tempValue[y - 1] *= 2;
                    tempValue[y] = 0;
                }
            }
            //合并后的值还原到原数组中
            for (int y = 0; y < tempValue.Count; y++)
            {
                if (tempValue[y] != 0)
                {
                    value[x, y] = tempValue[y];
                }
            }
        }
    }

    /// <summary>
    /// 在空白区域随机生成2或者4
    /// </summary>
    /// <returns></returns>
    private bool RandomlyGenerated()
    {
        List<Vector2Int> axis = new List<Vector2Int>();
        ForEachValue(value,(coordinate) =>
        {
            if (value[coordinate.x,coordinate.y] == 0)
            {
                axis.Add(coordinate);
            }
        });
        if (axis.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, axis.Count);
            int x = axis[index].x;
            int y = axis[index].y;
            value[x,y] = UnityEngine.Random.Range(1, 3) * 2;
            return true;
        }
        return false;
    }

    private void ValueChangeCallBack()
    {
        onValueChange?.Invoke(value);
    }

    /// <summary>
    /// 遍历二维数组,返回数组的坐标
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="callBack"></param>
    private void ForEachValue<T>(T[,] array, Action<Vector2Int> callBack)
    {
        for (int i = 0; i < array.GetLength(0); i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                callBack?.Invoke(new Vector2Int(i,j));
            }
        }
    }

    /// <summary>
    /// 转置二维数组
    /// </summary>
    /// <param name=""></param>
    private T[,] Transpose<T>(T[,] value)
    {
        T[,] newValue = new T[value.GetLength(1), value.GetLength(0)];
        for (int x = 0; x < value.GetLength(0); x++)
        {
            for (int y = 0; y < value.GetLength(1); y++)
            {
                newValue[y, x] = value[x, y];
            }
        }
        return newValue;
    }

}
