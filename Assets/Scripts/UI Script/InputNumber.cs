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
    //카메라 위치에 아이템 드롭
    private ActionController thePlayer;

    private void Update()
    {
        if(activated)
        {
            //엔터키를 누르면 종료
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
            //입력한것이 숫자인지 문자인지 체크
            if(CheckNumber(text_Input.text))
            {
                num = int.Parse(text_Input.text);

                if(num > DragSlot.instance.dragSlot.itemCount)
                    num = DragSlot.instance.dragSlot.itemCount;
            }
            else
                num = 1;
        }
        //아무것도 적지 않았을때
        else
            //입력 되지 않았으면 최대 갯수로 초기화
            num = int.Parse(text_Priveiw.text);

        StartCoroutine(DropItemCoroutine(num));
    }

    IEnumerator DropItemCoroutine(int _num)
    {
        for (int i = 0; i < _num; i++)
        {
            if(DragSlot.instance.dragSlot.item.itemPrefab != null)
            {
                //떨어뜨리면서 하나씩 생성함
                Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, thePlayer.transform.position + thePlayer.transform.forward, Quaternion.identity);
            }

            //슬롯에서 아이템 하나씩 없앰
            DragSlot.instance.dragSlot.SetSlotCount(-1);
            yield return new WaitForSeconds(0.05f);
        }

        //현재 아이템을 들고 있고, 그 아이템에 잇는 갯수를 모두 버릴경우 손에서 파괴
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
            //유니코드로 비교하여 숫자인지 문자인지 확인
            if (tempCharArray[i] >= 48 && tempCharArray[i] <= 57)
                continue;
            else
                isNumber = false;
        }

        return isNumber;

    }


}
