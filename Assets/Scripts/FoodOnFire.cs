using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOnFire : MonoBehaviour
{
    [SerializeField] private float time; //�����ų� Ÿ�µ� �ɸ��� �ð�
    private float currentTime;

    private bool done; //�������� ���̻� �ҿ� �־ ��� �ȵǵ��� ��.
    [SerializeField] private GameObject go_CookedItemPrefeb; //������, Ȥ�� ź ������ ��ü

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.tag == "Fire" && !done)
        {
            currentTime += Time.deltaTime;

            if(currentTime >= time)
            {
                done = true;
                Instantiate(go_CookedItemPrefeb, transform.position, Quaternion.Euler(transform.eulerAngles));
                Destroy(gameObject);
            }
        }
    }

}
