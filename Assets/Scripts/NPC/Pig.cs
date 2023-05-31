using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] private string animalName; //���� �̸�
    [SerializeField] private int hp; // ���� ü��
    [SerializeField] private float walkSpeed;//�ȱ� ���ǵ�

    private bool isWalking;// �ȴ��� �Ǻ�
    private bool isAction; //�ൿ ������ �Ǻ�

    [SerializeField] private float walkTime; //�ȱ� �ð�
    [SerializeField] private float waitTime; // ���ð�

    private float currentTime;
    private Vector3 direction;

    //�ʿ� ������Ʈ
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
    /// ������
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
            //�����϶��� ȸ���ϰ� ��
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
    /// ���� ���� �ൿ ����
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
        Debug.Log("���");
    }

    private void Eat()
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
        Debug.Log("Ǯ���");
    }

    private void Peek()
    {
        currentTime = waitTime;
        anim.SetTrigger("Peek");
        Debug.Log("�θ���");
    }

    private void TryWalk()
    {
        isWalking = true;
        currentTime = walkTime;
        anim.SetBool("Walking", isWalking);
        Debug.Log("�ȱ�");
    }
}
