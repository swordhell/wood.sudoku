using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGrid : MonoBehaviour
{
    public GameObject sudokuGrid;
    private int[,] gridNumber = new int[9, 9];
    private int[,] puzzleNumber = new int[9, 9];

    public SudokuSubGrid[,] subGrids { get; private set; }
    public SudokuCell[] cells;
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
        var grid = GetComponentsInChildren<SudokuSubGrid>();

        subGrids = new SudokuSubGrid[3, 3];
        int index = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                subGrids[i, j] = grid[index++];
                subGrids[i, j].SetCoordinate(i, j);
                subGrids[i, j].InitCells();
            }
        }
        cells = GetComponentsInChildren<SudokuCell>();
    }

    public void Init()
    {
        CreateGrid();

        CreatePuzzle();

        InitButtons();
    }

    void CreateGrid()
    {
        gridNumber = new int[9, 9];
        List<int> rowList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> colList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        int value = rowList[Random.Range(0, rowList.Count)];
        gridNumber[0, 0] = value;
        rowList.Remove(value);
        colList.Remove(value);

        for (int i = 1; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            value = rowList[Random.Range(0, rowList.Count)];
            gridNumber[i, 0] = value;
            rowList.Remove(value);
        }

        for (int i = 1; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            value = colList[Random.Range(0, colList.Count)];
            if (i < 3)
            {
                while (SquarContainsNumber(0, 0, value))
                {
                    value = colList[Random.Range(0, colList.Count)];
                }
            }
            gridNumber[0, i] = value;
            colList.Remove(value);
        }

        for (int i = 6; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            value = Random.Range(1, 10);
            while (SquarContainsNumber(i, i, value) || RowContainsNumber(i, value) || ColumnContainsNumber(i, value))
            {
                value = Random.Range(1, 10);
            }
            gridNumber[i, i] = value;
        }

        SolveSudoku();
    }

    bool ColumnContainsNumber(int col, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (gridNumber[i, col] == value)
            {
                return true;
            }
        }
        return false;
    }

    bool RowContainsNumber(int row, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (gridNumber[row, i] == value)
            {
                return true;
            }
        }
        return false;
    }

    // 计算九宫格中是否已经存在
    bool SquarContainsNumber(int row, int col, int value)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (gridNumber[(row / 3) * 3 + i, (col / 3) * 3 + j] == value)
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckAll(int row, int col, int value)
    {
        if (ColumnContainsNumber(col, value))
        {
            return false;
        }
        if (RowContainsNumber(row, value))
        {
            return false;
        }
        if (SquarContainsNumber(row, col, value))
        {
            return false;
        }
        return true;
    }

    bool IsValidate()
    {
        for (int i = 1; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            for (int j = 1; j < SudokuGameManager.instance.sudokuSize; j++)
            {
                if (gridNumber[i, j] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    bool SolveSudoku()
    {
        int row = 0;
        int col = 0;

        if (IsValidate())
        {
            return true;
        }

        for (int i = 1; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            for (int j = 1; j < SudokuGameManager.instance.sudokuSize; j++)
            {
                if (gridNumber[i, j] == 0)
                {
                    row = i;
                    col = j;
                    break;
                }
            }
            if (row != 0 || col != 0)
            {
                break;
            }
        }

        for (int i = 1; i <= SudokuGameManager.instance.sudokuSize; i++)
        {
            if (CheckAll(row, col, i))
            {
                gridNumber[row, col] = i;
                if (SolveSudoku())
                {
                    return true;
                }
            }
            else
            {
                gridNumber[row, col] = 0;
            }
        }

        return false;
    }

    void CreatePuzzle()
    {
        System.Array.Copy(gridNumber, puzzleNumber, gridNumber.Length);


        HashSet<int> uniqueNumber = new HashSet<int>();

        for (int i = 0; i < SudokuGameManager.instance.difficulty; i++)
        {
            int row = Random.Range(0, SudokuGameManager.instance.sudokuSize);
            int col = Random.Range(0, SudokuGameManager.instance.sudokuSize);

            while (puzzleNumber[row, col] == 0)
            {
                row = Random.Range(1, SudokuGameManager.instance.sudokuSize);
                col = Random.Range(1, SudokuGameManager.instance.sudokuSize);
            }
            uniqueNumber.Add(puzzleNumber[row, col]);
            puzzleNumber[row, col] = 0;
        }

        // HashSet<int> notExists = new HashSet<int>();
        // for (int i = 1; i <= 9; i++)
        // {
        //     if (!uniqueNumber.Contains(i))
        //     {
        //         notExists.Add(i);
        //     }
        // }

        // // 至少确保有8个不同种类的数字，否则就不能做出唯一解；
        // while (notExists.Count > 2)
        // {
        //     int row = Random.Range(0, SudokuGameManager.instance.sudokuSize);
        //     int col = Random.Range(0, SudokuGameManager.instance.sudokuSize);

        //     int value = gridNumber[row, col] ;
        //     if (notExists.Contains(value))
        //     {
        //         puzzleNumber[row,col] = gridNumber[row, col];
        //         notExists.Remove(value);
        //     }
        // }
    }

    void InitButtons()
    {
        for (int i = 0; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            for (int j = 0; j < SudokuGameManager.instance.sudokuSize; j++)
            {
                int number = puzzleNumber[i, j];
                FillCell(i, j, number);
            }
        }
    }

    void FillCell(int row, int col, int value)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].checkPosition(row, col))
            {
                cells[i].InitValues(value);
                return;
            }
        }
    }
}
