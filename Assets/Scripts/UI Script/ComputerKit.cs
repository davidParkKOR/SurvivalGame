using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ŰƮ ������ ���� Ŭ����

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
    [SerializeField] Transform tf_ItemAppearPos; //������ ������ ��ġ
    [SerializeField] ComputerToolTip theToolTip;
    [SerializeField] GameObject go_BaseUI;

    bool isCraft = false;//���� ����������
    
    Inventory theInventory;
    AudioSource theAudio;
    [SerializeField] AudioClip sound_ButtonClick;
    [SerializeField] AudioClip sound_Beep;
    [SerializeField] AudioClip sound_Activated; //��ġ����
    [SerializeField] AudioClip sound_output; //������ ���Ë� ���� �Ҹ�



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
        //���콺 ���̰� ��
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

    //��ư Ŭ���� 
    public void ClickButton(int _slotNumber)
    {
        PlaySE(sound_ButtonClick);

        if (!isCraft)
        {
            //��ᰡ �ִ��� Ȯ��
            if (!CheckIngredient(_slotNumber)) //��� üũ
                return;

            isCraft = true;

            UseIngredient(_slotNumber); //��� ���
            StartCoroutine(CraftCoroutine(_slotNumber)); //ŰƮ ����
        }
    }

    IEnumerator CraftCoroutine(int _slotNumber)
    {
        PlaySE(sound_Activated);
        //3�� �� ������ ����
        yield return new WaitForSeconds(3f);
        PlaySE(sound_output);
        Instantiate(kits[_slotNumber].go_Kit_Prefab, tf_ItemAppearPos.position, Quaternion.identity);
        isCraft = false;

 
    }

    /// <summary>
    /// ������ ��
    /// </summary>
    /// <param name="_slotNumber"></param>
    bool CheckIngredient(int _slotNumber)
    {
        Debug.Log("SlotNumber : " + _slotNumber);
        for (int i = 0; i < kits[_slotNumber].needItemNumber.Length; i++)
        {
            //���� �κ��丮�� ������ ������ �ʿ䰹�� ��
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
    /// ������ ���
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

