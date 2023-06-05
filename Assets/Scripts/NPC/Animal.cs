using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{

    protected StatusController thePlayerStatus;
    [SerializeField] protected string animalName; //동물 이름
    [SerializeField] protected int hp; // 동물 체력
    [SerializeField] protected float walkSpeed;//걷기 스피드
    [SerializeField] protected float runSpeed;
    //[SerializeField] protected float turningSpeed; //회전 속도
    protected float applySpped;

    protected bool isWalking;// 걷는지 판별
    protected bool isAction; //행동 중인지 판별
    protected bool isRunning;
    protected bool isDead;
    protected bool isChasing;//추격중인디
    protected bool isAttacking;//공격중

    [SerializeField] protected float walkTime; //걷기 시간
    [SerializeField] protected float waitTime; // 대기시간
    [SerializeField] protected float runTime;

    protected float currentTime;
    protected Vector3 destination;// 목적지

    //필요 컴포넌트
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected BoxCollider boxCol;

    protected AudioSource theAudio;
    protected NavMeshAgent nav;
    protected FieldOfViewAngle theViewAngle;

    [SerializeField] protected AudioClip[] sound_normal; //일상 피그 사운드
    [SerializeField] protected AudioClip sound_hurt; //다칠때
    [SerializeField] protected AudioClip sound_Dead;

    private void Start()
    {
        thePlayerStatus = FindObjectOfType<StatusController>();
        theViewAngle = GetComponent<FieldOfViewAngle>();
        nav = GetComponent<NavMeshAgent>();
        currentTime = waitTime;
        theAudio = GetComponent<AudioSource>();
        isAction = true;
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            Debug.Log("+++++++ navPath $$$$ Path End Position: " + nav.pathEndPosition);
            Move();
            ElapseTime();
        }
    }

    /// <summary>
    /// 움직임
    /// </summary>
    protected void Move()
    {
        if (isWalking || isRunning)
            //rigid.MovePosition(transform.position + (transform.forward * applySpped * Time.deltaTime));
            nav.SetDestination(transform.position + destination * 5f);
    }

    protected void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0 && !isChasing && !isAttacking)
            {
                ReSet();
            }
           
        }
    }

    protected virtual void ReSet()
    {
        isWalking = false;
        isRunning = false;
        isAction = true;
        //언덕을 만나 왓다갓다 하는거 초기화
        Debug.Log("+++++++ navPath Before Path End Position: " + nav.pathEndPosition);


        nav.ResetPath();
        Debug.Log("+++++++ navPath After Path End Position: " + nav.pathEndPosition);


        nav.speed = walkSpeed;
        anim.SetBool("Walking", isWalking);
        anim.SetBool("Running", isRunning);
        destination.Set(UnityEngine.Random.Range(-0.2f, 0.2f), 0f, UnityEngine.Random.Range(0.5f, 1f));
    }




    protected void TryWalk()
    {
        isWalking = true;
        nav.speed = walkSpeed;
        currentTime = walkTime;
        anim.SetBool("Walking", isWalking);
        Debug.Log("걷기");
    }




    /// <summary>
    /// 데미지 받음
    /// </summary>
    /// <param name="_dmg"></param>
    /// <param name="_targetPos">Player 위치</param>
    public virtual void Damage(int _dmg, Vector3 _targetPos)
    {
        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)
            {
                Dead();
                return;
            }

            PlaySE(sound_hurt);

            anim.SetTrigger("Hurt");
        }

    }

    protected void Dead()
    {
        PlaySE(sound_Dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        anim.SetTrigger("Dead");
    }

    protected void RandomSound()
    {
        int random = UnityEngine.Random.Range(0, 3);//일상 사운드 3개
        PlaySE(sound_normal[random]);
    }

    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

}
