using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    //Ȱ��ȭ ����
    public static bool isActivate = true;
    public static Item currentKit; //��ġ�Ϸ��� Ŷ 

    bool isPreview = false;
    GameObject go_preview;//��ġ�� ŰƮ ������
    Vector3 previewPos;//��ġ�� ŰƮ ��ġ
    [SerializeField] float rangeAdd;//����� �߰� �����Ÿ�
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
                    //�ѹ��� �����ϵ��� ��
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
                theQuickSlotController.DecreaseSelectedItem(); //���� ������ ���� -1
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
        Debug.Log("�Ծ����");
        //��Ŭ���� ����

        if(Input.GetButtonDown("Fire1") && !theQuickSlotController.GetIsCoolTime())
        {
            Debug.Log("�Ծ����");
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
