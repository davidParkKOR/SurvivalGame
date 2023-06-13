using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Water : MonoBehaviour
{
    [SerializeField] float waterDrag; //¹°¼Ó Áß·Â 
    [SerializeField] Color waterColor;// ¹°¼Ó »ö±ò
    [SerializeField] float waterFogDensity;//¹° Å¹ÇÔÁ¤µµ
    [SerializeField] Color originNightColor;
    [SerializeField] float originNightFogDensity;
    [SerializeField] Color waterNightColor; //¹ã »óÅÂÀÇ ¹°¼Ó »ö±ò
    [SerializeField] float waterNightFogDensity;

    [SerializeField] string sound_WaterOut;
    [SerializeField] string sound_WaterIn;
    [SerializeField] string sound_Breath;
    [SerializeField] float breathTime;

    [SerializeField] GameObject go_BaseUI;
    [SerializeField] Text text_totalOxygen;
    [SerializeField] float totalOxygen;
    [SerializeField] Text text_CurrentOxygen;
    [SerializeField] Image image_Gauge;



    float originDrag;
    float originFogDensity;//¹° Å¹ÇÔÁ¤µµ
    float currentBreathTime;
    float currentOxygen;
    float temp;
    Color originColor;// ¹°¼Ó »ö±ò
    StatusController thePlayerStatus;



    private void Start()
    {
        originColor = RenderSettings.fogColor;
        originFogDensity = RenderSettings.fogDensity;
        originDrag = 0;
        thePlayerStatus = FindObjectOfType<StatusController>();
        currentOxygen = totalOxygen;
        text_totalOxygen.text = totalOxygen.ToString();
    }

    private void Update()
    {
        if(GameManager.isWater)
        {
            currentBreathTime += Time.deltaTime;

            if(currentBreathTime >= breathTime)
            {
                SoundManager.instance.PlaySE(sound_Breath);
                currentBreathTime = 0;

            }
    
        }

        DecreaseOxygen();
    }

    void DecreaseOxygen()
    {
        if(GameManager.isWater)
        {
            currentOxygen -= Time.deltaTime;

            if (currentOxygen <= 0) 
                currentOxygen = 0;

            text_CurrentOxygen.text = Mathf.RoundToInt(currentOxygen).ToString();
            image_Gauge.fillAmount = currentOxygen / totalOxygen;

            if (currentOxygen <= 0)
            {
                text_CurrentOxygen.text = "0";
                temp += Time.deltaTime;

                if (temp <= 1)
                {
                    thePlayerStatus.DecreaseHP(1);
                    temp = 0;
                }    
            }
   
        }
    }


    private void OnTriggerEnter(Collider other)
    {        
        if(other.transform.tag == "Player")
        {
            GetInWater(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            GetOutWater(other);
        }
    }

    void GetInWater(Collider _player)
    {
        SoundManager.instance.PlaySE(sound_WaterIn);
        go_BaseUI.SetActive(true);
        GameManager.isWater = true;
        _player.transform.GetComponent<Rigidbody>().drag = waterDrag;

        if(!GameManager.isNight)
        {
            RenderSettings.fogColor = waterColor;
            RenderSettings.fogDensity = waterFogDensity;
        }
        else
        {
            RenderSettings.fogColor = waterNightColor;
            RenderSettings.fogDensity = waterNightFogDensity;
        }


    }
    void GetOutWater(Collider _player)
    {


        if (GameManager.isWater)
        {
            go_BaseUI.SetActive(false);

            currentOxygen = totalOxygen;
            SoundManager.instance.PlaySE(sound_WaterOut);
            GameManager.isWater = false;
            _player.transform.GetComponent<Rigidbody>().drag = originDrag;
        }

        if(!GameManager.isNight)
        {
            RenderSettings.fogColor = originColor;
            RenderSettings.fogDensity = originFogDensity;
        }
        else
        {
            RenderSettings.fogColor = originNightColor;
            RenderSettings.fogDensity = originNightFogDensity;
        }


    }
}
