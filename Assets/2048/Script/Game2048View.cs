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

    public void Refresh(int[,] data)
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
