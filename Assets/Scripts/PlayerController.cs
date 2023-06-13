using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    static public bool isActivated = true;
    //원래 인스펙터창에서 보이지 않게됨
    //하지만 Serialized를 선언하면 보호수준은 유지되며 인스펙터 창에서 보이게됨 

    //스피드 조정 변수
    [SerializeField]
    private float walkSpped;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed; //앉기
    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    [SerializeField] float swimSpeed;
    [SerializeField] float swimFastSpeed;
    [SerializeField] float upSwimSpeed;

    //상태변수
    private bool isWalk = false;
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;

    //움직임 체크 변수
    private Vector3 lastPos;

    //앉았을때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    //땅 착지 여부 판단
    private CapsuleCollider capsuleCollider;

    // 민감도
    [SerializeField]
    private float lookSensitivity; //카메라 민감도

    //카메라 한계
    [SerializeField]
    private float cameraRotationLimit; //카메라 회전 제한
    private float currentCameraRotationX = 0f;

    //필요 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid; //인간의 몸
    private Corsshair theCrosshair;

    private GunController theGunController;
    private StatusController theStatusController;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        theGunController = FindObjectOfType<GunController>();
        theCrosshair = FindObjectOfType<Corsshair>();
        theStatusController = FindObjectOfType<StatusController>();

        applySpeed = walkSpped;
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;

    }

    // Update is called once per frame
    void Update()
    {
        if(isActivated && GameManager.canPlayerMove)
        {
            WaterCheck();
            IsGround();
            TryJump();

            if (!GameManager.isWater)
                TryRun();
       

            TryCrouch();
            Move();
            MoveCheck();
            CameraRotation();
            CharacterRoation();

        }

    }

    void WaterCheck()
    {
        if(GameManager.isWater)
        {
            applySpeed = swimSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
                applySpeed = swimFastSpeed;
            else
                applySpeed = swimSpeed;
        }
    }

    //앉기 시도
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


    //부드러운 앉기 시도
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


    //지면 체크
    void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        theCrosshair.JumpingAnimation(!isGround);
    }

    //점프 시도
    void TryJump()
    {
      
        if(Input.GetKeyDown(KeyCode.Space) && isGround && theStatusController.GetCurrentSP() > 0)
        {
            Debug.Log("Jump");
            Jump();
        }
        else if (Input.GetKey(KeyCode.Space) && GameManager.isWater)
        {
            Debug.Log("UpSwim");
            UpSwim();
        }

    }

    void UpSwim()
    {
        myRigid.velocity = transform.up * upSwimSpeed;
    }

    //점프
    void Jump()
    {
        //앉은 상태에서 점프시 앉은 상태 해제
        if (isCrouch)
            Crouch();

        theStatusController.DecreaseStamina(100);

        //(0,1,0)
        myRigid.velocity = transform.up * jumpForce;
    }

    //달리는지 여부 판단
    void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && theStatusController.GetCurrentSP() > 0)
        {
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || theStatusController.GetCurrentSP() <= 0)
        {
            RunningCancle();
        }
    }

    //달리기
    void Running()
    {
        //앉은 상태에서 점프시 앉은 상태 해제
        if (isCrouch)
            Crouch();

        theGunController.CancleFineSight();       

        isRun = true;
        theCrosshair.RunningAnimation(isRun);
        theStatusController.DecreaseStamina(10);

        applySpeed = runSpeed;
    }

    //달리기 취소
    void RunningCancle()
    {
        isRun = false;
        theCrosshair.RunningAnimation(isRun);
        applySpeed = walkSpped;
    }

    //움직임
    void Move()
    {
        // L,R,Null : 1 , -1 , 0
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        //노멀라이즈드 해주는 이유
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
            //경사로의 경우 아주 미세하게 움직이고 잇을것이라 그것을 방지함
            if(Vector3.Distance(lastPos, transform.position) >= 0.01f)
                isWalk = true;
            else
                isWalk = false;
   
            theCrosshair.WalkingAnimation(isWalk);
            lastPos = transform.position; 

        }
    }

    //상하 카메라 회전
    void CameraRotation()
    {
        if(!pauseCameraRotation)
        {
            float _xRotation = Input.GetAxisRaw("Mouse Y");
            float _cameraRotationX = _xRotation * lookSensitivity;
            currentCameraRotationX -= _cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit); // -45, 45에 가둠

            //오일러 앵글 = rotation x,y,z
            theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }

    }

    bool pauseCameraRotation = false;

    public IEnumerator TreeLookCoroutine(Vector3 _target)
    {
        pauseCameraRotation = true;
        Quaternion direction = Quaternion.LookRotation(_target - theCamera.transform.position);
        Vector3 eulerValue = direction.eulerAngles;
        float destinationX = eulerValue.x;

        while(Mathf.Abs(destinationX - currentCameraRotationX) >= 0.5f)
        {
            eulerValue = Quaternion.Lerp(theCamera.transform.localRotation, direction, 0.3f).eulerAngles;
            theCamera.transform.localRotation = Quaternion.Euler(eulerValue.x, 0f, 0f);
            currentCameraRotationX = theCamera.transform.eulerAngles.x;
            yield return null;
        }

        pauseCameraRotation = false;
    }


    //좌우 캐릭터 회전
    void CharacterRoation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotaionY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotaionY));
    }


    public bool GetRun()
    {
        return isRun;
    }



   
}
