using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ComputerToolTip : MonoBehaviour
{
    [SerializeField] private GameObject go_BaseUI;
    [SerializeField] private Text kitName;
    [SerializeField] private Text kitDescription;
    [SerializeField] private Text kitNeedItem;

    public void ShowToolTip(string _kitName, string _kitDes, string[] _needItems, int[] needItemNumber)
    {
        go_BaseUI.SetActive(true);

        kitName.text = _kitName;
        kitDescription.text = _kitDes;

        for (int i = 0; i < _needItems.Length; i++)
        {
            kitNeedItem.text += _needItems[i];
            kitNeedItem.text += " x " + needItemNumber[i].ToString() + "\n";
        }

    }

    public void HideToolTip()
    {
        go_BaseUI.SetActive(false);
        kitName.text = "";
        kitDescription.text = "";
        kitNeedItem.text = "";
    }
    
}
