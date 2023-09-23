using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetNumberPanel : MonoBehaviour
{
    public void Awake()
    {
        var tmpButtons = GetComponentsInChildren<Button>();
        if (tmpButtons.Length < 9)
        {
            Debug.Log("SetNumberPanel.Awake(): Not enough buttons");
            return;
        }

        for (int i = 0; i < tmpButtons.Length; i++)
        {
            int num = i + 1;
            tmpButtons[i].onClick.AddListener(() =>
            {
                OnBTNInputNumber(num);
            });
        }

    }
    void Start()
    {
    }

    void Update()
    {
    }

    void OnBTNInputNumber(int num)
    {
        SudokuGameManager.instance.OnBTNInputNumber(num);
    }
}
