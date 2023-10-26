using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CellDraft
{
    public List<int> numbers;

    public void FromString(string input)
    {
        numbers = new List<int>();
        string[] numbs = input.Split(',');

        foreach (string number in numbs)
        {
            int temp;
            if (Int32.TryParse(number, out temp))
            {
                numbers.Add(temp);
            }
        }
    }

    public void RemoveOrAddNumber(int _number)
    {
        bool isRemove = RemoveNumber(_number);
        if (isRemove)
        {
            return;
        }
        numbers.Add(_number);
    }

    public bool RemoveNumber(int _number)
    {
        bool have = false;
        List<int> tmpnumbers = new List<int>();
        foreach (var number in numbers)
        {
            if (number == _number)
            {
                have = true;
            }
            if (number != _number)
            {
                tmpnumbers.Add(number);
            }
        }
        return have;
    }

    override public string ToString()
    {
        string stringList = String.Join(",", numbers);
        return stringList;
    }
}

public class GameData
{
    const string KeyGameState = "GameState";
    const string KeyDifficulty = "Difficulty";
    const string KeySpendTimeInSeconds = "spendTimeInSeconds";
    const string KeyErrorCount = "errorCount";
    const string KeyIsMusic = "IsMusic";
    const string KeyIsPaused = "IsPaused";

    const string KeyGridNumber = "gridNumber";
    const string KeyPuzzleNumber = "puzzleNumber";
    const string KeyResultNumber = "resultNumber";
    const string KeyPickupCoordinate = "PickupCoordinate";

    const string KeyIsDraft = "IsDraft";

    const string KeyCellDraft = "CellDraft";

    private bool isDirty;

    public enum EGameState
    {
        idle,
        start,
        fail,
        success,
    }
    public EGameState GameState { get; set; }
    public int Difficulty { get; set; }
    public int SpendTimeInSeconds { get; set; }
    public int ErrorCount { get; set; }
    public bool IsMusic { get; set; }
    public bool IsPaused { get; set; }

    public int[,] GridNumber = new int[9, 9];
    public int[,] PuzzleNumber = new int[9, 9];
    public int[,] ResultNumber = new int[9, 9];
    public Vector2Int PickupCoordinate;

    public bool IsDraft { get; set; }

    public CellDraft[,] DraftNumber = new CellDraft[9, 9];

    public void SetGameState(EGameState value)
    {
        GameState = value;
        PlayerPrefs.SetInt(KeyGameState, (int)GameState);
        isDirty = true;
    }

    public void SetDifficulty(float value)
    {
        Difficulty = (int)value;
        PlayerPrefs.SetInt(KeyDifficulty, Difficulty);
        isDirty = true;
    }

    public void SetSpendTimeInSeconds(int sec)
    {
        SpendTimeInSeconds = sec;
        PlayerPrefs.SetInt(KeySpendTimeInSeconds, SpendTimeInSeconds);
        isDirty = true;
    }

    public void IncrSpendTimeInSeconds()
    {
        SpendTimeInSeconds++;
        PlayerPrefs.SetInt(KeySpendTimeInSeconds, SpendTimeInSeconds);
        isDirty = true;
    }

    public void SetErrorCount(int count)
    {
        ErrorCount = count;
        PlayerPrefs.SetInt(KeyErrorCount, ErrorCount);
        isDirty = true;
    }

    public void IncErrorCount()
    {
        ErrorCount++;
        PlayerPrefs.SetInt(KeyErrorCount, ErrorCount);
        isDirty = true;
    }

    public void SetMusic(bool value)
    {
        IsMusic = value;
        PlayerPrefs.SetInt(KeyIsMusic, value ? 1 : 0);
        isDirty = true;
    }

    public void SetPause(bool value)
    {
        IsPaused = value;
        PlayerPrefs.SetInt(KeyIsPaused, value ? 1 : 0);
        isDirty = true;
    }

    public void LoadGridNumber()
    {
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            GridNumber[row, col] = PlayerPrefs.GetInt(KeyGridNumber + $"_${row}_${col}", 0);
        }
    }

    public void SetGridNumber(int row, int col, int value)
    {
        GridNumber[row, col] = value;
        PlayerPrefs.SetInt(KeyGridNumber + $"_${row}_${col}", value);
        isDirty = true;
    }

    public void ResetGridNumber()
    {
        GridNumber = new int[9, 9];
        DumpGridNumber();
    }

    public void DumpGridNumber()
    {
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            SetGridNumber(row, col, GridNumber[row, col]);
        }
        isDirty = true;
    }

    public void ResetPuzzleNumber()
    {
        PuzzleNumber = new int[9, 9];
        DumpPuzzleNumber();
    }

    public void DumpPuzzleNumber()
    {
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            SetPuzzleNumber(row, col, PuzzleNumber[row, col]);
        }
        isDirty = true;
    }

    public void LoadPuzzleNumber()
    {
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            PuzzleNumber[row, col] = PlayerPrefs.GetInt(KeyPuzzleNumber + $"_${row}_${col}", 0);
        }
    }

    public void SetPuzzleNumber(int row, int col, int value)
    {
        PuzzleNumber[row, col] = value;
        PlayerPrefs.SetInt(KeyPuzzleNumber + $"_${row}_${col}", value);
        isDirty = true;
    }

    public void ResetResultNumber()
    {
        ResultNumber = new int[9, 9];
        DumpResultNumber();
    }

    public void DumpResultNumber()
    {
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            SetResultNumber(row, col, ResultNumber[row, col]);
        }
        isDirty = true;
    }

    public void LoadResultNumber()
    {
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            ResultNumber[row, col] = PlayerPrefs.GetInt(KeyResultNumber + $"_${row}_${col}", 0);
        }
    }

    public void SetResultNumber(int row, int col, int value)
    {
        ResultNumber[row, col] = value;
        PlayerPrefs.SetInt(KeyResultNumber + $"_${row}_${col}", value);
        isDirty = true;
    }

    public void LoadDraftNumber()
    {
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            var node = new CellDraft();
            node.FromString(PlayerPrefs.GetString(KeyCellDraft + $"_${row}_${col}", ""));
            DraftNumber[row, col] = node;
        }
    }

    public void ResetDraftNumber()
    {
        for (int i = 0; i < 81; i++)
        {
            int row = i / 9;
            int col = i % 9;
            ClearDraftNumber(row, col);
        }
    }

    public void RemoveDrafNumber(int row, int col, int value)
    {
        DraftNumber[row, col].RemoveNumber(value);
        PlayerPrefs.SetString(KeyCellDraft + $"_${row}_${col}", DraftNumber[row, col].ToString());
        isDirty = true;
    }

    public void ClearDraftNumber(int row, int col)
    {
        DraftNumber[row, col].numbers = new List<int>();
        PlayerPrefs.SetString(KeyCellDraft + $"_${row}_${col}", DraftNumber[row, col].ToString());
        isDirty = true;
    }

    public void AddDraftNumber(int row, int col, int value)
    {
        DraftNumber[row, col].numbers.Add(value);
        PlayerPrefs.SetString(KeyCellDraft + $"_${row}_${col}", DraftNumber[row, col].ToString());
        isDirty = true;
    }

    public void SetDraftNumber(int row, int col, int number)
    {
        DraftNumber[row, col].RemoveOrAddNumber(number);
        PlayerPrefs.SetString(KeyCellDraft + $"_${row}_${col}", DraftNumber[row, col].ToString());
        isDirty = true;
    }

    public void SetIsDraft(bool _value)
    {
        IsDraft = _value;
        PlayerPrefs.SetInt(KeyIsDraft, _value ? 1 : 0);
        isDirty = true;
    }

    public void Awake()
    {
        GameState = (EGameState)PlayerPrefs.GetInt(KeyGameState, 0);
        Difficulty = PlayerPrefs.GetInt(KeyDifficulty, 40);
        SpendTimeInSeconds = PlayerPrefs.GetInt(KeySpendTimeInSeconds, 0);
        ErrorCount = PlayerPrefs.GetInt(KeyErrorCount, 0);
        IsMusic = PlayerPrefs.GetInt(KeyIsMusic, 0) != 0;
        IsPaused = PlayerPrefs.GetInt(KeyIsPaused, 0) != 0;

        LoadGridNumber();
        LoadPuzzleNumber();
        LoadResultNumber();
        PickupCoordinate = ConvertStringToVector2Int(PlayerPrefs.GetString(KeyPickupCoordinate, "{-1,-1}"));

        IsDraft = PlayerPrefs.GetInt(KeyIsDraft, 0) != 0;
        LoadDraftNumber();

        isDirty = false;
    }

    public void Update()
    {
        if (isDirty)
        {
            PlayerPrefs.Save();
            isDirty = false;
        }
    }

    public void ResetPickupCoordinate()
    {
        PickupCoordinate.x = -1;
        PickupCoordinate.y = -1;
        PlayerPrefs.SetString(KeyPickupCoordinate, ConvertVector2IntToString(PickupCoordinate));
        isDirty = true;
    }

    public void Restart()
    {
        SetErrorCount(0);
        SetSpendTimeInSeconds(0);
        SetPause(false);
        ResetResultNumber();
        ResetDraftNumber();
    }

    public void ResetAll()
    {
        SetErrorCount(0);
        SetSpendTimeInSeconds(0);
        SetPause(false);
        ResetGridNumber();
        ResetResultNumber();
        ResetDraftNumber();
    }

    public void SetPickupCoordinate(Vector2Int vect)
    {
        PickupCoordinate = vect;
        PlayerPrefs.SetString(KeyPickupCoordinate, ConvertVector2IntToString(PickupCoordinate));
        isDirty = true;
    }

    static public Vector2Int ConvertStringToVector2Int(string input)
    {
        // 使用正则表达式匹配并提取字符串中的两个整数  
        Match match = Regex.Match(input, @"(-?\d+),(-?\d+)");

        if (match.Success)
        {
            int x = int.Parse(match.Groups[1].Value);
            int y = int.Parse(match.Groups[2].Value);

            return new Vector2Int(x, y);
        }
        else
        {
            return new Vector2Int(-1, -1);
        }
    }

    static public string ConvertVector2IntToString(Vector2Int vector)
    {
        return $"{{{vector.x},{vector.y}}}";
    }

}