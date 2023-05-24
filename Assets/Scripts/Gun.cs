using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;//���� �̸�
    public float range; //�����Ÿ�
    public float accuracy; //��Ȯ��
    public float fireRate; //����ӵ�
    public float reloadTime; //������ �ӵ�

    public int damage; //���� ������
    public int reloadBulletCount;//���� ������ ����.
    public int currentBulletCount;//���� ź������ �����ִ� �Ѿ��� ����
    public int maxBulletCount; //�ִ� ���� ���� �Ѿ� ����
    public int carryBulletCount; //���� ���� �ϰ� �ִ� �Ѿ��� ����

    public float retroActionForce; //�ݵ� ����
    public float retroActionFineSightForce; //�����ؽ� �ݵ����� (�����ʹ�ư ������)

    public Vector3 fineSightOrginPos;
    public Animator anim;
    public ParticleSystem muzzleFlash;// �� �߻�� �ѱ� ����
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
