using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    public void InitCells(GameObject subGridPrefabObj, GameObject cellPrefabRef)
    {
        cells = new SudokuCell[3, 3];

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject cellPrefabObj = (GameObject)Instantiate(cellPrefabRef, new Vector3(0, 0, 0), Quaternion.identity);

                var cell = cellPrefabObj.GetComponent<SudokuCell>();

                cells[i, j] = cell;
                cells[i, j].SetCoordinate(i + coordinate.x * 3, j + coordinate.y * 3);
                cells[i, j].SetSubGrid(this);

                cellPrefabObj.transform.SetParent(subGridPrefabObj.transform);
            }
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
