using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputNumber : MonoBehaviour
{
    private bool activated = false;
    [SerializeField]
    private Text text_Priveiw;
    [SerializeField] 
    private Text text_Input;
    [SerializeField]
    private InputField if_Text;

    [SerializeField]
    private GameObject go_Base;

    [SerializeField]
    //ī�޶� ��ġ�� ������ ���
    private ActionController thePlayer;

    private void Update()
    {
        if(activated)
        {
            //����Ű�� ������ ����
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OK();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cancle();
            }
        }
    }

    public void Call()
    {
       
        go_Base.SetActive(true);
        activated = true;
        if_Text.text = "";
        text_Priveiw.text = DragSlot.instance.dragSlot.itemCount.ToString();
    }

    public void Cancle()
    {
       
        go_Base.SetActive(false);
        activated = false;
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OK()
    {
        DragSlot.instance.SetColor(0);

        int num;
        if(text_Input.text != "")
        {
            //�Է��Ѱ��� �������� �������� üũ
            if(CheckNumber(text_Input.text))
            {
                num = int.Parse(text_Input.text);

                if(num > DragSlot.instance.dragSlot.itemCount)
                    num = DragSlot.instance.dragSlot.itemCount;
            }
            else
                num = 1;
        }
        //�ƹ��͵� ���� �ʾ�����
        else
            //�Է� ���� �ʾ����� �ִ� ������ �ʱ�ȭ
            num = int.Parse(text_Priveiw.text);

        StartCoroutine(DropItemCoroutine(num));
    }

    IEnumerator DropItemCoroutine(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            if(DragSlot.instance.dragSlot.item.itemPrefab != null)
            {
                //����߸��鼭 �ϳ��� ������
                Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, thePlayer.transform.position + thePlayer.transform.forward, Quaternion.identity);
            }

            //���Կ��� ������ �ϳ��� ����
            DragSlot.instance.dragSlot.SetSlotCount(-1);
            yield return new WaitForSeconds(0.05f);
        }

        //���� �������� ��� �ְ�, �� �����ۿ� �մ� ������ ��� ������� �տ��� �ı�
        if(int.Parse(text_Priveiw.text) == _num)
        {
            if(QuickSlotController.go_HandItem != null)
            {
                Destroy(QuickSlotController.go_HandItem);
            }
        }

        DragSlot.instance.dragSlot = null;
        go_Base.SetActive(false);
        activated = false;
    }

    bool CheckNumber(string _argString)
    {
        char[] tempCharArray = _argString.ToCharArray();
        
        bool isNumber = true;

        for (int i = 0; i < tempCharArray.Length; i++)
        {
            //�����ڵ�� ���Ͽ� �������� �������� Ȯ��
            if (tempCharArray[i] >= 48 && tempCharArray[i] <= 57)
                continue;
            else
                isNumber = false;
        }

        return isNumber;

    }


}
