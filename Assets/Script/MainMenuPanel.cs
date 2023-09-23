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
        btnPlay.onClick.AddListener(OnBTNPlay);
        sldDifficulty.onValueChanged.AddListener(OnDifficultyChanged);
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnDifficultyChanged(float value)
    {
        SudokuGameManager.instance.SetDifficulty(value);
    }

    public void OnBTNPlay()
    {
        SudokuGameManager.instance.OnBTNPlay();
    }
}
