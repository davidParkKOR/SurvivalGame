using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] private string animalName; //동물 이름
    [SerializeField] private int hp; // 동물 체력
    [SerializeField] private float walkSpeed;//걷기 스피드

    private bool isWalking;// 걷는지 판별
    private bool isAction; //행동 중인지 판별

    [SerializeField] private float walkTime; //걷기 시간
    [SerializeField] private float waitTime; // 대기시간

    private float currentTime;
    private Vector3 direction;

    //필요 컴포넌트
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private BoxCollider boxCol;

    private void Start()
    {
        currentTime = waitTime;
        isAction = true;
    }

    private void Update()
    {
        Move();
        Rotation();
        ElapseTime();       
    }

    /// <summary>
    /// 움직임
    /// </summary>
    private void Move()
    {
        if (isWalking)
            rigid.MovePosition(transform.position + (transform.forward * walkSpeed * Time.deltaTime));
    }

    private void Rotation()
    {
        if(isWalking)
        {
            //움직일때만 회전하게 함
            Vector3 rotation = Vector3.Lerp(transform.eulerAngles, direction, 0.01f);
            rigid.MoveRotation(Quaternion.Euler(rotation));
        }
    }
    private void ElapseTime()
    {
        if(isAction)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
                ReSet();
        }
    }

    private void ReSet()
    {
        isWalking = false;
        isAction = true;
        anim.SetBool("Walking", isWalking);
        direction.Set(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        RandomAciton();
    }

    /// <summary>
    /// 다음 랜덤 행동 개시
    /// </summary>
    private void RandomAciton()
    {
        isAction = true;

        int randomNum = UnityEngine.Random.Range(0, 4);

        if      (randomNum == 0) Wait();
        else if (randomNum == 1) Eat();
        else if (randomNum == 2) Peek();
        else if (randomNum == 3) TryWalk();
    }

    private void Wait()
    {
        currentTime = waitTime;
        Debug.Log("대기");
    }

    private void Eat()
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
        Debug.Log("풀뜯기");
    }

    private void Peek()
    {
        currentTime = waitTime;
        anim.SetTrigger("Peek");
        Debug.Log("두리번");
    }

    private void TryWalk()
    {
        isWalking = true;
        currentTime = walkTime;
        anim.SetBool("Walking", isWalking);
        Debug.Log("걷기");
    }
}
