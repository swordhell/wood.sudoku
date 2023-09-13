using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuGameManager : MonoBehaviour
{
    public static SudokuGameManager instance { get; private set; }

    public int sudokuSize { get; set; }

    public GameObject sudokuCanvas;
    public GameObject mainMenuCanvas;

    public int difficulty { get; set; }
    private void Awake()
    {
        instance = this;
        OnBackToMainMenu();
        sudokuSize = 9;
        difficulty = 40;
    }

    public void SetDifficulty(float value)
    {
        difficulty = (int)value;
    }

    public void OnPlayGame()
    {
        mainMenuCanvas.SetActive(false);
        sudokuCanvas.SetActive(true);

    }

    public void OnBackToMainMenu()
    {
        sudokuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
