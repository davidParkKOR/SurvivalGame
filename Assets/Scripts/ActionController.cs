using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; //습득 가능한 최대 거리
    private bool pickupActivated = false; //습득 가능할 시 true;
    private RaycastHit hitInfo;// 충돌체 정보

    //땅을 보고 있는데 아이템을 획특하면 안되니 이것 사용
    // 아이템 레이어에만 반응하도록 설정
    [SerializeField]
    private LayerMask layerMask; 

    //필요 컴포넡느
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;


    private void Update()
    {
        CheckItem();
        TryAction();
    }

    void TryAction()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            CheckItem();
            CanPickUp();
        }
    }

    void CanPickUp()
    {
        if(pickupActivated) 
        { 
            if(hitInfo.transform != null)
            {
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "획득했습니다");
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisAppear();
            }
        }
    }

    void CheckItem()
    {
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {         
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }               
        }
        else
        {
            InfoDisAppear();
        }
    }

    void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);

        string hitInfoItem = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName;
        actionText.text = hitInfoItem  + "획득" + "<color=yellow>" + "(E)" + "</color>";
    }

    void InfoDisAppear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false); 
    }
}
