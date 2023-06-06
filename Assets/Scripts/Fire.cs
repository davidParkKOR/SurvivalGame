using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private string fireName;//불의 이름(난로, 모닥불, 화롯불)
    [SerializeField] private int damage; //불의 데미지

    [SerializeField] private float damageTime;//데미지가 들어갈 딜레이    
    private float currentDamageTime;

    [SerializeField] private float durationTime;//불의 지속시간
    private float currentDurationTime;

    [SerializeField] private ParticleSystem ps_Flame; //파티클 시스템

    //상태변수
    private bool isFire = true;


    private StatusController thePlayerStatus;


    private void Start()
    {
        thePlayerStatus = FindObjectOfType<StatusController>();
        currentDurationTime = durationTime;

    }

    private void Update()
    {
        //불이 켜졋는지 안켠
        if(isFire)
        {
            ElapseTime();
        }
    }


    void ElapseTime()
    {
        currentDurationTime -= Time.deltaTime;

        if(currentDamageTime > 0)
            currentDamageTime -= Time.deltaTime;

        if(currentDurationTime < 0)
        {
            //불끔
            Off();
        }
    }

    void Off()
    {
        ps_Flame.Stop();
        isFire = false;
    }


    //불 위에 있으면 불이 꺼지도록
    private void OnTriggerStay(Collider other)
    {
        if(isFire && other.transform.tag == "Player")
        {
            if(currentDamageTime <= 0)
            {
                other.GetComponent<Burn>().StartBurning();
                thePlayerStatus.DecreaseHP(damage);
                currentDamageTime = damageTime;
            }

        }
    }

    public bool GetIsFire()
    {
        return isFire;
    }

}
