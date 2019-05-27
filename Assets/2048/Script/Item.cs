using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public struct AnimationInfo
    {
        /// <summary>
        /// 最终移动到的位置
        /// </summary>
        public Vector3? EndPos;

        /// <summary>
        /// 最终显示的值
        /// </summary>
        public int? EndValue;

        /// <summary>
        /// 最终的颜色
        /// </summary>
        public Color? EndColor;

        public AnimationInfo(Vector3? endPos, int? endValue, Color? endColor)
        {
            EndPos = endPos;
            EndValue = endValue;
            EndColor = endColor;
        }
    }

    [SerializeField] private TextMeshProUGUI text = null;
    [SerializeField] private Image background = null;
    private Dictionary<int, Color> colorMap;
    private Vector3 lastPos;

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
        SetTextValue(value);
        background.color = colorMap.ContainsKey(value) ? colorMap[value] : colorMap[0];
    }

    public void Move(MoveDirection moveDirection, int distance)
    {
        this.transform.localPosition = MoveTo(moveDirection, distance);
    }

    public void PlayAnimation(MoveDirection? moveDirection, int? distance, int? value)
    {
        AnimationInfo animationInfo = new AnimationInfo();
        if (moveDirection != null && distance != null)
        {
            animationInfo.EndPos = MoveTo(moveDirection.Value, distance.Value);
        }
        if (value != null)
        {
            animationInfo.EndValue = value;
            animationInfo.EndColor = colorMap.ContainsKey(value.Value) ? colorMap[value.Value] : colorMap[0];
        }
        PlayAnimation(animationInfo);
    }

    private Sequence sequence = null;
    private void PlayAnimation(AnimationInfo animationInfo)
    {
        if (sequence != null)
        {
            sequence.Complete(true);
        }
        sequence = DOTween.Sequence();
        float speed = 0.25f;
        int lastValue = 0;
        if (text.text != "")
        {
            lastValue = int.Parse(text.text);
        }
        float moveDuration = speed* 0.5f;
        if (animationInfo.EndPos != null)
        {
            sequence.Append(this.transform.DOLocalMove(animationInfo.EndPos.Value, moveDuration).SetEase(Ease.InQuint));
        }
        //如果该方块没有移动,则让其等待相同时间,确保其他方块已经移动到指定位置再开始下一动画
        else
        {
            sequence.AppendInterval(moveDuration);
        }
        if (animationInfo.EndValue != null)
        {
            if (animationInfo.EndValue > 0)
            {
                sequence.AppendCallback(() => { SetTextValue(animationInfo.EndValue.Value); });
                if (lastValue == 0)
                {
                    sequence.Append(transform.DOScale(new Vector3(0.0f, 0.0f, 1.0f), speed).From().SetEase(Ease.InOutQuint));
                    sequence.Join(background.DOColor(animationInfo.EndColor.Value, speed).SetEase(Ease.InOutQuint));
                }
                else
                {
                    sequence.Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1.0f), speed * 0.5f).SetEase(Ease.OutQuint));
                    sequence.Join(background.DOColor(animationInfo.EndColor.Value, speed * 0.5f).SetEase(Ease.OutQuint));
                    sequence.Append(transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), speed * 0.5f).SetEase(Ease.OutQuint));
                }
            }
            else
            {
                sequence.AppendCallback(() =>
                {
                    DestroyItem(this);
                });
            }
        }
        sequence.AppendCallback(() =>
        {
            sequence = null;
        });
    }

    private Vector3 MoveTo(MoveDirection moveDirection, int distance)
    {
        Vector3 toPos = lastPos;
        switch (moveDirection)
        {
            case MoveDirection.Left:
                toPos -= Vector3.right * distance * rect.sizeDelta.x;
                break;
            case MoveDirection.Right:
                toPos += Vector3.right * distance * rect.sizeDelta.x;
                break;
            case MoveDirection.Up:
                toPos += Vector3.up * distance * rect.sizeDelta.y;
                break;
            case MoveDirection.Down:
                toPos -= Vector3.up * distance * rect.sizeDelta.y;
                break;
            default:
                break;
        }
        lastPos = toPos;
        return toPos;
    }

    private void SetTextValue(int value)
    {
        transform.SetSiblingIndex(value * -1);
        text.text = value > 0 ? value.ToString() : "";
    }

    public static Item CreateItem(Vector2Int coordinate, Transform parent, Vector2 size)
    {
        Item item = ItemPool.Instance.GetItem();
        item.transform.SetParent(parent, false);
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.localPosition = new Vector3(coordinate.y * size.x, coordinate.x * size.y * -1, 0);
        item.lastPos = rect.localPosition;
        item.SetValue(0);
        return item;
    }

    public static void DestroyItem(Item item)
    {
        ItemPool.Instance.RemoveItem(item);
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
