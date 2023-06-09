using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
    [SerializeField]
    private QuickSlotController theQuickSlot;

    private Slot[] slots; // �κ��丮 ���Ե�
    private Slot[] quickSlots;//�����Ե�
    private bool isNotPut;
    private int slotNumber;

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
        GameManager.isOpenInventory = true;
        go_inventoryBase.SetActive(true);
    }

    void CloseInventory()
    {
        GameManager.isOpenInventory = false;
        go_inventoryBase.SetActive(false);
    }

    //���Կ� ������ ä���
    public void AcquireItem(Item _item, int _count = 1)
    {
        //������ ���� �ֱ�
        PutSlot(quickSlots, _item, _count);

        if (!isNotPut)
            theQuickSlot.IsActivatedQuickSlot(slotNumber);

        //�κ��丮 �ֱ�
        if(isNotPut)
            PutSlot(slots, _item, _count);

        if (isNotPut)
            Debug.Log("�����԰� �κ��丮�� ��á���ϴ�.");
    }

    void PutSlot(Slot[] _slots, Item _item, int _count)
    {
        //���, Kit�ϰ�쿡�� �׳� �κ��丮 �߰�
        if (Item.ItemType.EQUIPMENT != _item.itemType && Item.ItemType.KIT != _item.itemType)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null)
                {
                    //���Կ� �������� �̹� �ִٸ� ���� ����
                    if (_slots[i].item.itemName == _item.itemName)
                    {
                        slotNumber = i;
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

    //�������� ã���� �������� ����� Ȯ��
    public int GetItemCount(string _itemName)
    {
        //������, �����۽���
        int temp = SearchSlotItem(slots, _itemName);

        return temp != 0 ? temp : SearchSlotItem(quickSlots, _itemName);
    }

    private int SearchSlotItem(Slot[] _slots, string _itemName)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item != null)
            {
                //��ҹ��� ���ж����� ��������� �վ 
                //��ü �� �ҹ��ڷ� ��ȯ���ѹ���
                if (_itemName.ToLower() == _slots[i].item.itemName.ToLower())
                {
                    return _slots[i].itemCount;
                }
                  
            }

        }

        return 0;
    }

    //�κ��丮���� �� ������ ī��Ʈ ����
    public void SetItemCount(string _itemName, int _itemCount)
    {
        if(!ItemCountAdjust(slots, _itemName, _itemCount))
        {
            //false�� �ѹ� ������ ����������
            ItemCountAdjust(quickSlots, _itemName, _itemCount);
        }
    }

    private bool ItemCountAdjust(Slot[] _slots, string _itemName, int _itemCount)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item != null)
            {
                if (_itemName == _slots[i].item.itemName)
                {
                    _slots[i].SetSlotCount(-_itemCount);
                    return true;
                }
            }
        }
         return false;
    }
}
