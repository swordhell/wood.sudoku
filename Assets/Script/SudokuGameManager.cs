using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuGameManager : MonoBehaviour
{
    public static SudokuGameManager instance { get; private set; }

    public int sudokuSize { get; private set; }

    public GameObject sudokuCanvas;
    private SudokuGrid sudokuGrid;

    public GameObject mainMenuCanvas;
    private SudokuBoard sudokuBoard;

    private int MaxErrorCount = 2;

    public GameData gameData;

    private void Awake()
    {
        gameData = new GameData();
        instance = this;

        gameData.Awake();

        initMember();

        if (gameData.GameState == GameData.EGameState.start)
        {
            mainMenuCanvas.SetActive(false);
            sudokuCanvas.SetActive(true);
        }
    }

    private void initMember()
    {
        sudokuSize = 9;
    }

    public void OnBTNPlay()
    {
        gameData.ResetAll();
        gameData.SetGameState(GameData.EGameState.idle);
        mainMenuCanvas.SetActive(false);
        sudokuCanvas.SetActive(true);
    }

    public void OnBTNNew()
    {
        gameData.ResetAll();
        gameData.SetGameState(GameData.EGameState.start);
    }

    public bool OnBTNRestart()
    {
        if (gameData.GameState == GameData.EGameState.idle)
        {
            return false;
        }
        
        gameData.Restart();
        return true;
    }

    public void OnBTNBack()
    {
        gameData.ResetAll();
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
        gameData.SetPickupCoordinate(coordinate);
        sudokuGrid.OnSelect();
    }

    public void OnBTNInputNumber(int _number)
    {
        if (!IsGameProgress())
        {
            return;
        }
        var result = sudokuGrid.doInputNumber(gameData.PickupCoordinate, gameData.IsDraft, _number);
        if (result == 2)
        {
            gameData.IncErrorCount();
            sudokuBoard.OnShowErrorCount();
            if (gameData.ErrorCount > MaxErrorCount)
            {
                OnFail();
            }
        }
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

    public bool IsGameProgress()
    {
        return gameData.GameState == GameData.EGameState.start && !gameData.IsPaused;
    }

    public void OnBTNClear()
    {
        if (!IsGameProgress())
        {
            return;
        }
        sudokuGrid.doClearCell(gameData.PickupCoordinate);

        gameData.ResetPickupCoordinate();
    }

    public void OnBTNMusicOpen(bool _isOpen)
    {
        gameData.SetMusic(_isOpen);
    }

    public void OnBTNDraftOpen(bool _isOpen)
    {
        gameData.SetIsDraft(_isOpen);
    }

    public void OnBTNPause()
    {
        if (gameData.GameState == GameData.EGameState.start)
        {
            gameData.SetPause(!gameData.IsPaused);
            sudokuBoard.OnPause();
        }
    }

    void Start()
    {
        InvokeRepeating(nameof(OnTimerElapsed), 1, 1);
        initSudoGameObject();
    }

    void Update()
    {
        gameData.Update();
    }

    private void OnTimerElapsed()
    {
        if (!IsGameProgress())
        {
            return;
        }
        gameData.IncrSpendTimeInSeconds();
        sudokuBoard.OnTimerElapsed();
    }

    public void OnSuccess()
    {
        gameData.SetGameState(GameData.EGameState.success);
        sudokuBoard.OnPause();
        sudokuBoard.doSuccess();
    }

    public void OnFail()
    {
        gameData.SetGameState(GameData.EGameState.fail);
        gameData.SetPause(false);
        sudokuBoard.OnPause();
        sudokuBoard.doDefeate();
    }

}
