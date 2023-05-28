using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{

    [SerializeField]
    private int hp; //Ǯ ü��

    [SerializeField]
    private float destroyTime; //����Ʈ ���� ��
    //���߷� ���� 
    [SerializeField]
    private float force;
    
    
    [SerializeField]
    private GameObject go_hit_effect_prefab; //Ÿ�� ȿ��
    [SerializeField]
    private Item item_leaf;
    [SerializeField]
    private int leafCount;
    private Inventory theInventory;


    private Rigidbody[] rigidbodys;
    private BoxCollider[] boxColliders;

    [SerializeField]
    private string hit_Sound;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodys =  this.transform.GetComponentsInChildren<Rigidbody>();
        boxColliders = transform.GetComponentsInChildren<BoxCollider>();
        theInventory = FindObjectOfType<Inventory>();
    }

    public void Damage()
    {
        hp--;
        Hit();

        if(hp <= 0)
        {
            //�ı�
            Destruction();
        }
    }

    void Hit()
    {
        SoundManager.instance.PlaySE(hit_Sound);

        var clone = Instantiate(go_hit_effect_prefab, transform.position + Vector3.up, Quaternion.identity);

        Destroy(clone, destroyTime);
    }


    void Destruction()
    {
        theInventory.AcquireItem(item_leaf, leafCount);
        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].useGravity = true;
            //������ �� �ϰ� ���󰡴� ȿ��
            rigidbodys[i].AddExplosionForce(force, transform.position, 1f);
            boxColliders[i].enabled = true;
        }

        Destroy(this.gameObject, destroyTime);
    }
}
