using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; //���� ������ �ִ� �Ÿ�
    private bool pickupActivated = false; //������ ���� ������ �� true;
    private bool fireLookActivated = false; //��� ��ü �����ҽ�  true;
    private RaycastHit hitInfo;// �浹ü ����
    private bool dissolveActivated = false; //��� ��ü ������ �� (���� ���� �ٶ󺼶�)
    private bool isDissolving = false; //��� ��ü �߿��� true (�ߺ� ��� ��ü ����)

    bool lookComputer = false; //��ǻ�� �ٶ� �� true

    //���� ���� �ִµ� �������� ȹƯ�ϸ� �ȵǴ� �̰� ���
    // ������ ���̾�� �����ϵ��� ����
    [SerializeField]
    private LayerMask layerMask; 

    //�ʿ� ��������
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;
    [SerializeField] private WeaponManager theWeaponManager;
    [SerializeField] private Transform tf_MeatDissolveTool;// ��� ��ü ��
    [SerializeField] private string sound_Meat;// ��� �Ҹ� ���
    [SerializeField] QuickSlotController theQuickSlot;
    [SerializeField] ComputerKit theComputer;


    private void Update()
    {
        CheckAction();
        TryAction();
    }

    //EŰ ����� �κ�
    void TryAction()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            CheckAction();
            CanPickUp();
            CanMeat();
            CanDropFire();
            CanComputerPowerOn();
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

    private void CanDropFire()
    {
        if (fireLookActivated)
        {
            if(hitInfo.transform.tag == "Fire" && hitInfo.transform.GetComponent<Fire>().GetIsFire())
            {
                // �տ� ��� �ִ� �������� �ҿ� ���� == ���õ� �� ������ ������
                // null ���� �Ǻ� �ʿ�

                Slot selectedSlot = theQuickSlot.GetSelectedSlot();

                if (selectedSlot.item != null)
                {
                    DropAnItem(selectedSlot);                
                }
            }        
        }
    }

    void CanComputerPowerOn()
    {
        if (lookComputer)
        {
            if (hitInfo.transform != null)
            {
                if(!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
                {
                    hitInfo.transform.GetComponent<ComputerKit>().PowerOn();
                    InfoDisAppear();
                }
            }
        }
    }




    private void DropAnItem(Slot selectedSlot)
    {
        switch(selectedSlot.item.itemType)
        {
            case Item.ItemType.USED:
                if(selectedSlot.item.itemName.Contains("���"))
                {
                    Instantiate(selectedSlot.item.itemPrefab, hitInfo.transform.position + Vector3.up, Quaternion.identity);
                    theQuickSlot.DecreaseSelectedItem();
                }
                break;
            case Item.ItemType.INGREDIENT:
                break;
        }
    }

    void CheckAction()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
            else if (hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal")
            {
                MeatInfoAppear();
            }
            else if(hitInfo.transform.tag == "Fire")
            {
                FireInfoAppear();
            }
            else if (hitInfo.transform.tag == "Computer")
            {
                ComputerInfoAppear();
            }
            else
                InfoDisAppear();
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


    void ComputerInfoAppear()
    {
        if(!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
        {
            Reset();
            lookComputer = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "��ǻ�� ����" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    void MeatInfoAppear()
    {
        Reset();
        //���� ���� �ٶ󺼶��� �ߵ��� ��
        if (hitInfo.transform.GetComponent<Animal>().isDead)
        {
            dissolveActivated = true;
            actionText.gameObject.SetActive(true);

            string hitInfoItem = hitInfo.transform.GetComponent<Animal>().animalName;
            actionText.text = hitInfoItem + "��ü�ϱ�" + "<color=yellow>" + "(E)" + "</color>";
        }

    }

    void FireInfoAppear()
    {
        Reset();
        fireLookActivated = true;


        if (hitInfo.transform.GetComponent<Fire>().GetIsFire())
        {
            actionText.gameObject.SetActive(true);
            actionText.text = "���õ� ������ �ҿ� �ֱ�" + "<color=yellow>" + "(E)" + "</color>";

        }
    }

    void InfoDisAppear()
    {
        Reset();
        pickupActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
        lookComputer = false;
        actionText.gameObject.SetActive(false); 
    }

    private void CanMeat()
    {
        if(dissolveActivated)
        {
            //��ü �۾� ����
            if ((hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal")&& hitInfo.transform.GetComponent<Animal>().isDead && !isDissolving)
            {
                isDissolving = true;
                InfoDisAppear();

                //��� ��ü �۾� �ǽ�
                StartCoroutine(MeatCoroutine());
            }
               

        }
    }

    IEnumerator MeatCoroutine()
    {
        WeaponManager.isChangeWeapon = true;
        WeaponSway.isActivated = false;
        WeaponManager.currentWeaponAnim.SetTrigger("Weapon_Out");
        PlayerController.isActivated = false;

        yield return new WaitForSeconds(0.2f);

        WeaponManager.currentWeapon.gameObject.SetActive(false);
        tf_MeatDissolveTool.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        SoundManager.instance.PlaySE(sound_Meat);

        yield return new WaitForSeconds(1.8f);

        Animal animal = hitInfo.transform.GetComponent<Animal>();
        Debug.Log(animal.GetItem().itemName) ;
        Debug.Log(animal.itemNumber);


        theInventory.AcquireItem(hitInfo.transform.GetComponent<Animal>().GetItem(), hitInfo.transform.GetComponent<Animal>().itemNumber);

        WeaponManager.currentWeapon.gameObject.SetActive(true);
        tf_MeatDissolveTool.gameObject.SetActive(false);

        PlayerController.isActivated = true;
        WeaponSway.isActivated = true;
        WeaponManager.isChangeWeapon = false;
        isDissolving = false;

    }

    private void Reset()
    {
        pickupActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
    }
}
