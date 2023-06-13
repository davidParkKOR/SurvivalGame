using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true; //�÷��̾� ������ ����
    public static bool isOpenInventory = false; //�κ��丮 Ȱ��ȭ
    public static bool isOpenArchemyTable = false; //�������̺� â Ȱ��ȭ
    public static bool isOpenCraftManual = false; //����(ũ����Ʈ)�޴�â Ȱ��ȭ
    public static bool isNight = false;//�㳷
    public static bool isWater = false;
    public static bool isPause;

    private WeaponManager theWM;
    bool flag = false;

    private void Update()
    {
        //���� �ϳ��� Ȱ��ȭ �Ǿ 
        //�÷��̾� ���� ����
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
