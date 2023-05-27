using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item item; //ȹ���� ������
    public int itemCount; //ȹ���� �������� ����
    public Image itemIamge; //������ �̹���

    //�ʿ� ������Ʈ
    [SerializeField]
    private Text text_count;
    [SerializeField]
    private GameObject go_CountImage;


    //���İ� ����
    private void SetColor(float _alpha)
    {
        Color color = itemIamge.color;
        color.a = _alpha;
        itemIamge.color = color;
    }

    //������ ȹ��
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemIamge.sprite = item.itemImage;

        if (item.itemType != Item.ItemType.EQUIPMENT)
        {
            go_CountImage.SetActive(true);
            text_count.text = itemCount.ToString();
        }
        else
        {
            text_count.text = "0";
            go_CountImage.SetActive(false);           
        }

        SetColor(1);
    }

    //������ ���� ����
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    //���� �ʱ�ȭ
    void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemIamge.sprite = null;
        SetColor(0);
        text_count.text = "0";
        go_CountImage.SetActive(false);

    }
}
