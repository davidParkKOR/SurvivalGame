using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private int hp; //바위의 체력
    [SerializeField]
    private float destroyTime; // 파편 제거 시간
    [SerializeField]
    private SphereCollider col; //구체 콜라이더

    //필요한 게임 오브젝트
    [SerializeField]
    private GameObject go_rock; // 일반 바위
    [SerializeField]
    private GameObject go_debris; //꺠진 바위
    [SerializeField]
    private GameObject go_effect_prefabs;//채굴 이펙트

    //필요 사운드
    [SerializeField]
    private string strike_Sound;
    [SerializeField]
    private string destroy_Sound;

    //채굴
    public void Mining()
    {
        SoundManager.instance.PlaySE(strike_Sound);
        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);



        hp--;

        if (hp <= 0)
            Destruction();
    }

    //바위 쪼개
    void Destruction()
    {
        SoundManager.instance.PlaySE(destroy_Sound);

        col.enabled = false;
        Destroy(go_rock);

        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);

    }


}
