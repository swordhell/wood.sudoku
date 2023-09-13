using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuBoard : MonoBehaviour
{
    public Button btnBack;
    public Button btnNew;
    public Button btnReStart;
    public Button btnSubmit;

    public GameObject grid;
    private void Awake()
    {
        btnBack.onClick.AddListener(OnBack);
        btnNew.onClick.AddListener(OnNew);
        btnReStart.onClick.AddListener(OnReStart);
        btnSubmit.onClick.AddListener(OnSubmit);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBack()
    {
        SudokuGameManager.instance.OnBackToMainMenu();
    }

    public void OnNew()
    {
        grid.GetComponent<SudokuGrid>().Init();
    }

    public void OnReStart()
    {

    }

    public void OnSubmit()
    {

    }
}
