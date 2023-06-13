using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongAnimal : Animal
{

    [SerializeField] protected int attackDamage;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected LayerMask targetMask;
    [SerializeField] protected float chaseTime; //총 추격시간
    protected float currentCahseTime; //계산
    [SerializeField] protected float chaseDelayTime; //추격 딜레이

    /// <summary>
    /// 달리기
    /// </summary>
    /// <param name="_targetPos"></param>
    public void Chase(Vector3 _targetPos)
    {
        isChasing= true;
        destination = _targetPos;
        nav.SetDestination(destination);
        isRunning = true;
        anim.SetBool("Running", isRunning);
        nav.SetDestination(destination);
    }

    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);

        if (!isDead)
            Chase(_targetPos);
    }


    //시야에서 사라져도 일정시간동안 추격
    protected IEnumerator ChaseTargetCoroutine()
    {
        currentCahseTime = 0;

        while (currentCahseTime < chaseTime)
        {
            Chase(theViewAngle.GetTargetPos());

            //거리가 가까우면 공격
            //충분히 가까이 있을때
            //Debug.Log("TARGET POS : " + theViewAngle.GetTargetPos());
            //Debug.Log("ANIMAL POS : " + transform.position);
            if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= 3f)
            {
                if (theViewAngle.View()) //눈앞에 있을때
                {

                    StartCoroutine(AttackCoroutine());

                }
            }
            yield return new WaitForSeconds(chaseDelayTime);
      
            currentCahseTime += chaseDelayTime;
        }

        isChasing = false;
        isRunning = false;
        anim.SetBool("Running", isRunning);
        nav.ResetPath();
    }

    protected IEnumerator AttackCoroutine()
    {
        isRunning = false;
        isAttacking = true;
        nav.ResetPath();
        currentCahseTime = chaseTime;
        yield return new WaitForSeconds(0.5f);
        //플레이어를 쳐다보게 만듬
        transform.LookAt(theViewAngle.GetTargetPos());
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.5f);

        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 3, targetMask))
        {
            thePlayerStatus.DecreaseHP(attackDamage);

        }
        else
        {
            Debug.Log("플레이어 빗나감");
        }
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
        StartCoroutine(ChaseTargetCoroutine());
    }
}
