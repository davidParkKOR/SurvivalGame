using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakAnimal : Animal
{
    /// <summary>
    /// �޸���
    /// </summary>
    /// <param name="_targetPos"></param>
    public void Run(Vector3 _targetPos)
    {
        //�¾������ �÷��̾� �ݴ� �������� �ٰ� ��
        destination = new Vector3(transform.position.x - _targetPos.x, 0f, transform.position.z - _targetPos.x).normalized;
        nav.speed = runTime;
        isWalking = false;
        isRunning = true;
        applySpped = runSpeed;
        anim.SetBool("Running", true);
    }

    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);

        if (!isDead)  Run(_targetPos);
    }
}
