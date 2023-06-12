using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float finishTime;

    bool isHurt = false; 
    bool isActivated = false;//한번만 작동하도록

    public IEnumerator ActivatedTrapCoroutine()
    {
        isActivated = true;
        yield return new WaitForSeconds(finishTime);
        isActivated = false;
        isHurt = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isActivated)
        {
            if (!isHurt)
            {
                isHurt = true;

                if(other.transform.name == "Player")
                {
                    other.transform.GetComponent<StatusController>().DecreaseHP(damage);
                }
            
            }
        }
    }
}
