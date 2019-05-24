using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game2048Mgr
{
    private Game2048Data data;
    private Game2048View view;
    private bool init = false;
    private bool gameOver = true;

    public Game2048Mgr(int count, int victoryScore, Transform root,Vector2 size)
    {
        view = new Game2048View(root, size,count);
        data = new Game2048Data(count, victoryScore);
        data.onValueChange += view.Refresh;
        data.onGameFail += OnGameFail;
        data.onVictory += OnVictory;
    }

    private void OnVictory()
    {
        gameOver = true;
        view.Victory();
    }

    private void OnGameFail()
    {
        gameOver = true;
        view.GameFail();
    }

    public void Init()
    {
        init = true;
        gameOver = false;
        data.Init();
    }

    public void Update()
    {
        if (!init) return;
        if (!gameOver)
        {
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
}
