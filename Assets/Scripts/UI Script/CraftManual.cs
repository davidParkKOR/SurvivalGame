using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Craft
{
    public string craftName;
    public Sprite craftImage;//�̹���
    public string craftDesc;
    public string[] craftNeedItem;//���۽� �ʿ��� ������
    public int[] craftNeedItemCount;//���۽� �ʿ��� ������ ����
    public GameObject go_Prefab; //������ġ�� ������
    public GameObject go_PreviewPrefab; //�̸����� ������
}

public class CraftManual : MonoBehaviour
{
    //���º���
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField] private GameObject go_BaseUI; //�⺻ ���̽� UI
    
    private int tabNumber = 0;
    private int page = 1;
    private int selectedSlotNumber;
    private Craft[] craft_selectedTab;

    [SerializeField] private Craft[] craft_fire; //��ںҿ� ��
    [SerializeField] private Craft[] craft_build; //����� ��

    private GameObject go_Preview; //�̸����� �������� ���� ����
    private GameObject go_Prefab; //���� ��ں�
    
    [SerializeField]private Transform tf_Player;//�÷��̾� ��ġ ��������


    //Raycast �ʿ� ���� ����
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;

    //�ʿ��� UI ���� ���
    [SerializeField] private GameObject[] go_Slots;
    [SerializeField] private Image[] image_Slot;
    [SerializeField] private Text[] text_slotName;
    [SerializeField] private Text[] text_slotDesc;
    [SerializeField] private Text[] text_slotNeedItem;

    //�ʿ� ������Ʈ
    private Inventory theInventory;

    private void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
        tabNumber = 0;
        page = 1;
        //�������� �ʱ�ȭ
        TabSlotSetting(craft_fire);

    }

    //�������� ���� �ʱ�ȭ �ϱ� ����
    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        page = 1;

        switch(tabNumber)
        {
            case 0: //�� ����
                TabSlotSetting(craft_fire);
                break;
            case 1: //���� ����
                TabSlotSetting(craft_build);
                break;
        }
    }

    //���� ������ �ѱ��
    public void RightPageSetting()
    {
        if (page < (craft_selectedTab.Length / go_Slots.Length) + 1)
            page++;
        else
            page = 1;

        TabSlotSetting(craft_selectedTab);
    }

    //���� ������ �ѱ��
    public void LeftPageSetting()
    {
        if (page != 1)
            page--;
        else
            page = (craft_selectedTab.Length / go_Slots.Length) + 1;

        TabSlotSetting(craft_selectedTab);

    }



    public void TabSlotSetting(Craft[] _craftTab)
    {
        ClearSlot();

        craft_selectedTab = _craftTab;

        int startSlotNumber = (page - 1) * go_Slots.Length;

        for (int i = startSlotNumber; i < craft_selectedTab.Length; i++)
        {
            if (i == page * go_Slots.Length)
                break;

            go_Slots[i - startSlotNumber].SetActive(true);

            image_Slot[i - startSlotNumber].sprite = _craftTab[i].craftImage;
            text_slotName[i - startSlotNumber].text = craft_selectedTab[i].craftName;
            text_slotDesc[i - startSlotNumber].text = craft_selectedTab[i].craftDesc;

            for (int j = 0; j < craft_selectedTab[i].craftNeedItem.Length; j++)
            {
                text_slotNeedItem[i - startSlotNumber].text += craft_selectedTab[i].craftNeedItem[j];
                text_slotNeedItem[i - startSlotNumber].text += " x " + craft_selectedTab[i].craftNeedItemCount[j] + "\n";
            }
        }
    }

    //���� �ʱ�ȭ
    private void ClearSlot()
    {
        //�� 4���� ����
        for (int i = 0; i < go_Slots.Length; i++)
        {
            image_Slot[i].sprite = null;
            text_slotName[i].text = null;
            text_slotDesc[i].text = null;
            text_slotNeedItem[i].text = null;
            go_Slots[i].SetActive(false);
        }
    }

    public void SlotClick(int _slotNumber)
    {
        //1�������� 0123, 2�������� 4567 �̷��� ��
        selectedSlotNumber = _slotNumber + (page - 1) * go_Slots.Length;

        if (!CheckIngredient())
            return;

        //�̸����� ����
        go_Preview = Instantiate(craft_selectedTab[selectedSlotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_selectedTab[selectedSlotNumber].go_Prefab;
        isPreviewActivated = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        go_BaseUI.SetActive(false);
    }

    bool CheckIngredient()
    {
        //�κ��丮�� ������ Ȯ��
        for (int i = 0; i < craft_selectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            if (theInventory.GetItemCount(craft_selectedTab[selectedSlotNumber].craftNeedItem[i]) < craft_selectedTab[selectedSlotNumber].craftNeedItemCount[i])
                return false;
        }

        return true;
    }

    void UseIngredient()
    {
        //�κ��丮�� ������ Ȯ��
        for (int i = 0; i < craft_selectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            theInventory.SetItemCount(craft_selectedTab[selectedSlotNumber].craftNeedItem[i], craft_selectedTab[selectedSlotNumber].craftNeedItemCount[i]);
        }
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
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isbuildable())
        {
            UseIngredient();
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
        GameManager.isOpenCraftManual = true;
        isActivated = true;
        go_BaseUI.SetActive(true);
    }

    void CloseWindow()
    {
        GameManager.isOpenCraftManual = false;
        isActivated = false;
        go_BaseUI.SetActive(false);
    }
}
