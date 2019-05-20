using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

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
}
