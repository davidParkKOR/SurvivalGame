using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //활성화 여부
    public static bool isActivate = true;

    //현재 장착된 핸드형 타입 무기
    [SerializeField]
    private Hand currentHand;

    // 공격중 
    private bool isAttack = false;
    private bool isSwing = false;
    private RaycastHit hitInfo;

    // Update is called once per frame
    void Update()
    {
        if(isActivate)
        {
            TryAttack();
        }
     
    }

    void TryAttack()
    {
        if(Input.GetButton("Fire1"))
        {
            if(!isAttack)
            {
                //코루틴 실행
                StartCoroutine(AttackCoroutine());  
            }
        }

    }

    IEnumerator AttackCoroutine()
    {
        isAttack = true;

        currentHand.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        //공격 활성화 시점
        StartCoroutine(HItCoroutine());

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }
    
    IEnumerator HItCoroutine()
    {
        while (isSwing)
        {
            if(CheckObject())
            {
                isSwing = !isSwing;
            }

            yield return null;

        }
    }

    bool CheckObject()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
        {
            return true;
        }

        return false;
    }

    public void HandChange(Hand _hand)
    {
        if (WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentHand = _hand;
        WeaponManager.currentWeapon = currentHand.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentHand.anim;
        currentHand.transform.localPosition = Vector3.zero;
        currentHand.gameObject.SetActive(true);
        isActivate = true;

    }
}
