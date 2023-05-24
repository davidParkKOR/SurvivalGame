using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController
{
    //활성화 여부
    public static bool isActivate = false;

    void Start()
    {
        //WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        //WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivate)
        {
            TryAttack();
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
