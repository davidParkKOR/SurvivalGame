using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corsshair : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    //크로스 헤어 상태에 따른 총의 정확도
    private float gunAccuracy;

    //크로스헤어 비활성화를 위한 부모 객체
    [SerializeField]
    private GameObject go_CrosshairHUD;

    [SerializeField]
    private GunController theGunController;

    public void WalkingAnimation(bool _flag)
    {
        if(!GameManager.isWater)
        {
            WeaponManager.currentWeaponAnim.SetBool("Walk", _flag);
            animator.SetBool("Walking", _flag);
        }
    }

    public void RunningAnimation(bool _flag)
    {
        if (!GameManager.isWater)
        {
            WeaponManager.currentWeaponAnim.SetBool("Run", _flag);
            animator.SetBool("Running", _flag);
        }

    }

    public void JumpingAnimation(bool _flag)
    {
        if (!GameManager.isWater)
            animator.SetBool("Running", _flag);
    }

    public void CrouchingAnimation(bool _flag)
    {
        if (!GameManager.isWater)
            animator.SetBool("Crouching", _flag);
    }

    public void FineSightAnimation(bool _flag)
    {
        if (!GameManager.isWater)
            animator.SetBool("FineSight", _flag);
    }

    public void FindAnimation()
    {
        if (!GameManager.isWater)
        {
            if (animator.GetBool("Walking"))
            {
                //WalkingFIre 실행
                animator.SetTrigger("Walk_Fire");
            }
            else if (animator.GetBool("Crouching"))
            {
                //Crouching Fire 실행
                animator.SetTrigger("Crouch_Fire");
            }
            else
            {
                //가만히 있는 상태에서 쏘는것
                animator.SetTrigger("Idle_Fire");
            }
        }

    }

    public float GetAccuracy()
    {
        if (animator.GetBool("Walking"))
        {
            gunAccuracy = 0.06f;
        }
        else if (animator.GetBool("Crouching"))
        {
            gunAccuracy = 0.015f;
        }
        else if(theGunController.GetFineSightMode())
        {
            gunAccuracy = 0.035f;
        }
        else
        {
            //가만히 있을때 정확도
            gunAccuracy = 0.04f;
        }

        return gunAccuracy;
    }


}
