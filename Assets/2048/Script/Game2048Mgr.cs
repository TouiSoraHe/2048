using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game2048Mgr
{
    private Game2048Data data;
    private Game2048View view;
    private bool init = false;

    public Game2048Mgr(int count,GridLayoutGroup grid)
    {
        view = new Game2048View(grid, count);
        data = new Game2048Data(count);
        data.onValueChange += view.Refresh;
    }

    public void Init()
    {
        init = true;
        data.Init();
    }

    public void Update()
    {
        if (!init) return;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            data.Move(MoveDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            data.Move(MoveDirection.Right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            data.Move(MoveDirection.Up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            data.Move(MoveDirection.Down);
        }
    }
}
