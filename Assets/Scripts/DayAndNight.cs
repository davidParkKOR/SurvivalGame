using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNight : MonoBehaviour
{
    [SerializeField] float secondPerRealTimeSecond; //게임세계의 100초 현실세계의 1초
    [SerializeField] float nightFogDensity;//밤 상태의 Fog 밀도
    [SerializeField] float fogDensityCalc;// 증감량 비율

    float dayFogDensity; //낮 상태의 fog 밀도
    float currentFogDensity;

    private void Start()
    {
        nightFogDensity = RenderSettings.fogDensity;
    }

    private void Update()
    {
        transform.Rotate(Vector3.right, 0.1f * secondPerRealTimeSecond * Time.deltaTime);

        //특정각도로 밤 낮 구분
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
