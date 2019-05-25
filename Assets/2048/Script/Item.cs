using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text = null;
    [SerializeField] private Image background = null;
    private Dictionary<int, Color> colorMap;

    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        colorMap = new Dictionary<int, Color>()
        {
            { 0,HtmlStringToColor("#ccc0b3")},
            { 2,HtmlStringToColor("#eee4da")},
            { 4,HtmlStringToColor("#ede0c8")},
            { 8,HtmlStringToColor("#f2b179")},
            { 16,HtmlStringToColor("#f2b179")},
            { 32,HtmlStringToColor("#f67c5f")},
            { 64,HtmlStringToColor("#f65e3b")},
            { 128,HtmlStringToColor("#edcf72")},
            { 256,HtmlStringToColor("#edcc61")},
            { 512,HtmlStringToColor("#edc850")},
            { 1024,HtmlStringToColor("#edc53f")},
            { 2048,HtmlStringToColor("#edc22e")},
        };
    }

    private void Start()
    {
        float size = rect.sizeDelta.x > rect.sizeDelta.y ? rect.sizeDelta.x : rect.sizeDelta.y;
        text.fontSize = size * 0.3f;
    }

    public void SetValue(int value)
    {
        if (value > 0)
        {
            text.text = value.ToString();
            if (colorMap.ContainsKey(value))
            {
                background.color = colorMap[value];
            }
            else
            {
                background.color = colorMap[0];
            }
        }
        else
        {
            text.text = "";
            background.color = colorMap[0];
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

    private static Color HtmlStringToColor(string colorString)
    {
        Color color;
        if (!ColorUtility.TryParseHtmlString(colorString, out color))
        {
            throw new System.Exception("转换失败");
        }
        return color;

    }
}
