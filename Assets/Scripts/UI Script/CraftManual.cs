using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;


[System.Serializable]
public class Craft
{
    public string craftName;
    public GameObject go_Prefab; //실제설치될 프리팹
    public GameObject go_PreviewPrefab; //미리보기 프리팹
}

public class CraftManual : MonoBehaviour
{
    //상태변수
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField] private GameObject go_BaseUI; //기본 베이스 UI
    [SerializeField] private Craft[] craft_fire; //모닥불용 탭

    private GameObject go_Preview; //미리보기 프리팹을 담을 변수
    private GameObject go_Prefab; //실제 모닥불
    
    [SerializeField]private Transform tf_Player;//플레이어 위치 가져오기


    //Raycast 필요 변ㅅ 선언
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;

    public void SlotClick(int _slotNumber)
    {
        //미리보기 생성
        go_Preview = Instantiate(craft_fire[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_fire[_slotNumber].go_Prefab;
        isPreviewActivated = true;
        go_BaseUI.SetActive(false);
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
            Window();

        if (isPreviewActivated)
            PreviewPositionUpdate();

        if (Input.GetButtonDown("Fire1"))
            Build();

        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();


    }

    void Build()
    {
        if(isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isbuildable())
        {
            Instantiate(go_Prefab, go_Preview.transform.position, go_Preview.transform.rotation);
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }

    void PreviewPositionUpdate()
    {
        if (Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if(hitInfo.transform != null)
            {         
                Vector3 location = hitInfo.point;     
                
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    go_Preview.transform.Rotate(0, -90f, 0f);
                }
                else if(Input.GetKeyDown(KeyCode.E))
                {
                    go_Preview.transform.Rotate(0, +90f, 0f);
                }

                location.Set(Mathf.Round(location.x), Mathf.Round(location.y / 0.1f) * 0.1f, Mathf.Round(location.z));

                //마우스가 위치하는곳에 위치하도록 함
                go_Preview.transform.position = location;
            }
        }
    
    }


    void Cancel()
    {
        if(isPreviewActivated)
        Destroy(go_Preview);

        isActivated = false;
        isPreviewActivated = false;
        go_Preview = null;
        go_Prefab = null;
        go_BaseUI.SetActive(false);
    }

    void Window()
    {
        if (!isActivated)
            OpenWindow();
        else
            CloseWindow();
    }

    void OpenWindow()
    {
        isActivated = true;
        go_BaseUI.SetActive(true);
    }

    void CloseWindow()
    {
        isActivated = false;
        go_BaseUI.SetActive(false);
    }
}
