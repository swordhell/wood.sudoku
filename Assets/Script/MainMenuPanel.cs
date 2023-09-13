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
        btnPlay.onClick.AddListener(OnPlayGame);

        sldDifficulty.onValueChanged.AddListener(OnDifficultyChanged);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDifficultyChanged(float value)
    {
        SudokuGameManager.instance.SetDifficulty(value);
    }

    public void OnPlayGame()
    {
        SudokuGameManager.instance.OnPlayGame();
    }
}
