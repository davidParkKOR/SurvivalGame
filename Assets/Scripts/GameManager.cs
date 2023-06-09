using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true; //�÷��̾� ������ ����
    public static bool isOpenInventory = false; //�κ��丮 Ȱ��ȭ
    public static bool isOpenArchemyTable = false; //�������̺� â Ȱ��ȭ
    public static bool isOpenCraftManual = false; //����(ũ����Ʈ)�޴�â Ȱ��ȭ

    private void Update()
    {
        //���� �ϳ��� Ȱ��ȭ �Ǿ 
        //�÷��̾� ���� ����
        if (isOpenInventory || isOpenCraftManual || isOpenArchemyTable)
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
    }

    private void Start()
    {
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
