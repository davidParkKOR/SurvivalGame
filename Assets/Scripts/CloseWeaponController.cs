using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour
{ 

    //현재 장착된 핸드형 타입 무기
    [SerializeField]
    protected CloseWeapon currentCloseWeapon;

    // 공격중 
    protected bool isAttack = false;
    protected bool isSwing = false;
    protected RaycastHit hitInfo;

    //필요 컴포넌트
    protected PlayerController thePlayerController;


    void Start()
    {
        Debug.Log("### Start ###");
        thePlayerController = FindObjectOfType<PlayerController>();
        Debug.Log(thePlayerController);
    }

    protected void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!isAttack)
            {
                if(CheckObject())
                {
                    if(currentCloseWeapon.isAxe && hitInfo.transform.tag == "Tree")
                    {
                        Debug.Log("### TryAttack_ThePlayerControoler");
                        Debug.Log(thePlayerController);
                        Debug.Log("### GetTreeCenterPosition");
                        Debug.Log(hitInfo.transform.GetComponent<TreeComponent>().GetTreeCenterPosition());
                        StartCoroutine(thePlayerController.TreeLookCoroutine(hitInfo.transform.GetComponent<TreeComponent>().GetTreeCenterPosition()));
                        //코루틴 실행
                        StartCoroutine(AttackCoroutine("Chop",
                                                       currentCloseWeapon.workDelayA,
                                                       currentCloseWeapon.workDelayB,
                                                       currentCloseWeapon.workDelay));

                        return;
                    }
                }


                //코루틴 실행
                StartCoroutine(AttackCoroutine("Attack", 
                                               currentCloseWeapon.attackDelayA , 
                                               currentCloseWeapon.attackDelayB, 
                                               currentCloseWeapon.attackDelay));
            }
        }

    }

    protected IEnumerator AttackCoroutine(string _swingType, float _delayA, float _delayB, float _delayC)
    {
        isAttack = true;

        currentCloseWeapon.anim.SetTrigger(_swingType);

        yield return new WaitForSeconds(_delayA);
        isSwing = true;

        //공격 활성화 시점
        StartCoroutine(HItCoroutine());

        yield return new WaitForSeconds(_delayB);
        isSwing = false;

        yield return new WaitForSeconds(_delayC - _delayA - _delayB);
        isAttack = false;
    }

    protected abstract IEnumerator HItCoroutine();


    protected bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range))
        {
            return true;
        }

        return false;
    }

    //완성함수이지만, 추가 편집이 가능한 함수
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        if (WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentCloseWeapon = _closeWeapon;
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);

    }
}
