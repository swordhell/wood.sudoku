using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SudokuGrid : MonoBehaviour
{
    public bool IsGenerate { get; private set; }
    private int[,] gridNumber = new int[9, 9];
    private int[,] puzzleNumber = new int[9, 9];
    private int[,] resultNumber = new int[9, 9];

    public SudokuSubGrid[,] subGrids { get; private set; }
    public SudokuCell[] cells;
    private int stakeLayer = 0;
    public GameObject subGridPrefabRef;
    public GameObject cellPrefabRef;

    void Start()
    {
    }

    void Update()
    {

    }

    void Awake()
    {
        subGrids = new SudokuSubGrid[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject subGridPrefabObj = (GameObject)Instantiate(subGridPrefabRef, Vector3.zero, Quaternion.identity);

                var grid = subGridPrefabObj.GetComponent<SudokuSubGrid>();

                subGrids[i, j] = grid;
                subGrids[i, j].SetCoordinate(i, j);
                subGrids[i, j].InitCells(subGridPrefabObj, cellPrefabRef);

                subGridPrefabObj.transform.SetParent(gameObject.transform);
            }
        }
        cells = GetComponentsInChildren<SudokuCell>();
        IsGenerate = false;
        resetDynamicData();
    }

    public void Init()
    {
        if (IsGenerate)
        {
            return;
        }
        IsGenerate = true;
        resetDynamicData();
        FillGrid();

        CreatePuzzle();

        InitButtons();

        IsGenerate = false;
    }

    public void ReStart()
    {
        if (IsGenerate)
        {
            return;
        }
        IsGenerate = true;

        InitButtons();
        resultNumber = new int[9, 9];
        IsGenerate = false;
    }

    public void GridCleanup()
    {
        resetDynamicData();


        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            FillCell(row, col, 0);
        }
    }

    private void resetDynamicData()
    {
        gridNumber = new int[9, 9];
        puzzleNumber = new int[9, 9];
        resultNumber = new int[9, 9];
    }

    bool FillGrid()
    {
        int row = 0;
        int col = 0;
        List<int> rowList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < 81; i++)
        {
            row = i / 9;
            col = i % 9;

            if (gridNumber[row, col] == 0)
            {
                ShuffleArray(rowList);
                foreach (var value in rowList)
                {
                    if (!RowContainsNumber(row, value) && !ColumnContainsNumber(col, value))
                    {
                        if (!SquarContainsNumber(row, col, value))
                        {
                            gridNumber[row, col] = value;
                            if (IsValidate())
                            {
                                return true;
                            }
                            else
                            {
                                if (FillGrid())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                break;
            }
        }
        gridNumber[row, col] = 0;
        return false;
    }

    bool IsValidate()
    {
        for (int i = 0; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            for (int j = 0; j < SudokuGameManager.instance.sudokuSize; j++)
            {
                if (gridNumber[i, j] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    void CreatePuzzle()
    {
        System.Array.Copy(gridNumber, puzzleNumber, gridNumber.Length);

        Dictionary<int, List<Vector2Int>> myMap = new Dictionary<int, List<Vector2Int>>();
        for (int i = 0; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            for (int j = 0; j < SudokuGameManager.instance.sudokuSize; j++)
            {
                var value = gridNumber[i, j];
                if (myMap.ContainsKey(value))
                {
                    myMap[value].Add(new Vector2Int(i, j));
                }
                else
                {
                    myMap.Add(value, new List<Vector2Int> { new Vector2Int(i, j) });
                }
            }
        }

        for (int i = 1; i <= SudokuGameManager.instance.sudokuSize; i++)
        {
            ShuffleArray(myMap[i]);
        }

        List<Vector2Int> candidata = new List<Vector2Int>();
        for (int i = 0; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            for (int key = 1; key <= SudokuGameManager.instance.sudokuSize; key++)
            {
                candidata.Add(myMap[key][i]);
            }
        }

        for (int i = 0; i < SudokuGameManager.instance.difficulty; i++)
        {
            Vector2Int pos = candidata[i];
            puzzleNumber[pos.x, pos.y] = 0;
        }

    }

    // 检查列中是否存在数字
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

    // 检查行中是否存在数字
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
        var offsetRow = (row / 3) * 3;
        var offsetCol = (col / 3) * 3;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (gridNumber[offsetRow + i, offsetCol + j] == value)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 检测某个数字放入是否合法
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
                cells[i].SetCellValue(value);
                cells[i].doHideAllDraftNumber();
                return;
            }
        }
    }

    private static void ShuffleArray<T>(List<T> array)
    {
        int n = array.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i);

            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    public void doSelect(Vector2Int newVec)
    {
        int pickupNum = puzzleNumber[newVec.x, newVec.y];
        if (pickupNum == 0)
        {
            pickupNum = resultNumber[newVec.x, newVec.y];
        }
        foreach (var cell in cells)
        {
            cell.SetSelectMask(SudokuCell.SelectMask.None);
            if (IsRelationPosition(cell.coordinate, newVec))
            {
                cell.SetSelectMask(SudokuCell.SelectMask.Relation);
            }
            if (cell.checkPosition(newVec.x, newVec.y))
            {
                cell.SetSelectMask(SudokuCell.SelectMask.Select);
            }

            if (pickupNum == 0)
            {
                cell.doClearAllBold();
            }
            else
            {
                cell.doTryCheckBold(pickupNum);
            }
        }
    }

    public bool IsRelationPosition(Vector2Int a, Vector2Int b)
    {
        if (a.x == b.x || a.y == b.y)
        {
            return true;
        }

        return ((a.x / 3) == (b.x / 3)) && (a.y / 3) == (b.y / 3);
    }

    public int doInputNumber(Vector2Int coordinate, bool isDraft, int _number)
    {
        if (coordinate.x == -1 || coordinate.y == -1)
        {
            return 0;
        }
        if (puzzleNumber[coordinate.x, coordinate.y] != 0)
        {
            return 0;
        }

        if (isDraft)
        {
            if (resultNumber[coordinate.x, coordinate.y] != 0)
            {
                return 0;
            }

            foreach (var cell in cells)
            {
                if (cell.checkPosition(coordinate.x, coordinate.y))
                {
                    cell.doInputDraftNumber(_number);
                    break;
                }
            }
        }
        else
        {
            if (resultNumber[coordinate.x, coordinate.y] == _number)
            {
                return 0;
            }

            if (resultNumber[coordinate.x, coordinate.y] == gridNumber[coordinate.x, coordinate.y])
            {
                return 0;
            }

            resultNumber[coordinate.x, coordinate.y] = _number;
            var isTrue = gridNumber[coordinate.x, coordinate.y] == _number;
            foreach (var cell in cells)
            {
                if (cell.checkPosition(coordinate.x, coordinate.y))
                {
                    cell.doInputNumber(_number, isTrue);
                }
            }

            if (isTrue)
            {
                doClearRelationCellDraftNumber(coordinate, _number);
            }

            if (checkResultSuccess())
            {
                SudokuGameManager.instance.OnSuccess();
            }

            return isTrue ? 1 : 2;

        }
        return 0;
    }

    private void doClearRelationCellDraftNumber(Vector2Int coordinate, int number)
    {
        int offsetCol = coordinate.x / 3;
        int offsetRow = coordinate.y / 3;

        for (int i = 0; i < 9; i++)
        {
            var colCell = findCellByCoordinate(new Vector2Int(i, coordinate.y));
            if (colCell != null)
            {
                colCell.doClearCellDraftNumber(number);
            }
            var rowCell = findCellByCoordinate(new Vector2Int(coordinate.x, i));
            if (rowCell != null)
            {
                rowCell.doClearCellDraftNumber(number);
            }
            var gridCell = findCellByCoordinate(new Vector2Int((offsetCol * 3) + (i % 3), (offsetRow * 3) + i / 3));
            if (gridCell != null)
            {
                gridCell.doClearCellDraftNumber(number);
            }
        }
    }

    private SudokuCell findCellByCoordinate(Vector2Int coordinate)
    {
        foreach (var cell in cells)
        {
            if (cell.checkPosition(coordinate.x, coordinate.y))
            {
                return cell;
            }
        }
        return null;
    }

    public bool checkResultSuccess()
    {

        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;

            if (puzzleNumber[row, col] == 0 && resultNumber[row, col] == 0)
            {
                return false;
            }

            if (resultNumber[row, col] != 0 && resultNumber[row, col] != gridNumber[row, col])
            {
                return false;
            }
        }
        return true;
    }


    public void doClearCell(Vector2Int coordinate)
    {
        foreach (var cell in cells)
        {
            cell.SetSelectMask(SudokuCell.SelectMask.None);
            cell.doClearAllBold();
        }
        if (coordinate.x == -1 || coordinate.y == -1)
        {
            return;
        }

        if (puzzleNumber[coordinate.x, coordinate.y] != 0)
        {
            return;
        }

        if (resultNumber[coordinate.x, coordinate.y] != 0)
        {
            resultNumber[coordinate.x, coordinate.y] = 0;
        }
        foreach (var cell in cells)
        {
            if (cell.checkPosition(coordinate.x, coordinate.y))
            {
                cell.doInputNumber(0, true);
            }
        }

    }
}
