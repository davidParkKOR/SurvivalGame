using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public static bool isActivated = true;
    //기존위치
    private Vector3 originPos;

    //현재위치
    private Vector3 currentPos;

    //무기가 흔들릴때 얼만나 흔들릴지
    [SerializeField]
    private Vector3 limitPos;

    //정조준 Sway 한계
    [SerializeField]
    private Vector3 fineSightLimitPos;

    //부드러운정도
    [SerializeField]
    private Vector3 smoothSway;

    //필요 컴포넌트
    [SerializeField]
    private GunController theGunController;

    // Start is called before the first frame update
    void Start()
    {
        originPos = this.transform.localPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.canPlayerMove && isActivated)
        {
            //인벤토리 활성화시 웨폰 움직임 막기
            TrySway();
        }

    }

    void TrySway()
    {
        if(Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y")!= 0)
        {
            Swaying();
        }
        else
        {
            //마우스가 움직이지 않을때 원래 위치로 돌아감
            BackToOriginPos();
        }
    }

    void Swaying()
    {
        float moveX = Input.GetAxisRaw("Mouse X");
        float moveY = Input.GetAxisRaw("Mouse Y");


        if(!theGunController.GetFineSightMode())
        {
            //화면 밖으로 벗어나지 않도록함
            //Clamp 가두리
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -moveX, smoothSway.x), -limitPos.x, limitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -moveY, smoothSway.x), -limitPos.y, limitPos.y),
                           originPos.z);
        
        }
        else
        {
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -moveX, smoothSway.y), -fineSightLimitPos.x, fineSightLimitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y),
                           originPos.z);
        }

        transform.localPosition = currentPos;

    }


    //마우스가 움직이지 않을때 다시 원래자리로 돌아감
    void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos;
    }
}
