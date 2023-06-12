using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTrap : MonoBehaviour
{
    private Animator anim;
    private AudioSource theAudio;
    private bool isActivated = false;
    [SerializeField] private AudioClip sound_Activated;
    [SerializeField] private TrapDamage theTrapDamage;

    private void Start()
    {
        anim = GetComponent<Animator>();
        theAudio= GetComponent<AudioSource>();

    }

    public void ReInstall()
    {
        isActivated = false;
        anim.SetTrigger("DeActivate");

    }

    public bool GetIsActivated()
    {
        return isActivated;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isActivated)
        {
            if(other.transform.tag != "Untagged" && other.transform.tag != "Trap")
            {
                StartCoroutine(theTrapDamage.ActivatedTrapCoroutine());
                isActivated = true;
                anim.SetTrigger("Activate");
                theAudio.clip = sound_Activated;
                theAudio.Play();
            }
        }
    }
}
