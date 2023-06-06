using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{
    private bool isBurning = false;

    [SerializeField] private int damage;
    [SerializeField] private float damageTime;

    private float currentDamangeTime;

    [SerializeField] private float durationTime;
    private float currentDurationTime;

    [SerializeField] GameObject flame_prefab; //�� ������ ������ ����
    private GameObject go_tempFlame; //�׸�

    private void Update()
    {
        if(isBurning)
        {
            //�ð��� ����Կ� ���� ������ ����
            ElapseTime();
        }
    }

    public void StartBurning()
    {
        if(!isBurning)
        {
            go_tempFlame = Instantiate(flame_prefab, transform.position, Quaternion.EulerAngles(-90f,0f,0f));
            go_tempFlame.transform.SetParent(transform);
        }

        isBurning = true;
        currentDurationTime = durationTime;


    }

    void ElapseTime()
    {
        if(isBurning)
        {
            currentDurationTime -= Time.deltaTime;

            if (currentDurationTime > 0)
                currentDurationTime -= Time.deltaTime;

            if(currentDamangeTime <= 0)
            {
                //������ ����
                Damage();
            }

            if(currentDurationTime <= 0)
            {
                //���� ��
                Off();

            }
        }
    }

    void Off()
    {
        isBurning = false;
        Destroy(go_tempFlame);
    }

    void Damage()
    {
        currentDamangeTime = damageTime;

        GetComponent<StatusController>().DecreaseHP(damage);
    }

}
