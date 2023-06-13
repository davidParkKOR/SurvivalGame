using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public static bool isActivated = true;
    //������ġ
    private Vector3 originPos;

    //������ġ
    private Vector3 currentPos;

    //���Ⱑ ��鸱�� �󸸳� ��鸱��
    [SerializeField]
    private Vector3 limitPos;

    //������ Sway �Ѱ�
    [SerializeField]
    private Vector3 fineSightLimitPos;

    //�ε巯������
    [SerializeField]
    private Vector3 smoothSway;

    //�ʿ� ������Ʈ
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
            //�κ��丮 Ȱ��ȭ�� ���� ������ ����
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
            //���콺�� �������� ������ ���� ��ġ�� ���ư�
            BackToOriginPos();
        }
    }

    void Swaying()
    {
        float moveX = Input.GetAxisRaw("Mouse X");
        float moveY = Input.GetAxisRaw("Mouse Y");


        if(!theGunController.GetFineSightMode())
        {
            //ȭ�� ������ ����� �ʵ�����
            //Clamp ���θ�
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


    //���콺�� �������� ������ �ٽ� �����ڸ��� ���ư�
    void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos;
    }
}
