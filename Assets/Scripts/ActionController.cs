using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; //���� ������ �ִ� �Ÿ�
    private bool pickupActivated = false; //���� ������ �� true;
    private RaycastHit hitInfo;// �浹ü ����

    //���� ���� �ִµ� �������� ȹƯ�ϸ� �ȵǴ� �̰� ���
    // ������ ���̾�� �����ϵ��� ����
    [SerializeField]
    private LayerMask layerMask; 

    //�ʿ� ��������
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
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "ȹ���߽��ϴ�");
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
        actionText.text = hitInfoItem  + "ȹ��" + "<color=yellow>" + "(E)" + "</color>";
    }

    void InfoDisAppear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false); 
    }
}
