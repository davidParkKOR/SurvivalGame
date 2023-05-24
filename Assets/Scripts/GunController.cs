using System.Collections;
//using System.Collections.Generic;
//using System.Numerics;
using System.Transactions;
using UnityEngine;

public class GunController : MonoBehaviour
{
    //Ȱ��ȭ ����
    public static bool isActivate = false;

    //���� ������ ��
    [SerializeField]
    private Gun currentGun;
    
    //���� �ӵ� ���
    private float currentFireRate;

    //���º���

    private bool isReload = false;
    [HideInInspector]
    private bool isFineSightMode = false;

    //���� ������ ��
    [SerializeField]
    private Vector3 orginPos;

    //ȿ����
    private AudioSource audioSource;

    //������ �浹 ���� �޾ƿ�
    private RaycastHit hitInfo;

    //�ʿ��� ������Ʈ 
    [SerializeField]
    private Camera theCam;
    private Corsshair theCrosshair;

    //�浹 ����Ʈ
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
            TryReload(); //RŰ ������ ���� ������ �ǵ���
            TryFineSight();
        }

    }

    //����ӵ� ����
    void GunFireRateCalc()
    {
        if(currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime; // 60���� 1
        }
    }

    //�߻� �õ�
    void TryFire()
    {
        if(Input.GetButton("Fire1") && currentFireRate <=0 && !isReload)
        {
            Fire();
        }
    }

    //������ �õ�
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

    //�߻��� ���
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

    //�߻��� ���
    void Shoot()
    {
        theCrosshair.FindAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; //���� �ӵ� ����
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());         //�ѱ� �ݵ� �ڷ�ƾ ����
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
                //������ ���ɼ��� 5�� ������ 5�� �ؾߵ�
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }
            isReload = false;
        }
        else
        {
            Debug.Log("������ �Ѿ��� �����ϴ�.");
        }

    }

    //������ �õ�
    void TryFineSight()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            FineSight(); 
        }
    }

    //������ ���� ����
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

    //������ ���
    public void CancleFineSight()
    {
        if (isFineSightMode)
            FineSight();
    }

    //������ Ȱ��ȭ
    IEnumerator FineSightActivateCoroutine()
    {
        while (currentGun.transform.localPosition != currentGun.fineSightOrginPos)
        {
            currentGun.transform.localPosition = UnityEngine.Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOrginPos,0.2f);
            yield return null;
        }
    }


    //������ ��Ȱ��ȭ
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
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, orginPos.y, orginPos.z); //���� �������� �ִ� �ݵ�
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOrginPos.y, currentGun.fineSightOrginPos.z);

        if (!isFineSightMode)
        {
            currentGun.transform.localPosition = orginPos;

            //�ݵ�����
            while (currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }
            
            //������ �������� �ݵ�
            while (currentGun.transform.localPosition != orginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, orginPos, 0.1f);
                yield return null; 
            }
        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOrginPos;

            //�ݵ�����
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            //������ �������� �ݵ�
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
