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

    [SerializeField] GameObject flame_prefab; //불 붙으면 프리팹 생성
    private GameObject go_tempFlame; //그릇

    private void Update()
    {
        if(isBurning)
        {
            //시간이 경과함에 따라 데미지 감소
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
                //데미지 입힘
                Damage();
            }

            if(currentDurationTime <= 0)
            {
                //불을 끔
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
