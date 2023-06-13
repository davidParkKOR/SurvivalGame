using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public string sceneName = "GameStage";

    public static Title instance;

    SaveNLoad theSaveNLoad;

    private void Awake()
    {     
        if (instance == null)
        { 
            instance = this;
            DontDestroyOnLoad(gameObject);
        }    
        else
        {
            Destroy(this.gameObject);
        }
           
    }

    public void ClickStart()
    {
        SceneManager.LoadScene(sceneName);
        Destroy(gameObject);
    }


    public void ClickLoad()
    {
        Debug.Log("로드");

        StartCoroutine(LoadCoroutine());

    }

    public void ClickExit()
    {
        Debug.Log("게임종료 ");
        Application.Quit();

    }

    IEnumerator LoadCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }

        theSaveNLoad = FindObjectOfType<SaveNLoad>();
        theSaveNLoad.LoadData();
        Destroy(gameObject);

        //while (operation.progress)
        //{
        //    theSaveNLoad.LoadData();
        //}

    }

}
