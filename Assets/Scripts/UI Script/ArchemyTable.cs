using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ArchemyItem
{
    public string itemName;
    public string itemDesc;
    public Sprite itemImage;
    public string[] needItemName;
    public int[] needItemNumber;
    public float itemCraftingTime;//포션 제조에 걸리는 시간(5,10,100)
    public GameObject go_ItemPrefab;

}


public class ArchemyTable : MonoBehaviour
{

    public bool GetIsOpen()
    {
        return isOpen;
    }

    bool isOpen = false;
    bool isCrafting = false; //아이템 제작 시작 여부

    Queue<ArchemyItem> archemyItemQueue= new Queue<ArchemyItem>(); //연금 아이템 제작 대기열
    ArchemyItem currentCraftingItem;//현재 제작중인 연금 아이템

    float craftingTime;//포션 제작 시간
    float currentCraftingTime; //실제 계산
    int page = 1;// 테이블의 페이지

    [SerializeField] int theNumberOfSlot; //한 페이지당 슬롯의 최대 갯수 (4개)
    [SerializeField] Image[] image_ArchemyItems; //페이지에 따른 포션 이미지들
    [SerializeField] Text[] text_ArchemyItems; //포션에 따른 알케미 텍스트
    [SerializeField] Button[] button_ArchemyItems; //버튼 
    [SerializeField] Slider slider_Gauge; //슬라이더 게이지
    [SerializeField] ArchemyItem[] archemyItems; //제작할 수 있는 연금 아이템 리스트
    [SerializeField] Transform tf_BaseUI; //베이스 UI
    [SerializeField] Transform tf_PostionAppearPos;
    [SerializeField] GameObject go_Liquid; //연금술 동작시키면 액체 등장
    [SerializeField] Image[] image_CraftingItems;//대기열 슬롯의 아이템 이미지


    //필요 컴포넌트
    [SerializeField] ArchemyTooltip theToolTip;
    private Inventory theInven;
    private AudioSource theAudio;
    [SerializeField] private AudioClip sound_ButtonClick;
    [SerializeField] private AudioClip sound_Beep;
    [SerializeField] private AudioClip sound_Activate;
    [SerializeField] private AudioClip sound_ExitItem;



    private void Start()
    {
        theInven = FindObjectOfType<Inventory>();
        theAudio = GetComponent<AudioSource>();
        ClearSlot();
        PageSetting();
    }

    private void Update()
    {
        if(!IsFinish())
        {
            Crafting();
        }

        if(isOpen)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                CloseWindow();
            }
        }
    }

    bool IsFinish()
    { 
        if(archemyItemQueue.Count == 0 && !isCrafting)
        {
            go_Liquid.SetActive(false);
            slider_Gauge.gameObject.SetActive(false);
            return true;
        }
        else
        {
            go_Liquid.SetActive(true);
            slider_Gauge.gameObject.SetActive(true);
            return false;
        }
    
    }

    private void Crafting()
    {
        if(!isCrafting && archemyItemQueue.Count != 0)
        {
            DequeueItem();
        }

        if(isCrafting)
        {
            //슬라이더 게이지 채우고 다 차면 포션 생성
            currentCraftingTime += Time.deltaTime;
            slider_Gauge.value = currentCraftingTime;

            if(currentCraftingTime >= craftingTime)
            {
                ProductionComplete();
            }
        }
    }

    void DequeueItem()
    {
        PlaySE(sound_Activate);

        //제작중
        isCrafting = true;
        currentCraftingItem = archemyItemQueue.Dequeue();
        craftingTime = currentCraftingItem.itemCraftingTime;
        currentCraftingTime = 0;
        slider_Gauge.maxValue = craftingTime;
        CraftingImageChange();
    }

    void CraftingImageChange()
    {
        image_CraftingItems[0].gameObject.SetActive(true);

        //위에서 Dequeue를 했기 떄문에 Count에 1을 더함
        for (int i = 0; i < archemyItemQueue.Count +1; i++)
        {
            image_CraftingItems[i].sprite = image_CraftingItems[i + 1].sprite;

            if (i + 1 == archemyItemQueue.Count + 1)
                image_CraftingItems[i + 1].gameObject.SetActive(false);

        }
    }

    public void Window()
    {
        isOpen = !isOpen;

        if (isOpen)
            OpenWindow();
        else 
            CloseWindow();
    }


    void CloseWindow()
    {
        isOpen = false;
        GameManager.isOpenArchemyTable = false;
        tf_BaseUI.localScale = new Vector3(0f, 0f, 0f);
    }

    void OpenWindow()
    {
        isOpen = true;
        GameManager.isOpenArchemyTable = true;
        tf_BaseUI.localScale = new Vector3(1f, 1f, 1f);
    }

    public void ButtonClick(int _buttonNum)
    {
        PlaySE(sound_ButtonClick);

        if (archemyItemQueue.Count < 3)
        {
            int archemyItemArrayNumber = _buttonNum + ((page - 1) * theNumberOfSlot);


            //인벤토리에서 재료 검색
            for (int i = 0; i < archemyItems[archemyItemArrayNumber].needItemName.Length; i++)
            {
                if (theInven.GetItemCount(archemyItems[archemyItemArrayNumber].needItemName[i]) < 
                    archemyItems[archemyItemArrayNumber].needItemNumber[i])
                {
                    //아이템이 부족
                    PlaySE(sound_Beep);
                    return;
                }
            }

            //인벤토리 재료 감산
            for (int i = 0; i < archemyItems[archemyItemArrayNumber].needItemName.Length; i++)
            {
                theInven.SetItemCount(archemyItems[archemyItemArrayNumber].needItemName[i], archemyItems[archemyItemArrayNumber].needItemNumber[i]);
            }


            archemyItemQueue.Enqueue(archemyItems[archemyItemArrayNumber]);
            image_CraftingItems[archemyItemQueue.Count].gameObject.SetActive(true);
            image_CraftingItems[archemyItemQueue.Count].sprite = archemyItems[archemyItemArrayNumber].itemImage;
        }
        else
        {
            PlaySE(sound_Beep);
        }
    }

    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    void ProductionComplete()
    {
        //제작이 끝남을 알림
        isCrafting = false;
        image_CraftingItems[0].gameObject.SetActive(false);

        PlaySE(sound_ExitItem);

        Instantiate(currentCraftingItem.go_ItemPrefab,
                    tf_PostionAppearPos.position,
                    Quaternion.identity);
    }

    public void UpButton()
    {
        PlaySE(sound_ButtonClick);


        if (page != 1)
            page--;
        else
            page = 1 + (archemyItems.Length / theNumberOfSlot);
        ClearSlot();
        PageSetting();
    }
    public void DownButton()
    {
        PlaySE(sound_ButtonClick);

        if (page < 1 + (archemyItems.Length / theNumberOfSlot))
            page++;
        else
            page = 1;

        ClearSlot();
        PageSetting();
    }

    void ClearSlot()
    {
        for (int i = 0; i < theNumberOfSlot; i++)
        {
            image_ArchemyItems[i].sprite= null;
            image_ArchemyItems[i].gameObject.SetActive(false);
            button_ArchemyItems[i].gameObject.SetActive(false);
            text_ArchemyItems[i].text = "";

        }
    }

    void PageSetting()
    {
        int pageArrayStartNumber = (page - 1) * theNumberOfSlot;

        for (int i = pageArrayStartNumber; i < archemyItems.Length; i++)
        {
            if (i == page * theNumberOfSlot)
                break;

            image_ArchemyItems[i - pageArrayStartNumber].sprite = archemyItems[i].itemImage;
            image_ArchemyItems[i - pageArrayStartNumber].gameObject.SetActive(true);
            button_ArchemyItems[i - pageArrayStartNumber].gameObject.SetActive(true);
            text_ArchemyItems[i - pageArrayStartNumber].text = archemyItems[i].itemName + "\n" + archemyItems[i].itemDesc;

        }
    }


    public void ShowTooltip(int _buttonNum)
    {
        int archemyItemArryNumber = _buttonNum + (page -1) * theNumberOfSlot;
        theToolTip.ShowTooltip(archemyItems[archemyItemArryNumber].needItemName, archemyItems[archemyItemArryNumber].needItemNumber);
    }
    public void HideTooltip()
    {
        theToolTip.HideTooltip();

    }

}
