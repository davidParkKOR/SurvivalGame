using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    //활성화 여부
    public static bool isActivate = true;
    public static Item currentKit; //설치하려는 킷 

    bool isPreview = false;
    GameObject go_preview;//설치할 키트 프리뷰
    Vector3 previewPos;//설치할 키트 위치
    [SerializeField] float rangeAdd;//건축시 추가 사정거리
    [SerializeField]  QuickSlotController theQuickSlotController;

    // Update is called once per frame
    void Update()
    {
        if (isActivate && !Inventory.inventoryActivated)
        {
            if( currentKit == null)
            {
                if (QuickSlotController.go_HandItem == null)
                    TryAttack();
                else
                    TryEating();
            }
            else
            {
                if(!isPreview)
                {
                    //한번만 생성하도록 함
                    InstallPreviewKit();
                }

                PreviewPositionUpdate();
                Build();
            }

        }
    }

    void InstallPreviewKit()
    {
        isPreview = true;
        go_preview = Instantiate(currentKit.kitPreviewPrefab, transform.position, Quaternion.identity);
    }

    void PreviewPositionUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range + rangeAdd, layerMask))
        {
            previewPos = hitInfo.point;
            go_preview.transform.position = previewPos;
        }
    }

    void Build()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            if(go_preview.GetComponent<PreviewObject>().isbuildable())
            {
                theQuickSlotController.DecreaseSelectedItem(); //슬롯 아이템 갯수 -1
                GameObject temp = Instantiate(currentKit.kitPrefab, previewPos, Quaternion.identity);
                temp.name = currentKit.itemName;
                Destroy(go_preview);
                currentKit = null;
                isPreview = false;
            }
        }
    }

    public void Cancel()
    {
        Destroy(go_preview);
        currentKit = null;
        isPreview = false;
    }

    void TryEating()
    {
        Debug.Log("먹어버림");
        //좌클릭시 먹음

        if(Input.GetButtonDown("Fire1") && !theQuickSlotController.GetIsCoolTime())
        {
            Debug.Log("먹어버림");
            currentCloseWeapon.anim.SetTrigger("Eat");
            theQuickSlotController.DecreaseSelectedItem();
        }
    }

    protected override IEnumerator HItCoroutine()
    {
        while (isSwing)
        {
            if (CheckObject())
            {
                isSwing = !isSwing;
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}
