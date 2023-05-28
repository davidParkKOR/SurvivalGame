using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.UIElements;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item; //획득한 아이템
    public int itemCount; //획득한 아이템의 갯수
    public Image itemIamge; //아이템 이미지

    //필요 컴포넌트
    [SerializeField]
    private Text text_count;
    [SerializeField]
    private GameObject go_CountImage;

    private ItemEffectDatabase theItemEffectDatabase;
    private Rect baseRect;
    private InputNumber theInputNumber;

    private void Start()
    {
        baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;       
        theInputNumber = FindObjectOfType<InputNumber>();
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
    }



    //알파값 변경
    private void SetColor(float _alpha)
    {
        Color color = itemIamge.color;
        color.a = _alpha;
        itemIamge.color = color;
    }

    //아이템 획득
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

    //아이템 갯수 조절
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    //슬롯 초기화
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
        //이 스크립트에 오른쪽 버튼을 클릭하면 이벤트 실행
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                //아이템 소모
                theItemEffectDatabase.UseItem(item);

                if(item.itemType == Item.ItemType.USED)
                    SetSlotCount(-1);             
                
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item !=null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemIamge);
            //드래그가 처음 시작했을때 
            DragSlot.instance.transform.position = eventData.position;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        //드래그 중일때 슬롯이 마우스 커서를 따라다님
        if (item != null)
        {
            //드래그가 처음 시작했을때 
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    //드래그 끝날떄 실행
    public void OnEndDrag(PointerEventData eventData)
    {
        if(DragSlot.instance.transform.localPosition.x < baseRect.xMin ||
           DragSlot.instance.transform.localPosition.x > baseRect.xMax ||
           DragSlot.instance.transform.localPosition.y < baseRect.yMin ||
           DragSlot.instance.transform.localPosition.y > baseRect.yMax)
        {
            //Instantiate(DragSlot.instance.dragSlot.item.itemPrefab,
            //           theWeaponManager.transform.position + theWeaponManager.transform.forward,
            //           Quaternion.identity);
            //DragSlot.instance.dragSlot.ClearSlot();

            if(DragSlot.instance.dragSlot != null)
                theInputNumber.Call();
        }
        else
        {
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }


    }

    //슬롯 바꿀때
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDROP 실행");
        //오류방지
        if (DragSlot.instance.dragSlot != null)
        {          
            ChangeSlot();
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


    //마우스가 슬롯에 들어갈떄 실행
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
        theItemEffectDatabase.ShowToolTip(item, transform.position);
    }

    //마우스가 슬롯에 빠져나갈떄 시행
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip();
    }
}
