using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //�κ��丮 ����������� ���콺 Ŭ��, ���� ��� ����
    public static bool inventoryActivated = false;

    //�ʿ� ������Ʈ
    [SerializeField]
    private GameObject go_inventoryBase;
    [SerializeField]
    private GameObject go_SlotParent;

    private Slot[] slots;

    void Start()
    {
        slots = go_SlotParent.GetComponentsInChildren<Slot>();
    }

    void Update()
    {
        TryOpenInventory();
    }

    void TryOpenInventory()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
            {
                OpenInventory();
            }
            else
                CloseInventory();
        }
    }

    void OpenInventory()
    {
        go_inventoryBase.SetActive(true);
    }

    void CloseInventory()
    {
        go_inventoryBase.SetActive(false);
    }

    //���Կ� ������ ä���
    public void AcquireItem(Item _item, int _count = 1)
    {
        //����ϰ�쿡�� �׳� �κ��丮 �߰�
        if(Item.ItemType.EQUIPMENT != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    //���Կ� �������� �̹� �ִٸ� ���� ����
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            //�������� �߰��Ȱ� ���ٸ�
            //�󽽷Կ� �߰�
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }

    }
}
