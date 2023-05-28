using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{

    [SerializeField]
    private int hp; //풀 체력

    [SerializeField]
    private float destroyTime; //이펙트 삭제 시
    //폭발력 세기 
    [SerializeField]
    private float force;
    
    
    [SerializeField]
    private GameObject go_hit_effect_prefab; //타격 효과
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
            //파괴
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
            //떄리고 팍 하고 날라가는 효과
            rigidbodys[i].AddExplosionForce(force, transform.position, 1f);
            boxColliders[i].enabled = true;
        }

        Destroy(this.gameObject, destroyTime);
    }
}
