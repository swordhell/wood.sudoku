using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SudokuCell : MonoBehaviour
{
    public enum SelectMask : int
    {
        None = 0,
        Select = 1,
        Relation = 2,
    }
    public Vector2Int coordinate;
    public SudokuSubGrid subGrid;
    public int value { get; private set; }

    public TextMeshProUGUI txtNumber;
    public GameObject draftPanel;
    public TextMeshProUGUI[] txtDraftNumber;
    public Button btnNum;

    public Color colorInputNormal = (Color)new Color32(0x2C, 0x52, 0xCF, 255);
    public Color colorInputError = (Color)new Color32(0xFF, 0x10, 0x31, 255);

    private UnityEngine.UI.Image bgImage;
    public Color colorSelect = new Color32(0xB1, 0xC5, 0xFF, 255);
    public Color colorRelation = new Color32(0xE9, 0xEE, 0xFE, 0xFF);
    public Color colorUnselect = new Color32(0xE9, 0xDF, 0xD7, 255);

    void Start()
    {
        bgImage = gameObject.GetComponent<UnityEngine.UI.Image>();

        for (int i = 0; i < txtDraftNumber.Length; i++)
        {
            txtDraftNumber[i].text = (i + 1).ToString();
            txtDraftNumber[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
    }

    void Awake()
    {
        btnNum.onClick.AddListener(OnBTNSelect);
    }

    void OnBTNSelect()
    {
        SudokuGameManager.instance.OnBTNSelect(coordinate);
    }
    public void SetCellValue(int _value)
    {
        btnNum.gameObject.SetActive(true);
        if (_value != 0)
        {
            txtNumber.text = _value.ToString();
            txtNumber.color = (Color)new Color32(119, 110, 101, 255);
        }
        else
        {
            txtNumber.text = " ";
            txtNumber.color = colorInputNormal;
        }
        value = _value;
        SetSelectMask(SudokuCell.SelectMask.None);
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

    public void SetSelectMask(SelectMask _mode)
    {
        switch (_mode)
        {
            case SelectMask.None:
                bgImage.color = colorUnselect;
                break;
            case SelectMask.Select:
                bgImage.color = colorSelect;
                break;
            case SelectMask.Relation:
                bgImage.color = colorRelation;
                break;
        }
    }

    public void doInputNumber(int _value, bool isTrue)
    {
        doHideAllDraftNumber();
        if (_value == 0)
        {
            txtNumber.text = " ";
        }
        else
        {
            txtNumber.text = _value.ToString();
        }

        if (isTrue)
        {
            txtNumber.color = colorInputNormal;
        }
        else
        {
            txtNumber.color = colorInputError;
        }
    }

    public void doInputDraftNumber(int _value)
    {
        if (_value < 1)
        {
            return;
        }
        var txt = txtDraftNumber[_value - 1];
        txtDraftNumber[_value - 1].gameObject.SetActive(!txt.gameObject.activeSelf);
    }

    public void doClearCellDraftNumber(int _value)
    {
        if (_value < 1)
        {
            return;
        }
        txtDraftNumber[_value - 1].gameObject.SetActive(false);
    }

    public void doHideAllDraftNumber()
    {
        foreach (var txt in txtDraftNumber)
        {
            txt.gameObject.SetActive(false);
        }
    }

    public void doBoldDraftNumber(int _value, bool _isBold)
    {
        if (_value < 1)
        {
            return;
        }

        if (_isBold)
        {
            txtDraftNumber[_value - 1].fontStyle |= FontStyles.Bold;
        }
        else
        {
            txtDraftNumber[_value - 1].fontStyle = FontStyles.Normal;
        }
    }

    public void doBoldNumber(bool _isBold)
    {
        if (_isBold)
        {
            txtNumber.fontStyle |= FontStyles.Bold;
        }
        else
        {
            txtNumber.fontStyle = FontStyles.Normal;
        }
    }

    public void doClearAllBold()
    {
        txtNumber.fontStyle = FontStyles.Normal;
        foreach (var txt in txtDraftNumber)
        {
            txt.fontStyle = FontStyles.Normal;
        }
    }

    public void doTryCheckBold(int num)
    {
        var inputNum = num.ToString();
        if (txtNumber.text == inputNum)
        {
            txtNumber.fontStyle |= FontStyles.Bold;
            SetSelectMask(SudokuCell.SelectMask.Relation);
        }
        else
        {
            txtNumber.fontStyle = FontStyles.Normal;
        }

        for (int i = 0; i < txtDraftNumber.Length; i++)
        {
            if ((i + 1) == num)
            {
                txtDraftNumber[i].fontStyle |= FontStyles.Bold;
            }
            else
            {
                txtDraftNumber[i].fontStyle = FontStyles.Normal;
            }
        }
    }
}
