using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public Vector3 playerRot;

    public List<int> invenArrayNumber = new List<int>();
    public List<string> invenItemName = new List<string>();
    public List<int> invenItemNumber = new List<int>(); 
}

public class SaveNLoad : MonoBehaviour
{
    string SAVE_FILENAME = "/SaveFile.txt";
    string SAVE_DATA_DIRECTORY;

    SaveData saveData = new SaveData();


    //�÷��̾� ��ġ
    PlayerController thePlayer;
    Inventory theInven;


    private void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        if(!Directory.Exists(SAVE_DATA_DIRECTORY))
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);

    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void SaveData()
    {
        thePlayer = FindObjectOfType<PlayerController>();
        theInven = FindObjectOfType<Inventory>();  


        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;

        Slot[] slots = theInven.GetSlots();

        Debug.Log(slots.Length);


        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                Debug.Log(slots[i].item.itemName);
                Debug.Log(slots[i].itemCount);
                saveData.invenArrayNumber.Add(i);
                saveData.invenItemName.Add(slots[i].item.itemName);
                saveData.invenItemNumber.Add(slots[i].itemCount);
            }

        }

        string json =  JsonUtility.ToJson(saveData);
        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);
        Debug.Log("���� �Ϸ�");
        Debug.Log(json);
    }

    /// <summary>
    /// ���� �ε�
    /// </summary>
    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            thePlayer = FindObjectOfType<PlayerController>();
            theInven = FindObjectOfType<Inventory>();

            thePlayer.transform.position = saveData.playerPos;
            thePlayer.transform.eulerAngles = saveData.playerRot;


            Debug.Log(saveData.invenItemNumber.Count);
            

            for (int i = 0; i < saveData.invenItemNumber.Count; i++)
            {
                Debug.Log(saveData.invenArrayNumber[i]);
                Debug.Log(saveData.invenItemName[i]);
                Debug.Log(saveData.invenItemNumber[i]);


                theInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemName[i], saveData.invenItemNumber[i]);
            }


            Debug.Log("�ε� �Ϸ�");
        }
        else
            Debug.Log("Save ������ ��������");

    }
}
