using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private string fireName;//���� �̸�(����, ��ں�, ȭ�Ժ�)
    [SerializeField] private int damage; //���� ������

    [SerializeField] private float damageTime;//�������� �� ������    
    private float currentDamageTime;

    [SerializeField] private float durationTime;//���� ���ӽð�
    private float currentDurationTime;

    [SerializeField] private ParticleSystem ps_Flame; //��ƼŬ �ý���

    //���º���
    private bool isFire = true;


    private StatusController thePlayerStatus;


    private void Start()
    {
        thePlayerStatus = FindObjectOfType<StatusController>();
        currentDurationTime = durationTime;

    }

    private void Update()
    {
        //���� �Ѡ����� ����
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
            //�Ҳ�
            Off();
        }
    }

    void Off()
    {
        ps_Flame.Stop();
        isFire = false;
    }


    //�� ���� ������ ���� ��������
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
