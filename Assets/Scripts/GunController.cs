using System.Collections;
//using System.Collections.Generic;
//using System.Numerics;
using System.Transactions;
using UnityEngine;

public class GunController : MonoBehaviour
{
    //활성화 여부
    public static bool isActivate = false;

    //현재 장착된 총
    [SerializeField]
    private Gun currentGun;
    
    //연사 속도 계산
    private float currentFireRate;

    //상태변수

    private bool isReload = false;
    [HideInInspector]
    private bool isFineSightMode = false;

    //본래 포지션 값
    [SerializeField]
    private Vector3 orginPos;

    //효과음
    private AudioSource audioSource;

    //레이저 충돌 정보 받아옴
    private RaycastHit hitInfo;

    //필요한 컴포넌트 
    [SerializeField]
    private Camera theCam;
    private Corsshair theCrosshair;

    //충돌 이펙트
    [SerializeField]
    private GameObject hit_Effect_prefab;



    // Start is called before the first frame update
    void Start()
    {
        orginPos = Vector3.zero;
        audioSource = GetComponent<AudioSource>();
        theCrosshair = FindObjectOfType<Corsshair>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivate)
        {
            GunFireRateCalc();
            TryFire();
            TryReload(); //R키 누르면 수동 재장전 되도록
            TryFineSight();
        }

    }

    //연사속도 재계산
    void GunFireRateCalc()
    {
        if(currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime; // 60분의 1
        }
    }

    //발사 시도
    void TryFire()
    {
        if(Input.GetButton("Fire1") && currentFireRate <=0 && !isReload)
        {
            Fire();
        }
    }

    //재장전 시도
    void TryReload()
    {
        if(Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancleFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void CancelReload()
    {
        if(isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }

    //발사전 계산
    void Fire()
    {
        if(!isReload)
        {
            if (currentGun.currentBulletCount > 0)
            {
                Shoot();
            }
            else
            {
                CancleFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }

    }

    //발사후 계산
    void Shoot()
    {
        theCrosshair.FindAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; //연사 속도 재계산
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());         //총기 반동 코루틴 실행
    }

    void Hit()
    {
        if(Physics.Raycast(theCam.transform.position, 
                           theCam.transform.forward + new Vector3(Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy), 
                                                                  Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy),
                                                                  0),
                           out hitInfo, currentGun.range))
        {
            var clone = Instantiate(hit_Effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
        }
    }

    IEnumerator ReloadCoroutine()
    {
        if(currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if(currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                //재장전 가능수가 5면 재장전 5만 해야됨
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }
            isReload = false;
        }
        else
        {
            Debug.Log("소유한 총알이 없습니다.");
        }

    }

    //정조준 시도
    void TryFineSight()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            FineSight(); 
        }
    }

    //정조준 로직 가동
    void FineSight()
    {
        isFineSightMode = !isFineSightMode;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrosshair.FineSightAnimation(isFineSightMode);

        if(isFineSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }
    }

    //정조준 취소
    public void CancleFineSight()
    {
        if (isFineSightMode)
            FineSight();
    }

    //정조준 활성화
    IEnumerator FineSightActivateCoroutine()
    {
        while (currentGun.transform.localPosition != currentGun.fineSightOrginPos)
        {
            currentGun.transform.localPosition = UnityEngine.Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOrginPos,0.2f);
            yield return null;
        }
    }


    //정조준 비활성화
    IEnumerator FineSightDeactivateCoroutine()
    {
        while (currentGun.transform.localPosition != orginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, orginPos, 0.2f);
            yield return null;
        }
    }

    //
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, orginPos.y, orginPos.z); //조준 안했을때 최대 반동
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOrginPos.y, currentGun.fineSightOrginPos.z);

        if (!isFineSightMode)
        {
            currentGun.transform.localPosition = orginPos;

            //반동시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }
            
            //정조준 안했을때 반동
            while (currentGun.transform.localPosition != orginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, orginPos, 0.1f);
                yield return null; 
            }
        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOrginPos;

            //반동시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            //정조준 안했을때 반동
            while (currentGun.transform.localPosition != currentGun.fineSightOrginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOrginPos, 0.1f);
                yield return null;
            }
        }
    }

    //Sound Effect
    void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun()
    {
        return currentGun;
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    public void GunChange(Gun _gun)
    {
        if(WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;
        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;

    }
}
