using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTrap : MonoBehaviour
{
    private Rigidbody[] rigid;
    private bool isActivated = false;
    private AudioSource theAudio;

    [SerializeField] private GameObject go_Meat;
    [SerializeField] private int damage;
   
    [SerializeField] private AudioClip sound_Acitvate;

    private void Start()
    {
        rigid = GetComponentsInChildren<Rigidbody>();
        theAudio= GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isActivated)
        {
            if(other.transform.tag != "Untagged")
            {
                isActivated = true;
                theAudio.clip= sound_Acitvate;
                theAudio.Play();
                Destroy(go_Meat);//고기 제거

                for (int i = 0; i < rigid.Length; i++)
                {
                    rigid[i].useGravity = true;
                    rigid[i].isKinematic = false;

                }

                if(other.transform.name =="Player")
                {
                    other.transform.GetComponent<StatusController>().DecreaseHP(damage);
                }
            }
        }
    }
}
