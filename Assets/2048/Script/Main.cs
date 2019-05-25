using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    private Game2048Mgr game2048Mgr;
    public Transform root;
    public Vector2 size;
    public int count = 4;

    private void Awake()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = size;
        SetRootPos(count, size);
        game2048Mgr = new Game2048Mgr(count, 2048, root, size);
        game2048Mgr.Init();
    }

    private void SetRootPos(int count, Vector2 size)
    {
        float offserX = -1 * size.x / 2 + size.x / count / 2;
        float offserY = size.y / 2 - size.y / count / 2;
        root.localPosition = new Vector3(offserX, offserY, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        game2048Mgr.Update();
    }
}
