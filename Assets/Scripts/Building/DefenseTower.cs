using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseTower : MonoBehaviour
{
    [SerializeField] string towerName;//방어 타워 이름
    [SerializeField] float range; //방어타워  사정거리
    [SerializeField] int damage;//공격력
    [SerializeField] float rateOfAccuracy; //정확도
    [SerializeField] float rateOfFire;//연사속도
    [SerializeField] float viewAngle;//시야각
    [SerializeField] float spinSpeed;//회전속도
    [SerializeField] LayerMask layerMask;//움직이는 대상만 타겟으로 지정 (플레이어)
    [SerializeField] Transform tf_TopGun;//방어 타워의 포탑
    [SerializeField] ParticleSystem particle_MuzzleFlash;//총구 섬광
    [SerializeField] GameObject go_HitEffect_Prefab;//적중 효과 이펙트 
    [SerializeField] AudioClip sound_Fire;

    float currentRateOfFire;//연사속도 계산
    bool isFindTarget = false;//적 타겟 발견시 true
    bool isAttack = false; //총구 방향과 적 방향이 일치할 시 true
    RaycastHit hitInfo;//광선 충돌객ㅊ페의 정보
    Transform tf_Target; // 타겟의 위치 정보
    Animator anim;
    AudioSource theAudio;

    private void Start()
    {
        theAudio = GetComponent<AudioSource>();
        theAudio.clip = sound_Fire;
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Spin(); //360도 회전시킴 
        SearchEnemy(); //적을 찾음
        LookTarget(); //포지션을 적으로 고정
        Attack(); //공격
    }

    void Spin()
    {
        if(!isFindTarget && !isAttack)
        {
            //매프레임마다 포지션 회전
            Quaternion spin = Quaternion.Euler(0f, tf_TopGun.eulerAngles.y + (1f * spinSpeed * Time.deltaTime), 0f);
            tf_TopGun.rotation = spin;
        }
    }

    void SearchEnemy()
    {
        //범위 안의 타겟을 모두 담음
        Collider[] targets = Physics.OverlapSphere(tf_TopGun.position, range, layerMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Transform targetTf = targets[i].transform;

            if(targetTf.name == "Player")
            {
                // 자기와 상대방의 방향
                Vector3 direction = (targetTf.position - tf_TopGun.position).normalized;
                float angle = Vector3.Angle(direction, tf_TopGun.forward);

                if (angle < viewAngle * 0.5f)
                {
                    tf_Target = targetTf;
                    isFindTarget = true;//타겟 찾음

                    if (angle < 5f)
                    {
                        //발사 시작
                        isAttack = true;
                    }
                    else
                        isAttack = false;

                    return;
                }
            }

        }

        tf_Target = null;
        isAttack = false;
        isFindTarget = false;

    }

    void LookTarget()
    {
        if(isFindTarget)
        {
            Vector3 direction = (tf_Target.position - tf_TopGun.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            //자연스럽게 바라보도록 함
            Quaternion rotation = Quaternion.Lerp(tf_TopGun.rotation, lookRotation, 0.2f);
            tf_TopGun.rotation = rotation;
        }
    }

    void Attack()
    {
        if(isAttack)
        {
            currentRateOfFire += Time.deltaTime;

            if(currentRateOfFire >= rateOfFire)
            {
                currentRateOfFire = 0;
                anim.SetTrigger("Fire");
                theAudio.Play();
                particle_MuzzleFlash.Play();

                //총알이 박힘
                if (Physics.Raycast(tf_TopGun.position,
                        tf_TopGun.forward + new Vector3(Random.Range(-1, 1f) * Random.Range(-1, 1f) * rateOfAccuracy, 0f),
                        out hitInfo, range, layerMask))
                        {
                            //맞은 거리에 생성
                            GameObject temp = Instantiate(go_HitEffect_Prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                            Destroy(temp, 1f);

                            if(hitInfo.transform.name == "Player")
                            {
                                hitInfo.transform.GetComponent<StatusController>().DecreaseHP(damage);
                            }
                        }
            }
        }
    }








}
