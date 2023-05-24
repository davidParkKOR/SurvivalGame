using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;//총의 이름
    public float range; //사정거리
    public float accuracy; //정확도
    public float fireRate; //연사속도
    public float reloadTime; //재장전 속도

    public int damage; //총의 데미지
    public int reloadBulletCount;//총의 재장전 개수.
    public int currentBulletCount;//현재 탄알집에 남아있는 총알의 갯수
    public int maxBulletCount; //최대 소유 가능 총알 갯수
    public int carryBulletCount; //현재 소유 하고 있는 총알의 갯수

    public float retroActionForce; //반동 세기
    public float retroActionFineSightForce; //정조준시 반동세기 (오른쪽버튼 누를시)

    public Vector3 fineSightOrginPos;
    public Animator anim;
    public ParticleSystem muzzleFlash;// 총 발사시 총구 섬광
    public AudioClip fire_Sound; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
