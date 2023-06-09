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
    public float itemCraftingTime;//���� ������ �ɸ��� �ð�(5,10,100)
    public GameObject go_ItemPrefab;

}


public class ArchemyTable : MonoBehaviour
{

    public bool GetIsOpen()
    {
        return isOpen;
    }

    bool isOpen = false;
    bool isCrafting = false; //������ ���� ���� ����

    Queue<ArchemyItem> archemyItemQueue= new Queue<ArchemyItem>(); //���� ������ ���� ��⿭
    ArchemyItem currentCraftingItem;//���� �������� ���� ������

    float craftingTime;//���� ���� �ð�
    float currentCraftingTime; //���� ���
    int page = 1;// ���̺��� ������

    [SerializeField] int theNumberOfSlot; //�� �������� ������ �ִ� ���� (4��)
    [SerializeField] Image[] image_ArchemyItems; //�������� ���� ���� �̹�����
    [SerializeField] Text[] text_ArchemyItems; //���ǿ� ���� ���ɹ� �ؽ�Ʈ
    [SerializeField] Button[] button_ArchemyItems; //��ư 
    [SerializeField] Slider slider_Gauge; //�����̴� ������
    [SerializeField] ArchemyItem[] archemyItems; //������ �� �ִ� ���� ������ ����Ʈ
    [SerializeField] Transform tf_BaseUI; //���̽� UI
    [SerializeField] Transform tf_PostionAppearPos;
    [SerializeField] GameObject go_Liquid; //���ݼ� ���۽�Ű�� ��ü ����
    [SerializeField] Image[] image_CraftingItems;//��⿭ ������ ������ �̹���



    private void Start()
    {
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
            //�����̴� ������ ä��� �� ���� ���� ����
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
        //������
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

        //������ Dequeue�� �߱� ������ Count�� 1�� ����
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

        if (archemyItemQueue.Count < 3)
        {
            int archemyItemArrayNumber = _buttonNum + ((page - 1) * theNumberOfSlot);


            archemyItemQueue.Enqueue(archemyItems[archemyItemArrayNumber]);
            image_CraftingItems[archemyItemQueue.Count].gameObject.SetActive(true);
            image_CraftingItems[archemyItemQueue.Count].sprite = archemyItems[archemyItemArrayNumber].itemImage;
        }
    }

    void ProductionComplete()
    {
        //������ ������ �˸�
        isCrafting = false;
        image_CraftingItems[0].gameObject.SetActive(false);

        Instantiate(currentCraftingItem.go_ItemPrefab,
                    tf_PostionAppearPos.position,
                    Quaternion.identity);
    }

    public void UpButton()
    {
        if (page != 1)
            page--;
        else
            page = 1 + (archemyItems.Length / theNumberOfSlot);
        ClearSlot();
        PageSetting();
    }
    public void DownButton()
    {
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

}