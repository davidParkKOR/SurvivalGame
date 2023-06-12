using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArchemyTooltip : MonoBehaviour
{
    [SerializeField] private Text text_NeedItemName;
    [SerializeField] private Text text_NeedItemNumber;
    [SerializeField] private GameObject go_BaseToolTip;

    void Clear()
    {
        text_NeedItemName.text = "";
        text_NeedItemNumber.text = "";
    }

    public void ShowTooltip(string[] _needItemName, int[] _needItemNumber)
    {
        Clear();
        go_BaseToolTip.SetActive(true);

        for (int i = 0; i < _needItemNumber.Length; i++)
        {
            text_NeedItemName.text += _needItemName[i] + "\n";
            text_NeedItemNumber.text += " x " + _needItemNumber[i] + "\n";

        }
    }

    public void HideTooltip()
    {
        Clear();
        go_BaseToolTip.SetActive(false);
    }
}


