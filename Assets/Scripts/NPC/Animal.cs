using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{

    protected StatusController thePlayerStatus;
    [SerializeField] protected string animalName; //���� �̸�
    [SerializeField] protected int hp; // ���� ü��
    [SerializeField] protected float walkSpeed;//�ȱ� ���ǵ�
    [SerializeField] protected float runSpeed;
    //[SerializeField] protected float turningSpeed; //ȸ�� �ӵ�
    protected float applySpped;

    protected bool isWalking;// �ȴ��� �Ǻ�
    protected bool isAction; //�ൿ ������ �Ǻ�
    protected bool isRunning;
    protected bool isDead;
    protected bool isChasing;//�߰����ε�
    protected bool isAttacking;//������

    [SerializeField] protected float walkTime; //�ȱ� �ð�
    [SerializeField] protected float waitTime; // ���ð�
    [SerializeField] protected float runTime;

    protected float currentTime;
    protected Vector3 destination;// ������

    //�ʿ� ������Ʈ
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected BoxCollider boxCol;

    protected AudioSource theAudio;
    protected NavMeshAgent nav;
    protected FieldOfViewAngle theViewAngle;

    [SerializeField] protected AudioClip[] sound_normal; //�ϻ� �Ǳ� ����
    [SerializeField] protected AudioClip sound_hurt; //��ĥ��
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
    /// ������
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
        //����� ���� �Ӵٰ��� �ϴ°� �ʱ�ȭ
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
        Debug.Log("�ȱ�");
    }




    /// <summary>
    /// ������ ����
    /// </summary>
    /// <param name="_dmg"></param>
    /// <param name="_targetPos">Player ��ġ</param>
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
        int random = UnityEngine.Random.Range(0, 3);//�ϻ� ���� 3��
        PlaySE(sound_normal[random]);
    }

    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

}
