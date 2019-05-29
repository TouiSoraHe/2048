using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Util
{
    public static Vector2Int GetResolution()
    {

#if UNITY_EDITOR
        return GetGameViewSize();
#else
        return new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
#endif
    }

    /// <summary>
    /// 获取GameView的分辨率
    /// </summary>
    /// <returns></returns>
    public static Vector2Int GetGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetMainGameView.Invoke(null, null);
        var gameView = (UnityEditor.EditorWindow)Res;
        var prop = gameView.GetType().GetProperty("currentGameViewSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var gvsize = prop.GetValue(gameView, new object[0] { });
        var gvSizeType = gvsize.GetType();
        int height = (int)gvSizeType.GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0] { });
        int width = (int)gvSizeType.GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0] { });
        return new Vector2Int(width, height);
    }

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

    /// <summary>
    /// 交换两个变量的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
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
