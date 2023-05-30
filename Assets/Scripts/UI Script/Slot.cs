using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.UIElements;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item; //ȹ���� ������
    public int itemCount; //ȹ���� �������� ����
    public Image itemIamge; //������ �̹���

    //�ʿ� ������Ʈ
    [SerializeField]
    private Text text_count;
    [SerializeField]
    private GameObject go_CountImage;

    [SerializeField] private bool isQuickSlot; //�W���� ���� �Ǵ�
    [SerializeField] private int quickSlotNumber; //������ ��ȣ

    private ItemEffectDatabase theItemEffectDatabase;

    [SerializeField] private RectTransform baseRect; // �κ��丮 ����
    [SerializeField] private RectTransform quickSlotBaseRect; //������ ����
    private InputNumber theInputNumber;

    private void Start()
    {
        theInputNumber = FindObjectOfType<InputNumber>();
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
    }


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

    public int GetQuickSlotNumber()
    {
        return quickSlotNumber;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        //�� ��ũ��Ʈ�� ������ ��ư�� Ŭ���ϸ� �̺�Ʈ ����
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                //������ �Ҹ�
                theItemEffectDatabase.UseItem(item);

                if(item.itemType == Item.ItemType.USED)
                    SetSlotCount(-1);             
                
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        if(item !=null && Inventory.inventoryActivated)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemIamge);
            //�巡�װ� ó�� ���������� 
            DragSlot.instance.transform.position = eventData.position;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        //�巡�� ���϶� ������ ���콺 Ŀ���� ����ٴ�
        if (item != null)
        {
            //�巡�װ� ó�� ���������� 
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    //�巡�� ������ ����
    public void OnEndDrag(PointerEventData eventData)
    {

        if(!((DragSlot.instance.transform.localPosition.x > baseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < baseRect.rect.xMax &&
           DragSlot.instance.transform.localPosition.y > baseRect.rect.yMin && DragSlot.instance.transform.localPosition.y < baseRect.rect.yMax)
           ||
           (DragSlot.instance.transform.localPosition.x > quickSlotBaseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < quickSlotBaseRect.rect.xMax &&
           DragSlot.instance.transform.localPosition.y > quickSlotBaseRect.transform.localPosition.y - quickSlotBaseRect.rect.yMax && DragSlot.instance.transform.localPosition.y < quickSlotBaseRect.transform.localPosition.y - quickSlotBaseRect.rect.yMin)))
        {        

            if (DragSlot.instance.dragSlot != null)
            {
                theInputNumber.Call();
            }      
        }
        else
        {
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }


    }

    //���� �ٲܶ�
    public void OnDrop(PointerEventData eventData)
    {
      
        //��������
        if (DragSlot.instance.dragSlot != null)
        {          
            ChangeSlot();

            //�κ��丮���� ������ , �W���Կ��� ������
            if(isQuickSlot)
            {
                theItemEffectDatabase.IsActivatedQuickSlot(quickSlotNumber);
            }
            else
            {
                //�κ��丮-> �κ��丮, ������ -> �κ��丮
                if (DragSlot.instance.dragSlot.isQuickSlot) // �W���� -> �κ��丮
                {
                    //Ȱ��ȭ�� ������ �Ǵ�
                    theItemEffectDatabase.IsActivatedQuickSlot(DragSlot.instance.dragSlot.quickSlotNumber);
                }

            }
        }

    }

    void ChangeSlot()
    {
        Item tempItem = item;
        int tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if(tempItem != null)
            DragSlot.instance.dragSlot.AddItem(tempItem, tempItemCount);     
        else       
            DragSlot.instance.dragSlot.ClearSlot();
     

    }


    //���콺�� ���Կ� ���� ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
        theItemEffectDatabase.ShowToolTip(item, transform.position);
    }

    //���콺�� ���Կ� ���������� ����
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip();
    }
}
