using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Craft
{
    public string craftName;
    public Sprite craftImage;//이미지
    public string craftDesc;
    public string[] craftNeedItem;//제작시 필요한 아이템
    public int[] craftNeedItemCount;//제작시 필요한 아이템 갯수
    public GameObject go_Prefab; //실제설치될 프리팹
    public GameObject go_PreviewPrefab; //미리보기 프리팹
}

public class CraftManual : MonoBehaviour
{
    //상태변수
    private bool isActivated = false;
    private bool isPreviewActivated = false;

    [SerializeField] private GameObject go_BaseUI; //기본 베이스 UI
    
    private int tabNumber = 0;
    private int page = 1;
    private int selectedSlotNumber;
    private Craft[] craft_selectedTab;

    [SerializeField] private Craft[] craft_fire; //모닥불용 탭
    [SerializeField] private Craft[] craft_build; //건축용 탭

    private GameObject go_Preview; //미리보기 프리팹을 담을 변수
    private GameObject go_Prefab; //실제 모닥불
    
    [SerializeField]private Transform tf_Player;//플레이어 위치 가져오기


    //Raycast 필요 변ㅅ 선언
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;

    //필요한 UI 슬롯 요소
    [SerializeField] private GameObject[] go_Slots;
    [SerializeField] private Image[] image_Slot;
    [SerializeField] private Text[] text_slotName;
    [SerializeField] private Text[] text_slotDesc;
    [SerializeField] private Text[] text_slotNeedItem;

    //필요 컴포넌트
    private Inventory theInventory;

    private void Start()
    {
        theInventory = FindObjectOfType<Inventory>();
        tabNumber = 0;
        page = 1;
        //불탭으로 초기화
        TabSlotSetting(craft_fire);

    }

    //보여지는 탭을 초기화 하기 위함
    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        page = 1;

        switch(tabNumber)
        {
            case 0: //불 세팅
                TabSlotSetting(craft_fire);
                break;
            case 1: //건축 세팅
                TabSlotSetting(craft_build);
                break;
        }
    }

    //우측 페이지 넘기기
    public void RightPageSetting()
    {
        if (page < (craft_selectedTab.Length / go_Slots.Length) + 1)
            page++;
        else
            page = 1;

        TabSlotSetting(craft_selectedTab);
    }

    //좌측 페이지 넘기기
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

    //슬롯 초기화
    private void ClearSlot()
    {
        //총 4개의 슬롯
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
        //1페이지면 0123, 2페이지면 4567 이렇게 됨
        selectedSlotNumber = _slotNumber + (page - 1) * go_Slots.Length;

        if (!CheckIngredient())
            return;

        //미리보기 생성
        go_Preview = Instantiate(craft_selectedTab[selectedSlotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_selectedTab[selectedSlotNumber].go_Prefab;
        isPreviewActivated = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        go_BaseUI.SetActive(false);
    }

    bool CheckIngredient()
    {
        //인벤토리의 아이템 확인
        for (int i = 0; i < craft_selectedTab[selectedSlotNumber].craftNeedItem.Length; i++)
        {
            if (theInventory.GetItemCount(craft_selectedTab[selectedSlotNumber].craftNeedItem[i]) < craft_selectedTab[selectedSlotNumber].craftNeedItemCount[i])
                return false;
        }

        return true;
    }

    void UseIngredient()
    {
        //인벤토리의 아이템 확인
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
