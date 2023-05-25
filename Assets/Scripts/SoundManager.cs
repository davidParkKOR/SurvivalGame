using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name; //곡 이름
    public AudioClip clip; // 곡
}

public class SoundManager : MonoBehaviour
{
    //싱글톤 생성
    static public SoundManager instance;

    #region ___Singleton___

    //# Awake : 객체 생성시 최초 실행
    //# Start : 매번 활성화 되면 실행 (코루틴 실행 불가)
    //# OnEnable : 매번 활성화 되면 실행 (코루틴 실행 가능)
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    #endregion


    public string[] playSoundName;

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBGM;
    public Sound[] effectSounds;
    public Sound[] bgmSound;


    void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
    }

    public void PlaySE(string _name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (effectSounds[i].name == _name)
            {
                for (int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                return;
            }           

        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
    }

   public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                break;
            }
        }
        Debug.Log("재생중인" + _name + "사운드가 없습니다.");
    }

}
