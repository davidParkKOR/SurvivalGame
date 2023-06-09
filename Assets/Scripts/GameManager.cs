using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true; //플레이어 움직임 제어
    public static bool isOpenInventory = false; //인벤토리 활성화
    public static bool isOpenArchemyTable = false; //연금테이블 창 활성화
    public static bool isOpenCraftManual = false; //건축(크래프트)메뉴창 활성화
    public static bool isNight = false;//밤낮
    public static bool isWater = false;
    public static bool isPause;

    private WeaponManager theWM;
    bool flag = false;

    private void Update()
    {
        //둘중 하나만 활성화 되어도 
        //플레이어 무브 방지
        if (isOpenInventory || isOpenCraftManual || isOpenArchemyTable || isPause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canPlayerMove = false;
        }
            
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canPlayerMove = true;
        }

        if(isWater)
        {
            if (!flag)
            {
                StopAllCoroutines();
                theWM.StartCoroutine(theWM.WeaponInCoroutine());
                flag = true;
            }
        }   
        else
        {
            if (flag)
            {
                flag = false;
                theWM.WeaponOutCoroutine(); 
      
            }
        }
    }

    private void Start()
    {
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible = false;
        theWM = FindObjectOfType<WeaponManager>();
    }
}
