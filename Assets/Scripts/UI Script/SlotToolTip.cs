using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotToolTip : MonoBehaviour
{
    [SerializeField]
    private GameObject go_Base;

    [SerializeField]
    private Text text_itemName;
    [SerializeField]
    private Text text_ItemDesc;
    [SerializeField]
    private Text text_ItemHowtoUsed;

    public void ShowToolTip(Item _item, Vector3 _position)
    {
        go_Base.SetActive(true);
        _position += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.5f,
                                -go_Base.GetComponent<RectTransform>().rect.height,
                                0);

        go_Base.transform.position = _position;

        text_itemName.text = _item.itemName;
        text_ItemDesc.text = _item.itemDesc;

        if(_item.itemType == Item.ItemType.EQUIPMENT)
            text_ItemHowtoUsed.text = "우클릭 - 장착";
        else if (_item.itemType == Item.ItemType.USED)
            text_ItemHowtoUsed.text = "우클릭 - 먹기";
        else
            text_ItemHowtoUsed.text = "";
    }

    public void HideToolTip()
    {
        go_Base.SetActive(false);
    }
}
