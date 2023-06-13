using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject go_BaseUI;
    [SerializeField] SaveNLoad theSaveNLoad;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (!GameManager.isPause)
                CallMenu();
            else
                CloseMenu();

        }
    }

    void CallMenu()
    {
        GameManager.isPause = true;
        go_BaseUI.SetActive(true);
        Time.timeScale = 0f; //0������� ����
    }

    void CloseMenu()
    {
        GameManager.isPause = false;
        go_BaseUI.SetActive(false);
        Time.timeScale = 1f; //1������� ����
    }

    public void ClickSave()
    {
        Debug.Log("Save");
        theSaveNLoad.SaveData();
    }

    public void ClickLoad()
    {
        Debug.Log("Load");
        theSaveNLoad.LoadData();

    }
    public void ClickExit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
