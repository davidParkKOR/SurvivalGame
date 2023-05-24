using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //���� �ν�����â���� ������ �ʰԵ�
    //������ Serialized�� �����ϸ� ��ȣ������ �����Ǹ� �ν����� â���� ���̰Ե� 

    //���ǵ� ���� ����
    [SerializeField]
    private float walkSpped;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed; //�ɱ�
    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    //���º���
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    //������ üũ ����
    private Vector3 lastPos;

    //�ɾ����� �󸶳� ������ �����ϴ� ����
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    //�� ���� ���� �Ǵ�
    private CapsuleCollider capsuleCollider;

    // �ΰ���
    [SerializeField]
    private float lookSensitivity; //ī�޶� �ΰ���

    //ī�޶� �Ѱ�
    [SerializeField]
    private float cameraRotationLimit; //ī�޶� ȸ�� ����
    private float currentCameraRotationX = 0f;

    //�ʿ� ������Ʈ
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid; //�ΰ��� ��
    private Corsshair theCrosshair;

    private GunController theGunController;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Corsshair>();

        applySpeed = walkSpped;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;

    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        MoveCheck();
        CameraRotation();
        CharacterRoation();
    }

    //�ɱ� �õ�
    void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    void Crouch()
    {
        isCrouch = !isCrouch;
        theCrosshair.CrouchingAnimation(isCrouch);


        if (isCrouch) 
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpped;
            applyCrouchPosY = originPosY;

        }

        StartCoroutine(CrouchCoroutine());
        //theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x, applyCrouchPosY, theCamera.transform.localPosition.z);
    }

    //�ε巯�� �ɱ� �õ�
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(_posY != applyCrouchPosY)
        {

            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);

            if (count > 15)
                break;
            
            yield return null;
        }

        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }

    //���� üũ
    void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        theCrosshair.JumpingAnimation(!isGround);
    }

    //���� �õ�
    void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }

    //����
    void Jump()
    {
        //���� ���¿��� ������ ���� ���� ����
        if (isCrouch)
            Crouch();

        //(0,1,0)
        myRigid.velocity = transform.up * jumpForce;
    }

    //�޸����� ���� �Ǵ�
    void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancle();
        }
    }

    //�޸���
    void Running()
    {
        //���� ���¿��� ������ ���� ���� ����
        if (isCrouch)
            Crouch();

        theGunController.CancleFineSight();       

        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = runSpeed;
    }

    //�޸��� ���
    void RunningCancle()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpped;
    }

    //������
    void Move()
    {
        // L,R,Null : 1 , -1 , 0
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        //��ֶ������ ���ִ� ����
        //(1,0,0)
        //(0,0,1)
        //(1,0,1) = 2
        //(0.5,0,0.5) = 1
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    void MoveCheck()
    {
        if (!isRun && !isCrouch && !isGround)
        {
            //������ ��� ���� �̼��ϰ� �����̰� �������̶� �װ��� ������
            if(Vector3.Distance(lastPos, transform.position) >= 0.01f)
                isWalk = true;
            else
                isWalk = false;
   
            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position; 

        }
    }

    //���� ī�޶� ȸ��
    void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // -45, 45�� ����

        //���Ϸ� �ޱ� = rotation x,y,z
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }


    //�¿� ĳ���� ȸ��
    void CharacterRoation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotaionY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotaionY));
    }



   
}