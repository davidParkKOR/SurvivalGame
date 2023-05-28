using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{

    public string itemName; //������ �̸�
    [TextArea]
    public string itemDesc; //������ ����
    public ItemType itemType; //������ ����
    public Sprite itemImage; //������ �̹��� (ĵ���� �ʿ� ���� ��밡��)
    public GameObject itemPrefab; // ������ ������
    public string WeaponType; //���� ����

    public enum ItemType
    {
        EQUIPMENT,
        USED,
        INGREDIENT,
        ETC
    }



}

