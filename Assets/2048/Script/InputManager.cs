using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    enum Status
    {
        Touch,
        Trigger,
        Null
    }

    private Status status = Status.Null;

    private Vector2 beginPos;
    private float triggerDistance;

    public event Action<MoveDirection> OnMoved;


    private void Awake()
    {
        triggerDistance = 80;
        if (Instance != null)
        {
            throw new System.Exception("实例化第二个单例");
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            status = Status.Touch;
            beginPos = Input.mousePosition;
        }
        if (status == Status.Touch)
        {
            if (Vector2.Distance(Input.mousePosition, beginPos) >= triggerDistance)
            {
                Vector2 direction = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - beginPos;
                MoveDirection moveDirection = MoveDirection.Down;
                //左右
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    if (direction.x > 0)
                    {
                        moveDirection = MoveDirection.Right;
                    }
                    else
                    {
                        moveDirection = MoveDirection.Left;
                    }
                }
                //上下
                else
                {
                    if (direction.y > 0)
                    {
                        moveDirection = MoveDirection.Up;
                    }
                    else
                    {
                        moveDirection = MoveDirection.Down;
                    }
                }
                OnMoved?.Invoke(moveDirection);
                status = Status.Trigger;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            status = Status.Null;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnMoved?.Invoke(MoveDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnMoved?.Invoke(MoveDirection.Right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnMoved?.Invoke(MoveDirection.Up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnMoved?.Invoke(MoveDirection.Down);
        }
    }
}
