using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    private Game2048Mgr game2048Mgr;

    private void Awake()
    {
        game2048Mgr = new Game2048Mgr(4, FindObjectOfType<GridLayoutGroup>());
        game2048Mgr.Init();
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
