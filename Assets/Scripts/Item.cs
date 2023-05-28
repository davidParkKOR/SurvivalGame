using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject
{

    public string itemName; //아이템 이름
    [TextArea]
    public string itemDesc; //아이템 설명
    public ItemType itemType; //아이템 유형
    public Sprite itemImage; //아이템 이미지 (캔버스 필요 없이 사용가능)
    public GameObject itemPrefab; // 아이템 프리팹
    public string WeaponType; //무기 유형

    public enum ItemType
    {
        EQUIPMENT,
        USED,
        INGREDIENT,
        ETC
    }



}

