using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField] private Slot[] quickSlots; // �����Ե�
    [SerializeField] private Image[] img_coolTime; //������ ��Ÿ��
    [SerializeField] private Transform tf_parent; //�������� �θ� ��ü
    [SerializeField] private Transform tf_itemPos;//�������� ��ġ�� �� ��
    public static GameObject go_HandItem; //�տ� �� ������

    private int selectedSlot; //���õ� �W ���� (0~7) 8��

    [SerializeField] GameObject go_SelectedImage; //���õ� �������� �̹��� 
    [SerializeField] WeaponManager theWeaponManager;

    //�ִϸ��̼�
    private Animator anim;
    //�ִϸ��̼� ���� ����
    [SerializeField] private float apperTime;
    private float currentApperTime;
    private bool isApper;

    //��Ÿ�� ����
    [SerializeField] private float coolTime;
    private float currentCoolTime;
    private bool isCoolTime;

    private void Start()
    {
        quickSlots = tf_parent.GetComponentsInChildren<Slot>();
        selectedSlot = 0;
        anim = GetComponent<Animator>();    
    }

    private void Update()
    {
        TryInputNumber();
        CoolTimeCalc();
        ApperCalc();
    }

    private void AppearReset()
    {
        currentApperTime = apperTime; ;
        isApper = true;
        anim.SetBool("Appear", isApper);
    }

    void ApperCalc()
    {
        if(Inventory.inventoryActivated)
        {
            AppearReset();
        }
        else
        {
            if (isApper)
            {
                currentApperTime -= Time.deltaTime; //1�ʿ� 1 ����

                if (currentApperTime <= 0)
                {
                    isApper = false;
                    anim.SetBool("Appear", isApper);
                }
            }
        }

    }

    void CoolTimeCalc()
    {
        if(isCoolTime)
        {
            currentCoolTime -= Time.deltaTime;

            for (int i = 0; i < img_coolTime.Length; i++)
            {
                img_coolTime[i].fillAmount = currentCoolTime / coolTime;
            }
     

            if (currentCoolTime <= 0)
                isCoolTime = false;
        }
    }

    public void IsActivatedQuickSlot(int _num)
    {
        if(selectedSlot == _num)
        {
            Excute();
            return;

        }
        if(DragSlot.instance != null)
        {
            if(DragSlot.instance.dragSlot != null)
            {
                if (DragSlot.instance.dragSlot.GetQuickSlotNumber() == selectedSlot)
                {
                    Excute();
                    return;
                }
            }

        }


            
    }

    void TryInputNumber()
    {
        if(!isCoolTime)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeSlot(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeSlot(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeSlot(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ChangeSlot(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ChangeSlot(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ChangeSlot(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ChangeSlot(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                ChangeSlot(7);
            }
        }

    }

    void ChangeSlot(int _num)
    {
        SelectedSlot(_num);
        Excute();
    }

    void SelectedSlot(int _num)
    {
        selectedSlot = _num; //���õ� ����
        go_SelectedImage.transform.position = quickSlots[selectedSlot].transform.position; //���õ� �������� �̹��� �̵�
    }

    void CoolTimeReset()
    {
        currentCoolTime = coolTime;
        isCoolTime = true;
    }

    void Excute()
    {
        CoolTimeReset();
        AppearReset();

        if (quickSlots[selectedSlot].item != null)
        {
            if (quickSlots[selectedSlot].item.itemType == Item.ItemType.EQUIPMENT)
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(quickSlots[selectedSlot].item.WeaponType, quickSlots[selectedSlot].item.itemName));
            else if (quickSlots[selectedSlot].item.itemType == Item.ItemType.USED)
                Changehand(quickSlots[selectedSlot].item);
            else
                Changehand();
        }
        else
        {
            Changehand();
        }

    }

    void Changehand(Item _item =null)
    {
        StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "�Ǽ�"));

        if(_item != null)
        {
            StartCoroutine(HandItemCoroutine());
        }
    }

    IEnumerator HandItemCoroutine()
    {
        HandController.isActivate = false;
        //isActivate��  true�ϋ����� ��ٸ� (���ⱳü�� ������ ����)
        yield return new WaitUntil(() => HandController.isActivate); 
        go_HandItem = Instantiate(quickSlots[selectedSlot].item.itemPrefab, tf_itemPos.position, tf_itemPos.rotation);
        go_HandItem.GetComponent<Rigidbody>().isKinematic = true;//�߷¿� ���� �Ȥ�������
        go_HandItem.GetComponent<BoxCollider>().enabled = false; //�÷��̾�� �浹 ���ϰ� ��
        go_HandItem.tag = "Untagged";
        go_HandItem.layer = 7; //Weapon
        go_HandItem.transform.SetParent(tf_itemPos);

    }


    public void DecreaseSelectedItem()
    {
        CoolTimeReset();
        AppearReset();

        //������ ������ ���ÿ� ���� -1
        quickSlots[selectedSlot].SetSlotCount(-1);

        if (quickSlots[selectedSlot].itemCount <= 0)
            Destroy(go_HandItem);
    }


    public bool GetIsCoolTime()
    {
        return isCoolTime;
    }

    public Slot GetSelectedSlot()
    {
        return quickSlots[selectedSlot];
    }
}
