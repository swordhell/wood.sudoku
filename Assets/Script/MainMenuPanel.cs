using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    public Button btnPlay;
    public Slider sldDifficulty;

    private void Awake()
    {
    }

    void Start()
    {
        btnPlay.onClick.AddListener(OnBTNPlay);
        sldDifficulty.onValueChanged.AddListener(OnDifficultyChanged);
        
        sldDifficulty.value = (float)(SudokuGameManager.instance.gameData.Difficulty);
    }

    void Update()
    {
    }

    public void OnDifficultyChanged(float value)
    {
        SudokuGameManager.instance.gameData.SetDifficulty(value);
    }

    public void OnBTNPlay()
    {
        SudokuGameManager.instance.OnBTNPlay();
    }
}
