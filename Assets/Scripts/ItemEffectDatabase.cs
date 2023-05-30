using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class ItemEffect
{
    public string itemName; //������ �̸� (Ű��)
    [Tooltip("HP, SP, DP, HUNGRY, THIRSTY �� �����մϴ�.")]
    public string[] part; //��� ������ ȸ����ų��
    public int[] num; //��ġ (����ϰ�� ���ָ� 10, ��������� ��� ���ָ� 10, ü�� -10) �̷���

}
public class ItemEffectDatabase : MonoBehaviour
{
    [SerializeField] private ItemEffect[] itemEffects;
    [SerializeField] private StatusController thePlayerStatus;
    [SerializeField] private WeaponManager theWeaponManager;
    [SerializeField] private SlotToolTip theSlotToolTip;
    [SerializeField] private QuickSlotController theQuickSlotController;

    private const string HP = "HP", SP = "SP", DP = "DP", HUNGRY = "HUNGRY", THIRSTY = "THIRSTY", SATISFY = "SATISFY";


    //QuickSlotController ¡�˴ٸ�
    public void IsActivatedQuickSlot(int _num)
    {
        theQuickSlotController.IsActivatedQuickSlot(_num);
    }

    //SlotTolltip¡�˴ٸ�
    public void ShowToolTip(Item _item, Vector3 _position)
    {
        theSlotToolTip.ShowToolTip(_item, _position);
    }

    public void HideToolTip()
    {
        theSlotToolTip.HideToolTip();
    }

    public void UseItem(Item _item)
    {
        if (_item.itemType == Item.ItemType.EQUIPMENT)
        {
            //���� ����
            StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(_item.WeaponType, _item.itemName));
        }

        else if (_item.itemType == Item.ItemType.USED)
        {
            for (int x = 0; x < itemEffects.Length; x++)
            {
                if (itemEffects[x].itemName == _item.itemName)
                {
                    for (int y = 0; y < itemEffects[x].part.Length; y++)
                    {
                        switch (itemEffects[x].part[y])
                        {
                            case HP:
                                thePlayerStatus.IncreaseHP(itemEffects[x].num[y]);
                                 break;
                            case SP:
                                thePlayerStatus.IncreaseSP(itemEffects[x].num[y]);
                                break;
                            case DP:
                                thePlayerStatus.IncreaseDP(itemEffects[x].num[y]);
                                break;
                            case HUNGRY:
                                thePlayerStatus.IncreaseHungry(itemEffects[x].num[y]);
                                break;
                            case THIRSTY:
                                thePlayerStatus.IncreaseThirsty(itemEffects[x].num[y]);
                                break;
                            case SATISFY:
                                break;

                            default:
                                Debug.Log("�߸��� Status ���� HP, SP, DP, HUNGRY, THIRSTY �� �����մϴ�.");
                                break;
                        }

                        Debug.Log(_item.itemName + " �� ����߽��ϴ�");


                    }
                    return;
                }
            }
            Debug.Log("ItemEffectDatabase�� ��ġ�ϴ� ItemName�� �����ϴ�.");
        }
    }

}
