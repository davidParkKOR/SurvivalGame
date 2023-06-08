using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//키트 생성을 위한 클래스

[System.Serializable]
public class Kit
{
    public string kitName;
    public string kitDescription;
    public string[] needItemName;
    public int[] needItemNumber;
    public GameObject go_Kit_Prefab;
}

public class ComputerKit : MonoBehaviour
{
    public bool isPowerOn = false;

    [SerializeField] Kit[] kits;
    [SerializeField] Transform tf_ItemAppearPos; //생성된 아이템 위치
    [SerializeField] ComputerToolTip theToolTip;
    [SerializeField] GameObject go_BaseUI;

    bool isCraft = false;//현재 생성중인지
    
    Inventory theInventory;
    AudioSource theAudio;
    [SerializeField] AudioClip sound_ButtonClick;
    [SerializeField] AudioClip sound_Beep;
    [SerializeField] AudioClip sound_Activated; //장치가동
    [SerializeField] AudioClip sound_output; //아이템 나올떄 나는 소리



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        theInventory = FindObjectOfType<Inventory>();
        theAudio = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (isPowerOn)
            if (Input.GetKeyDown(KeyCode.Escape))
                PowerOff();
        
    }
    public void PowerOn()
    {
        //마우스 보이게 함
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPowerOn = true;
        go_BaseUI.SetActive(true);
    }

    public void PowerOff()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPowerOn = false;
        theToolTip.HideToolTip();
        go_BaseUI.SetActive(false);

    }


    public void ShowToolTip(int _buttonNum)
    {
        theToolTip.ShowToolTip(kits[_buttonNum].kitName, kits[_buttonNum].kitDescription, kits[_buttonNum].needItemName, kits[_buttonNum].needItemNumber);
    }

    public void HideToolTip()
    {
        theToolTip.HideToolTip();
    }



    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    //버튼 클릭시 
    public void ClickButton(int _slotNumber)
    {
        PlaySE(sound_ButtonClick);

        if (!isCraft)
        {
            //재료가 있는지 확인
            if (!CheckIngredient(_slotNumber)) //재료 체크
                return;

            isCraft = true;

            UseIngredient(_slotNumber); //재료 사용
            StartCoroutine(CraftCoroutine(_slotNumber)); //키트 생성
        }
    }

    IEnumerator CraftCoroutine(int _slotNumber)
    {
        PlaySE(sound_Activated);
        //3초 후 아이템 생성
        yield return new WaitForSeconds(3f);
        PlaySE(sound_output);
        Instantiate(kits[_slotNumber].go_Kit_Prefab, tf_ItemAppearPos.position, Quaternion.identity);
        isCraft = false;

 
    }

    /// <summary>
    /// 아이템 비교
    /// </summary>
    /// <param name="_slotNumber"></param>
    bool CheckIngredient(int _slotNumber)
    {
        Debug.Log("SlotNumber : " + _slotNumber);
        for (int i = 0; i < kits[_slotNumber].needItemNumber.Length; i++)
        {
            //현재 인벤토리의 아이템 갯수와 필요갯수 비교
            Debug.Log("InventoryGetItemCount : " + theInventory.GetItemCount(kits[_slotNumber].needItemName[i]));
            Debug.Log("Kit Need Item Count : " + kits[_slotNumber].needItemNumber[i]);
            if (theInventory.GetItemCount(kits[_slotNumber].needItemName[i]) < kits[_slotNumber].needItemNumber[i])
            {
                PlaySE(sound_Beep);
                return false;
            }
              
        }
        return true;
    }


    /// <summary>
    /// 아이템 사용
    /// </summary>
    /// <param name="_slotNumber"></param>
    void UseIngredient(int _slotNumber)
    {
        for (int i = 0; i < kits[_slotNumber].needItemNumber.Length; i++)
        {
            theInventory.SetItemCount(kits[_slotNumber].needItemName[i], kits[_slotNumber].needItemNumber[i]);
        }
    }


}

