using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; //습득 가능한 최대 거리
    private bool pickupActivated = false; //아이템 습득 가능할 시 true;
    private bool fireLookActivated = false; //고기 해체 가능할시  true;
    private RaycastHit hitInfo;// 충돌체 정보
    private bool dissolveActivated = false; //고기 해체 가능할 시 (죽은 돼지 바라볼때)
    private bool isDissolving = false; //고기 해체 중에는 true (중복 고기 해체 방지)

    bool lookComputer = false; //컴퓨터 바라볼 시 true

    //땅을 보고 있는데 아이템을 획특하면 안되니 이것 사용
    // 아이템 레이어에만 반응하도록 설정
    [SerializeField]
    private LayerMask layerMask; 

    //필요 컴포넡느
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;
    [SerializeField] private WeaponManager theWeaponManager;
    [SerializeField] private Transform tf_MeatDissolveTool;// 고기 해체 툴
    [SerializeField] private string sound_Meat;// 고기 소리 재생
    [SerializeField] QuickSlotController theQuickSlot;
    [SerializeField] ComputerKit theComputer;


    private void Update()
    {
        CheckAction();
        TryAction();
    }

    //E키 누루는 부분
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
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + "획득했습니다");
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
                // 손에 들고 있는 아이템을 불에 넣음 == 선택된 퀵 슬롯의 아이템
                // null 인지 판별 필요

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
                if(selectedSlot.item.itemName.Contains("고기"))
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
        actionText.text = hitInfoItem  + "획득" + "<color=yellow>" + "(E)" + "</color>";
    }


    void ComputerInfoAppear()
    {
        if(!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
        {
            Reset();
            lookComputer = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "컴퓨터 가동" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    void MeatInfoAppear()
    {
        Reset();
        //죽은 돼지 바라볼때만 뜨도록 함
        if (hitInfo.transform.GetComponent<Animal>().isDead)
        {
            dissolveActivated = true;
            actionText.gameObject.SetActive(true);

            string hitInfoItem = hitInfo.transform.GetComponent<Animal>().animalName;
            actionText.text = hitInfoItem + "해체하기" + "<color=yellow>" + "(E)" + "</color>";
        }

    }

    void FireInfoAppear()
    {
        Reset();
        fireLookActivated = true;


        if (hitInfo.transform.GetComponent<Fire>().GetIsFire())
        {
            actionText.gameObject.SetActive(true);
            actionText.text = "선택된 아이템 불에 넣기" + "<color=yellow>" + "(E)" + "</color>";

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
            //해체 작업 시작
            if ((hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal")&& hitInfo.transform.GetComponent<Animal>().isDead && !isDissolving)
            {
                isDissolving = true;
                InfoDisAppear();

                //고기 해체 작업 실시
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
