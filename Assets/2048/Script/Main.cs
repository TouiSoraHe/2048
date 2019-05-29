using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private Game2048Mgr game2048Mgr;
    [SerializeField] private RectTransform GamePanle = null;
    [SerializeField] private Transform root = null;
    [SerializeField] private Button newGameBtn = null;
    [SerializeField] private Button undoBtn = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private TextMeshProUGUI bestScoreText = null;
    [SerializeField] private GameObject gameOver = null;
    [SerializeField] private TextMeshProUGUI gameOverText = null;
    [SerializeField] private Button gameOverBtn = null;
    private Vector2 size;
    private int count = 4;
    private int victoryScore = 2048;
    private int score = -1;
    private int bestScore = -1;
    private bool canUndo = true;

    public int Score
    {
        get => score;
        set
        {
            if (value != score)
            {
                score = value;
                scoreText.text = "分数\n" + score;
            }
        }
    }
    public int BestScore
    {
        get => bestScore;
        set
        {
            if (bestScore != value)
            {
                bestScore = value;
                bestScoreText.text = "最佳\n" + bestScore;
            }
        }
    }

    public bool CanUndo
    {
        get => canUndo;
        set
        {
            if (canUndo != value)
            {
                canUndo = value;
                undoBtn.interactable = canUndo;
            }
        }
    }

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        gameOver.SetActive(false);
        size = GamePanle.sizeDelta;
        gameOver.SetActive(false);
    }

    private void Start()
    {
        StartGame();
        newGameBtn.onClick.AddListener(NewGame);
        undoBtn.onClick.AddListener(() =>
        {
            game2048Mgr.Undo();
        });
        gameOverBtn.onClick.AddListener(NewGame);
    }

    private void StartGame()
    {
        int count = 4;
        SetRootPos(count, size);
        game2048Mgr = new Game2048Mgr();
        InputManager.Instance.OnMoved += game2048Mgr.Move;
        game2048Mgr.onGameOver += (isVictory) =>
        {
            gameOver.SetActive(true);
            gameOverText.text = isVictory ? "胜利" : "失败";
        };
        if (!game2048Mgr.LoadGame(root, size))
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        gameOver.SetActive(false);
        game2048Mgr.NewGame(count, victoryScore, root, size);
    }


    private void SetRootPos(int count, Vector2 size)
    {
        float offserX = -1 * size.x / 2 + size.x / count / 2;
        float offserY = size.y / 2 - size.y / count / 2;
        root.localPosition = new Vector3(offserX, offserY, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (game2048Mgr != null)
        {
            Score = game2048Mgr.Score;
            BestScore = game2048Mgr.BestScore;
            CanUndo = game2048Mgr.CanUndo;
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
        game2048Mgr.Dispose();
        game2048Mgr = null;
    }
}
