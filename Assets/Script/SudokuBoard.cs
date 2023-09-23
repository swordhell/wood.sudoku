using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MidiPlayerTK;

public class SudokuBoard : MonoBehaviour
{
    public Button btnBack;
    public Button btnNew;
    public Button btnReStart;
    public Button btnPause;
    public Button btnClear;

    public GameObject grid;
    public SudokuGrid sudokuGrid;
    public GameObject pauseBoard;

    public Toggle tglDraft;

    public TextMeshProUGUI txtSpendTime;
    public TextMeshProUGUI txtErrorCount;

    public GameObject panelSuccess;
    public GameObject panelDeafeate;

    public MidiPlayerTK.MidiFilePlayer musicPlayerGO;
    public Toggle toggleMusic;
    public Toggle toggleDraft;

    private void Awake()
    {
        btnBack.onClick.AddListener(OnBTNBack);
        btnNew.onClick.AddListener(OnBTNNew);
        btnReStart.onClick.AddListener(OnBTNReStart);
        btnPause.onClick.AddListener(OnBTNPause);
        btnClear.onClick.AddListener(OnBTNClear);

        toggleMusic.onValueChanged.AddListener(OnBTNMusicOpen);
        toggleDraft.onValueChanged.AddListener(OnBTNDraftOpen);
    }

    void Start()
    {
        sudokuGrid = grid.GetComponent<SudokuGrid>();
    }

    void Update()
    {
    }

    public void OnBTNBack()
    {
        SudokuGameManager.instance.OnBTNBack();
        sudokuGrid.GridCleanup();
        ResetUI();
    }

    public void OnBTNNew()
    {
        SudokuGameManager.instance.OnBTNNew();
        ResetUI();

        sudokuGrid.Init();
    }

    public void OnBTNReStart()
    {
        SudokuGameManager.instance.OnBTNNew();
        ResetUI();

        sudokuGrid.ReStart();
    }

    private void ResetUI()
    {
        OnShowErrorCount();
        doPause(false);
        OnBTNMusicOpen(false);

        panelSuccess.SetActive(false);
        panelDeafeate.SetActive(false);
        toggleMusic.isOn = false;
        txtSpendTime.text = "00:00:00";
    }

    public void OnBTNPause()
    {
        SudokuGameManager.instance.OnBTNPause();
    }

    public void doPause(bool _enable)
    {
        pauseBoard.SetActive(_enable);
    }

    public void doSuccess()
    {
        panelSuccess.SetActive(true);
    }

    public void doDefeate()
    {
        panelDeafeate.SetActive(true);
    }

    public void OnBTNClear()
    {
        SudokuGameManager.instance.OnBTNClear();
    }

    public void OnBTNMusicOpen(bool _isOpen)
    {
        SudokuGameManager.instance.OnBTNMusicOpen(_isOpen);
        if (_isOpen)
        {
            musicPlayerGO.MPTK_UnPause();
            if (musicPlayerGO.state != fluid_synth_status.FLUID_SYNTH_PLAYING)
            {
                musicPlayerGO.MPTK_Play();
            }
        }
        else
        {
            musicPlayerGO.MPTK_Pause();
        }
    }

    public void OnBTNDraftOpen(bool _isOpen)
    {
        SudokuGameManager.instance.OnBTNDraftOpen(_isOpen);
    }

    public void OnTimerElapsed(int spendTimeInSeconds)
    {
        txtSpendTime.text = ConvertSecondsToTime(spendTimeInSeconds);
    }

    public static string ConvertSecondsToTime(int seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return timeSpan.ToString();
    }

    public void OnShowErrorCount()
    {
        txtErrorCount.text = string.Format("error: {0}", SudokuGameManager.instance.errorCount);
    }
}
