using System.Collections;
using System.Collections.Generic;
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

    //슬롯에 아이템 채우기
    public void AcquireItem(Item _item, int _count = 1)
    {
        //장비일경우에는 그냥 인벤토리 추가
        if(Item.ItemType.EQUIPMENT != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    //슬롯에 아이템이 이미 있다면 갯수 증가
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
            //아이템이 추가된게 없다면
            //빈슬롯에 추가
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }

    }
}
