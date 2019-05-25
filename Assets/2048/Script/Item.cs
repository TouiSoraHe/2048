using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text = null;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetValue(int value)
    {
        if (value > 0)
        {
            text.text = value.ToString();
        }
        else
        {
            text.text = "";
        }
    }

    public void Move(MoveDirection moveDirection, int distance)
    {
        switch (moveDirection)
        {
            case MoveDirection.Left:
                rect.localPosition -= Vector3.right * distance * rect.sizeDelta.x;
                break;
            case MoveDirection.Right:
                rect.localPosition += Vector3.right * distance * rect.sizeDelta.x;
                break;
            case MoveDirection.Up:
                rect.localPosition += Vector3.up * distance * rect.sizeDelta.y;
                break;
            case MoveDirection.Down:
                rect.localPosition -= Vector3.up * distance * rect.sizeDelta.y;
                break;
            default:
                break;
        }
    }
}
