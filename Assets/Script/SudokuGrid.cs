using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SudokuGrid : MonoBehaviour
{
    public bool IsGenerate { get; private set; }

    public SudokuSubGrid[,] subGrids { get; private set; }
    public SudokuCell[] cells;
    public GameObject subGridPrefabRef;
    public GameObject cellPrefabRef;
    public GameData gameData;

    void Start()
    {
        gameData = SudokuGameManager.instance.gameData;
        subGrids = new SudokuSubGrid[3, 3];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject subGridPrefabObj = (GameObject)Instantiate(
                    subGridPrefabRef,
                    Vector3.zero,
                    Quaternion.identity
                );

                var grid = subGridPrefabObj.GetComponent<SudokuSubGrid>();

                subGrids[i, j] = grid;
                subGrids[i, j].SetCoordinate(i, j);
                subGrids[i, j].InitCells(subGridPrefabObj, cellPrefabRef);

                subGridPrefabObj.transform.SetParent(gameObject.transform);
                subGridPrefabObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
        cells = GetComponentsInChildren<SudokuCell>();
        IsGenerate = false;

        if (gameData.GameState == GameData.EGameState.start)
        {
            OnLoadLastDate();
        }
        else
        {
            gameData.ResetAll();
        }
    }

    void OnLoadLastDate()
    {

        InitButtons();
        InitResultButtons();
        InitDraft();

        OnSelect();
    }

    void Update() { }

    void Awake()
    {
    }

    public void GridInit()
    {
        if (IsGenerate)
        {
            return;
        }

        ClearAllDraftNumberBold();

        IsGenerate = true;
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

        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            FillCell(row, col, 0);
        }
        ClearAllDraftNumberBold();
        InitButtons();
        IsGenerate = false;
    }

    public void GridCleanup()
    {
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            FillCell(row, col, 0);
        }
        ClearAllDraftNumberBold();
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

            if (gameData.GridNumber[row, col] == 0)
            {
                ShuffleArray(rowList);
                foreach (var value in rowList)
                {
                    if (!RowContainsNumber(row, value) && !ColumnContainsNumber(col, value))
                    {
                        if (!SquarContainsNumber(row, col, value))
                        {
                            gameData.SetGridNumber(row, col, value);
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

        gameData.SetGridNumber(row, col, 0);
        return false;
    }

    bool IsValidate()
    {
        for (int i = 0; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            for (int j = 0; j < SudokuGameManager.instance.sudokuSize; j++)
            {
                if (gameData.GridNumber[i, j] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    void CreatePuzzle()
    {
        System.Array.Copy(gameData.GridNumber, gameData.PuzzleNumber, gameData.GridNumber.Length);

        Dictionary<int, List<Vector2Int>> myMap = new Dictionary<int, List<Vector2Int>>();
        for (int i = 0; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            for (int j = 0; j < SudokuGameManager.instance.sudokuSize; j++)
            {
                var value = gameData.GridNumber[i, j];
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

        for (int i = 0; i < SudokuGameManager.instance.gameData.Difficulty; i++)
        {
            Vector2Int pos = candidata[i];
            gameData.SetPuzzleNumber(pos.x, pos.y, 0);
        }

        gameData.DumpPuzzleNumber();
    }

    // 检查列中是否存在数字
    bool ColumnContainsNumber(int col, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (gameData.GridNumber[i, col] == value)
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
            if (gameData.GridNumber[row, i] == value)
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
                if (gameData.GridNumber[offsetRow + i, offsetCol + j] == value)
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
                int number = gameData.PuzzleNumber[i, j];
                FillCell(i, j, number);
            }
        }
    }

    void InitResultButtons()
    {
        for (int i = 0; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            for (int j = 0; j < SudokuGameManager.instance.sudokuSize; j++)
            {
                int number = gameData.ResultNumber[i, j];
                if (number != 0)
                {
                    FillResultCell(i, j, number);
                }
            }
        }
    }

    void InitDraft()
    {

        for (int i = 0; i < SudokuGameManager.instance.sudokuSize; i++)
        {
            for (int j = 0; j < SudokuGameManager.instance.sudokuSize; j++)
            {
                var draft = gameData.DraftNumber[i, j];

                SudokuCell cell = GetCell(i, j);
                if (cell == null)
                {
                    continue;
                }

                foreach (var node in draft.numbers)
                {
                    cell.DoInputDraftNumber(node, false);
                }
            }
        }
    }
    void FillCell(int row, int col, int value)
    {
        SudokuCell cell = GetCell(row, col);
        if (cell == null)
        {
            return;
        }
        cell.SetCellValue(value, true);
    }

    void FillResultCell(int row, int col, int value)
    {
        SudokuCell cell = GetCell(row, col);
        if (cell == null)
        {
            return;
        }
        cell.doInputNumber(value, gameData.ResultNumber[row, col] == gameData.GridNumber[row, col]);
        cell.doHideAllDraftNumber();
    }

    SudokuCell GetCell(int row, int col)
    {
        int gridRow = row / 3;
        int gridCol = col / 3;
        var grid = subGrids[gridRow, gridCol];
        if (grid == null)
        {
            return null;
        }

        return grid.cells[row % 3, col % 3];
    }

    void ClearAllDraftNumberBold()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].doHideAllDraftNumber();
            cells[i].doClearAllBold();
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

    public void doMarkGridByVector(Vector2Int newVec)
    {
        foreach (var cell in cells)
        {
            if (IsRelationPosition(cell.coordinate, newVec))
            {
                cell.SetSelectMask(SudokuCell.SelectMask.Relation);
            }
        }
    }

    public void doClearAllGrid()
    {

        foreach (var cell in cells)
        {
            cell.SetSelectMask(SudokuCell.SelectMask.None);
        }
    }

    public void doMarkRelation(int pickupNum)
    {
        if (pickupNum == 0)
        {
            return;
        }

        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;

            Vector2Int vec = new Vector2Int(row, col);
            int checkNumber = getConfirmNumber(vec);
            if (checkNumber == pickupNum)
            {
                doMarkGridByVector(vec);
            }
        }
    }

    public int getConfirmNumber(Vector2Int newVec)
    {
        int pickupNum = gameData.PuzzleNumber[newVec.x, newVec.y];
        if (pickupNum == 0)
        {
            return gameData.ResultNumber[newVec.x, newVec.y];
        }
        return pickupNum;
    }

    public void OnSelect()
    {
        Vector2Int newVec = SudokuGameManager.instance.gameData.PickupCoordinate;
        if (newVec.x == -1 || newVec.y == -1)
        {
            return;
        }
        doClearAllGrid();
        int pickupNum = getConfirmNumber(newVec);

        // 自动标记相关的格子（大量降低难度，先关闭）
        // doMarkRelation(pickupNum);

        if (pickupNum != 0)
        {
            doMarkGridByVector(newVec);
        }

        foreach (var cell in cells)
        {
            if (pickupNum == 0)
            {
                cell.doClearAllBold();
            }
            else
            {
                cell.doTryCheckBold(pickupNum);
            }

            if (cell.checkPosition(newVec.x, newVec.y))
            {
                cell.SetSelectMask(SudokuCell.SelectMask.Select);
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
        if (gameData.PuzzleNumber[coordinate.x, coordinate.y] != 0)
        {
            return 0;
        }

        if (isDraft)
        {
            if (gameData.ResultNumber[coordinate.x, coordinate.y] != 0)
            {
                return 0;
            }

            foreach (var cell in cells)
            {
                if (cell.checkPosition(coordinate.x, coordinate.y))
                {
                    cell.DoInputDraftNumber(_number, true);
                    break;
                }
            }
            gameData.SetDraftNumber(coordinate.x, coordinate.y, _number);
        }
        else
        {
            if (gameData.ResultNumber[coordinate.x, coordinate.y] == _number)
            {
                return 0;
            }

            if (gameData.ResultNumber[coordinate.x, coordinate.y] == gameData.GridNumber[coordinate.x, coordinate.y])
            {
                return 0;
            }

            gameData.SetResultNumber(coordinate.x, coordinate.y, _number);
            var isTrue = gameData.GridNumber[coordinate.x, coordinate.y] == _number;

            var pickCell = GetCell(coordinate.x, coordinate.y);
            if (pickCell)
            {
                pickCell.doInputNumber(_number, isTrue);
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
        int offsetCol = (coordinate.x / 3) * 3;
        int offsetRow = (coordinate.y / 3) * 3;

        for (int i = 0; i < 9; i++)
        {
            var colCell = GetCell(i, coordinate.y);
            if (colCell != null)
            {
                colCell.DoClearCellDraftNumber(number);
            }
            var rowCell = GetCell(coordinate.x, i);
            if (rowCell != null)
            {
                rowCell.DoClearCellDraftNumber(number);
            }
            var gridCell = GetCell(offsetCol + (i % 3), offsetRow + (i / 3));
            if (gridCell != null)
            {
                gridCell.DoClearCellDraftNumber(number);
            }
        }
    }

    private SudokuCell findCellByCoordinate(Vector2Int coordinate)
    {

        return GetCell(coordinate.x, coordinate.y);
    }

    public bool checkResultSuccess()
    {
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;

            if (gameData.PuzzleNumber[row, col] == 0 && gameData.ResultNumber[row, col] == 0)
            {
                return false;
            }

            if (gameData.ResultNumber[row, col] != 0 && gameData.ResultNumber[row, col] != gameData.GridNumber[row, col])
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

        if (gameData.PuzzleNumber[coordinate.x, coordinate.y] != 0)
        {
            return;
        }

        if (gameData.ResultNumber[coordinate.x, coordinate.y] != 0)
        {
            gameData.SetResultNumber(coordinate.x, coordinate.y, 0);
        }

        var c = GetCell(coordinate.x, coordinate.y);

        if (c != null)
        {
            c.doInputNumber(0, true);
        }
    }
}
