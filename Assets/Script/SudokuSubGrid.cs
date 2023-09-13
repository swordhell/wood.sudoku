using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuSubGrid : MonoBehaviour
{
    public Vector2Int coordinate;

    public SudokuCell[,] cells { get; private set; }

    private void Awake()
    {
        cells = new SudokuCell[9, 9];
    }

    public void SetCoordinate(int row, int col)
    {
        coordinate = new Vector2Int(row, col);
    }

    public void InitCells()
    {
        var vcells = GetComponentsInChildren<SudokuCell>();
        cells = new SudokuCell[3, 3];

        int index = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cells[i, j] = vcells[index++];
                cells[i, j].SetCoordinate(i + coordinate.x * 3, j + coordinate.y * 3);
                cells[i, j].SetSubGrid(this);
            }
        }
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
