using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;


[System.Serializable]
public class Craft
{
    public string craftName;
    public GameObject go_Prefab; //������ġ�� ������
    public GameObject go_PreviewPrefab; //�̸����� ������
}

public class CraftManual : MonoBehaviour
{
    //���º���
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField] private GameObject go_BaseUI; //�⺻ ���̽� UI
    [SerializeField] private Craft[] craft_fire; //��ںҿ� ��

    private GameObject go_Preview; //�̸����� �������� ���� ����
    private GameObject go_Prefab; //���� ��ں�
    
    [SerializeField]private Transform tf_Player;//�÷��̾� ��ġ ��������


    //Raycast �ʿ� ���� ����
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;

    public void SlotClick(int _slotNumber)
    {
        //�̸����� ����
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

                //���콺�� ��ġ�ϴ°��� ��ġ�ϵ��� ��
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
