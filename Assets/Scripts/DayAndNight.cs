using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [SerializeField] float secondPerRealTimeSecond; //���Ӽ����� 100�� ���Ǽ����� 1��
    [SerializeField] float nightFogDensity;//�� ������ Fog �е�
    [SerializeField] float fogDensityCalc;// ������ ����

    float dayFogDensity; //�� ������ fog �е�
    float currentFogDensity;

    private void Start()
    {
        nightFogDensity = RenderSettings.fogDensity;
    }

    private void Update()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        //Ư�������� �� �� ����
        if (transform.eulerAngles.x >= 170)
        {
            GameManager.isNight = true;
        }
           
        else if (transform.eulerAngles.x <= 10)
        {
            GameManager.isNight = false;
        }





        //Debug.Log("CurrentFogDensity : " + currentFogDensity);
        //Debug.Log("nightFogDensity : " + nightFogDensity);
        //Debug.Log("dayFogDensity : " + dayFogDensity);

        if (GameManager.isNight)
        {
            if (currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            } 
        }
        else
        {
            if (currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }

    }
}
