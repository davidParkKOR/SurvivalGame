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
    [SerializeField]
    private GameObject go_QuickSlotParent;

    private Slot[] slots; // �κ��丮 ���Ե�
    private Slot[] quickSlots;//�����Ե�
    private bool isNotPut;

    void Start()
    {
        slots = go_SlotParent.GetComponentsInChildren<Slot>();
        quickSlots = go_QuickSlotParent.GetComponentsInChildren<Slot>();
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
        //������ ���� �ֱ�
        PutSlot(quickSlots, _item, _count);

        //�κ��丮 �ֱ�
        if(isNotPut)
            PutSlot(slots, _item, _count);

        if (isNotPut)
            Debug.Log("�����԰� �κ��丮�� ��á���ϴ�.");
    }

    void PutSlot(Slot[] _slots, Item _item, int _count)
    {
        //����ϰ�쿡�� �׳� �κ��丮 �߰�
        if (Item.ItemType.EQUIPMENT != _item.itemType)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null)
                {
                    //���Կ� �������� �̹� �ִٸ� ���� ����
                    if (_slots[i].item.itemName == _item.itemName)
                    {
                        _slots[i].SetSlotCount(_count);
                        isNotPut = false;
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            //�������� �߰��Ȱ� ���ٸ�
            //�󽽷Կ� �߰�
            if (_slots[i].item == null)
            {
                _slots[i].AddItem(_item, _count);
                isNotPut = false;
                return;
            }
        }

        isNotPut = true;
    }
}
