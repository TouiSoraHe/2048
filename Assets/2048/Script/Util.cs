using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    /// <summary>
    /// 转置二维数组
    /// </summary>
    /// <param name=""></param>
    public static void Transpose<T>(T[,] value)
    {
        for (int x = 0; x < value.GetLength(0)-1; x++)
        {
            for (int y = x+1; y < value.GetLength(1); y++)
            {
                Swap(ref value[x, y], ref value[y, x]);
            }
        }
    }

    public static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    /// <summary>
    /// 遍历二维数组,返回数组的坐标,回调返回true代表继续，返回false代表break
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="callBack"></param>
    public static void ForEachValue<T>(T[,] array, Func<Vector2Int, bool> callBack)
    {
        if (callBack != null)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (!callBack.Invoke(new Vector2Int(i, j)))
                    {
                        return;
                    }
                }
            }
        }
    }
}
