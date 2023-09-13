using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SudokuCell : MonoBehaviour
{
    public Vector2Int coordinate;
    public SudokuSubGrid subGrid;
    int value = 0;

    public TextMeshProUGUI txtNumber;
    public Button btnNum;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        btnNum.onClick.AddListener(OnNumClick);
    }

    void OnNumClick()
    {

    }
    public void InitValues(int _value)
    {
        if (_value != 0)
        {
            txtNumber.text = _value.ToString();
            txtNumber.color = (Color)new Color32(119, 110, 101, 255);
            txtNumber.gameObject.SetActive(true);
            btnNum.gameObject.SetActive(false);
        }
        else
        {
            txtNumber.gameObject.SetActive(false);
            btnNum.gameObject.SetActive(true);
            txtNumber.text = "";
            txtNumber.color = (Color)new Color32(233, 213, 215, 255);
        }
        value = _value;
    }

    public void SetCoordinate(int row, int col)
    {
        coordinate = new Vector2Int(row, col);
    }

    public bool checkPosition(int row, int col)
{
    return coordinate.x == row && coordinate.y == col;

}
    public void SetSubGrid(SudokuSubGrid _subGrid)
    {
        subGrid = _subGrid;
    }
}
