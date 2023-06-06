using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    //È°¼ºÈ­ ¿©ºÎ
    public static bool isActivate = true;

    [SerializeField] private QuickSlotController theQuickSlotController;

    // Update is called once per frame
    void Update()
    {
        if (isActivate && !Inventory.inventoryActivated)
        {
            if (QuickSlotController.go_HandItem == null)
                TryAttack();
            else
                TryEating();
        }
    }

    void TryEating()
    {
        Debug.Log("¸Ô¾î¹ö¸²");
        //ÁÂÅ¬¸¯½Ã ¸ÔÀ½

        if(Input.GetButtonDown("Fire1") && !theQuickSlotController.GetIsCoolTime())
        {
            Debug.Log("¸Ô¾î¹ö¸²");
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
