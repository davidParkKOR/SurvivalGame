using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseTower : MonoBehaviour
{
    [SerializeField] string towerName;//��� Ÿ�� �̸�
    [SerializeField] float range; //���Ÿ��  �����Ÿ�
    [SerializeField] int damage;//���ݷ�
    [SerializeField] float rateOfAccuracy; //��Ȯ��
    [SerializeField] float rateOfFire;//����ӵ�
    [SerializeField] float viewAngle;//�þ߰�
    [SerializeField] float spinSpeed;//ȸ���ӵ�
    [SerializeField] LayerMask layerMask;//�����̴� ��� Ÿ������ ���� (�÷��̾�)
    [SerializeField] Transform tf_TopGun;//��� Ÿ���� ��ž
    [SerializeField] ParticleSystem particle_MuzzleFlash;//�ѱ� ����
    [SerializeField] GameObject go_HitEffect_Prefab;//���� ȿ�� ����Ʈ 
    [SerializeField] AudioClip sound_Fire;

    float currentRateOfFire;//����ӵ� ���
    bool isFindTarget = false;//�� Ÿ�� �߽߰� true
    bool isAttack = false; //�ѱ� ����� �� ������ ��ġ�� �� true
    RaycastHit hitInfo;//���� �浹�������� ����
    Transform tf_Target; // Ÿ���� ��ġ ����
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
        Spin(); //360�� ȸ����Ŵ 
        SearchEnemy(); //���� ã��
        LookTarget(); //�������� ������ ����
        Attack(); //����
    }

    void Spin()
    {
        if(!isFindTarget && !isAttack)
        {
            //�������Ӹ��� ������ ȸ��
            Quaternion spin = Quaternion.Euler(0f, tf_TopGun.eulerAngles.y + (1f * spinSpeed * Time.deltaTime), 0f);
            tf_TopGun.rotation = spin;
        }
    }

    void SearchEnemy()
    {
        //���� ���� Ÿ���� ��� ����
        Collider[] targets = Physics.OverlapSphere(tf_TopGun.position, range, layerMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Transform targetTf = targets[i].transform;

            if(targetTf.name == "Player")
            {
                // �ڱ�� ������ ����
                Vector3 direction = (targetTf.position - tf_TopGun.position).normalized;
                float angle = Vector3.Angle(direction, tf_TopGun.forward);

                if (angle < viewAngle * 0.5f)
                {
                    tf_Target = targetTf;
                    isFindTarget = true;//Ÿ�� ã��

                    if (angle < 5f)
                    {
                        //�߻� ����
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
            //�ڿ������� �ٶ󺸵��� ��
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

                //�Ѿ��� ����
                if (Physics.Raycast(tf_TopGun.position,
                        tf_TopGun.forward + new Vector3(Random.Range(-1, 1f) * Random.Range(-1, 1f) * rateOfAccuracy, 0f),
                        out hitInfo, range, layerMask))
                        {
                            //���� �Ÿ��� ����
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
