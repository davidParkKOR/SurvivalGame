using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : WeakAnimal
{
    protected override void Update()
    {
        base.Update();

        if(theViewAngle.View() && !isDead)
        {
            Run(theViewAngle.GetTargetPos());
        }
    }


    private void Wait()
    {
        currentTime = waitTime;
    }

    private void Eat()
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
    }

    private void Peek()
    {
        currentTime = waitTime;
        anim.SetTrigger("Peek");
    }

    protected override void ReSet()
    {
        base.ReSet();
        RandomAciton();
    }

    /// <summary>
    /// 다음 랜덤 행동 개시
    /// </summary>
    private void RandomAciton()
    {
        //isAction = true;
        RandomSound();

        int randomNum = UnityEngine.Random.Range(0, 4);

        if (randomNum == 0) Wait();
        else if (randomNum == 1) Eat();
        else if (randomNum == 2) Peek();
        else if (randomNum == 3) TryWalk();
    }

}
