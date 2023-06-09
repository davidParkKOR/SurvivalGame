using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //인벤토리 띄워져있을때 마우스 클릭, 무기 사용 제한
    public static bool inventoryActivated = false;

    //필요 컴포넌트
    [SerializeField]
    private GameObject go_inventoryBase;
    [SerializeField]
    private GameObject go_SlotParent;
    [SerializeField]
    private GameObject go_QuickSlotParent;
    [SerializeField]
    private QuickSlotController theQuickSlot;

    private Slot[] slots; // 인벤토리 슬롯들
    private Slot[] quickSlots;//퀵슬롯들
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

    //슬롯에 아이템 채우기
    public void AcquireItem(Item _item, int _count = 1)
    {
        //퀵슬롯 먼저 넣기
        PutSlot(quickSlots, _item, _count);

        if (!isNotPut)
            theQuickSlot.IsActivatedQuickSlot(slotNumber);

        //인벤토리 넣기
        if(isNotPut)
            PutSlot(slots, _item, _count);

        if (isNotPut)
            Debug.Log("퀵슬롯과 인벤토리가 꽉찼습니다.");
    }

    void PutSlot(Slot[] _slots, Item _item, int _count)
    {
        //장비, Kit일경우에는 그냥 인벤토리 추가
        if (Item.ItemType.EQUIPMENT != _item.itemType && Item.ItemType.KIT != _item.itemType)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null)
                {
                    //슬롯에 아이템이 이미 있다면 갯수 증가
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
            //아이템이 추가된게 없다면
            //빈슬롯에 추가
            if (_slots[i].item == null)
            {
                _slots[i].AddItem(_item, _count);
                isNotPut = false;
                return;
            }
        }

        isNotPut = true;
    }

    //아이템을 찾으면 아이템이 몇개인지 확인
    public int GetItemCount(string _itemName)
    {
        //퀵슬롯, 아이템슬롯
        int temp = SearchSlotItem(slots, _itemName);

        return temp != 0 ? temp : SearchSlotItem(quickSlots, _itemName);
    }

    private int SearchSlotItem(Slot[] _slots, string _itemName)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item != null)
            {
                //대소문자 구분때문에 오류생길수 잇어서 
                //전체 다 소문자로 변환시켜버림
                if (_itemName.ToLower() == _slots[i].item.itemName.ToLower())
                {
                    return _slots[i].itemCount;
                }
                  
            }

        }

        return 0;
    }

    //인벤토리에서 쓴 아이템 카운트 변경
    public void SetItemCount(string _itemName, int _itemCount)
    {
        if(!ItemCountAdjust(slots, _itemName, _itemCount))
        {
            //false면 한번 더돌림 퀵슬롯으로
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
