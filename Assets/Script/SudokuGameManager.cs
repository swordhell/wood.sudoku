using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuGameManager : MonoBehaviour
{
    public static SudokuGameManager instance { get; private set; }

    public int sudokuSize { get; private set; }
    public int difficulty { get; set; }
    public GameState gameState { get; set; }
    public bool isPaused { get; set; }
    public bool isDraft { get; set; }
    public int errorCount { get; set; }
    public Vector2Int pickupCoordinate;

    public GameObject sudokuCanvas;
    private SudokuGrid sudokuGrid;

    public GameObject mainMenuCanvas;
    private SudokuBoard sudokuBoard;

    public enum GameState
    {
        idle,
        start,
        fail,
        success,
    }

    public int spendTimeInSeconds { get; set; }
    private void Awake()
    {
        instance = this;
        OnBTNBack();
        initMember();
    }

    private void initMember()
    {
        sudokuSize = 9;
        difficulty = 40;

        pickupCoordinate.x = -1;
        pickupCoordinate.y = -1;

        gameState = GameState.idle;
    }

    private void resetDynamicData()
    {
        gameState = GameState.idle;
        errorCount = 0;
        spendTimeInSeconds = 0;
        isPaused = false;
    }

    public void SetDifficulty(float value)
    {
        difficulty = (int)value;
    }

    public void OnBTNPlay()
    {
        mainMenuCanvas.SetActive(false);
        sudokuCanvas.SetActive(true);
    }

    public void OnBTNNew()
    {
        resetDynamicData();
        gameState = GameState.start;
    }

    public void OnBTNBack()
    {
        resetDynamicData();
        sudokuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void OnBTNSelect(Vector2Int coordinate)
    {
        if (!IsGameProgress())
        {
            return;
        }
        Debug.Log($"OnBTNSelect {coordinate.x} {coordinate.y}");
        pickupCoordinate = coordinate;

        sudokuGrid.doSelect(pickupCoordinate);

    }

    private void initSudoGameObject()
    {
        if (sudokuGrid == null)
        {
            var child = sudokuCanvas.transform.Find("Grid");
            sudokuGrid = child.GetComponent<SudokuGrid>();
        }

        if (sudokuBoard == null)
        {
            sudokuBoard = sudokuCanvas.GetComponent<SudokuBoard>();
        }
    }

    public void OnBTNInputNumber(int number)
    {
        if (!IsGameProgress())
        {
            return;
        }
        var result = sudokuGrid.doInputNumber(pickupCoordinate, isDraft, number);
        if (result == 2)
        {
            errorCount++;
            sudokuBoard.OnShowErrorCount();
            if (errorCount > 5)
            {
                OnFail();
            }
        }

    }

    public bool IsGameProgress()
    {
        return gameState == GameState.start && !isPaused;
    }

    public void OnBTNClear()
    {
        if (!IsGameProgress())
        {
            return;
        }
        sudokuGrid.doClearCell(pickupCoordinate);

        pickupCoordinate.x = -1;
        pickupCoordinate.y = -1;


    }

    void Start()
    {
        InvokeRepeating(nameof(OnTimerElapsed), 1, 1);
        initSudoGameObject();
    }

    void Update()
    {
    }

    public void OnBTNPause()
    {
        if (gameState == GameState.start)
        {
            isPaused = !isPaused;
            sudokuBoard.doPause(isPaused);
        }

    }

    private void OnTimerElapsed()
    {
        if (!IsGameProgress())
        {
            return;
        }
        spendTimeInSeconds++;
        sudokuBoard.OnTimerElapsed(spendTimeInSeconds);
    }

    public void OnSuccess()
    {
        gameState = GameState.success;
        sudokuBoard.doPause(isPaused);
        sudokuBoard.doSuccess();
    }

    public void OnFail()
    {
        gameState = GameState.fail;
        sudokuBoard.doPause(isPaused);
        sudokuBoard.doDefeate();
    }

    public void OnBTNMusicOpen(bool _isOpen)
    {

    }

    public void OnBTNDraftOpen(bool _isOpen)
    {
        isDraft = _isOpen;
    }
}
