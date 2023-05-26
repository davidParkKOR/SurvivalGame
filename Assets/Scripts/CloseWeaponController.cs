using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour
{ 

    //���� ������ �ڵ��� Ÿ�� ����
    [SerializeField]
    protected CloseWeapon currentCloseWeapon;

    // ������ 
    protected bool isAttack = false;
    protected bool isSwing = false;
    protected RaycastHit hitInfo;

    //�ʿ� ������Ʈ
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
                        //�ڷ�ƾ ����
                        StartCoroutine(AttackCoroutine("Chop",
                                                       currentCloseWeapon.workDelayA,
                                                       currentCloseWeapon.workDelayB,
                                                       currentCloseWeapon.workDelay));

                        return;
                    }
                }


                //�ڷ�ƾ ����
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

        //���� Ȱ��ȭ ����
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

    //�ϼ��Լ�������, �߰� ������ ������ �Լ�
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
