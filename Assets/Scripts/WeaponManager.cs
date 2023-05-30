using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이게 있어야 실행되도록 함
//[RequireComponent(typeof(GunController)] 
public class WeaponManager : MonoBehaviour
{
    //무기 중복 교체 실행 방지
    public static bool isChangeWeapon;

    //현재 무기, 현재무기 애니메이션
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    //현재 무기 타입
    [SerializeField]
    private string currentWeaponType;



    //무기 교체 딜레이, 무기 교체가 완전히 끝난 시점
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    //무기 종류 전부 관리
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private CloseWeapon[] hans;
    [SerializeField]
    private CloseWeapon[] axes;
    [SerializeField]
    private CloseWeapon[] pickaxes;

    //관리차원에서 쉽게 무기 접근이 가능하도록 만듦.
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> pickaxeDictionary = new Dictionary<string, CloseWeapon>();


    //필요 컴포넌트
    [SerializeField]
    private GunController theGunController;
    [SerializeField]
    private HandController theHandController;
    [SerializeField]
    private AxeController theAxeController;
    [SerializeField]
    private PickaxeController thePickaxeController;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }

        for (int i = 0; i < hans.Length; i++)
        {
            handDictionary.Add(hans[i].closeWeaponName, hans[i]);
        }


        for (int i = 0; i < axes.Length; i++)
        {
            axeDictionary.Add(axes[i].closeWeaponName, axes[i]);
        }

        for (int i = 0; i < pickaxes.Length; i++)
        {
            pickaxeDictionary.Add(pickaxes[i].closeWeaponName, pickaxes[i]);
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(!isChangeWeapon)
    //    {
    //        if(Input.GetKeyDown(KeyCode.Alpha1))
    //        {
    //            //무기 교체 실행(맨손)
    //            StartCoroutine(ChangeWeaponCoroutine("HAND", "맨손"));
    //        }
    //        else if(Input.GetKeyDown(KeyCode.Alpha2))
    //        {
    //            //무기교체 실행(서브머신)
    //            StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun1"));
    //        }
    //        else if (Input.GetKeyDown(KeyCode.Alpha3))
    //        {
    //            //무기교체 실행(Axe)
    //            StartCoroutine(ChangeWeaponCoroutine("AXE", "Axe"));
    //        }
    //        else if (Input.GetKeyDown(KeyCode.Alpha4))
    //        {
    //            //무기교체 실행(Axe)
    //            StartCoroutine(ChangeWeaponCoroutine("PICKAXE", "Pickaxe"));
    //        }
    //    }
    //}

    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");
        yield return new WaitForSeconds(changeWeaponDelayTime);

        //정조준 상태 해제 
        CancelPreWeaponAction();

        //무기 교체
        WaeponChange(_type, _name);
        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        currentWeaponType = _type;
        isChangeWeapon = false;

    }

    void CancelPreWeaponAction()
    { 
        switch(currentWeaponType)
        {
            case "GUN":
                theGunController.CancleFineSight();
                theGunController.CancelReload();
                GunController.isActivate = false;
                break;
            case "HAND":
                if (QuickSlotController.go_HandItem != null) 
                    Destroy(QuickSlotController.go_HandItem);
                HandController.isActivate = false;
                break;
            case "AXE":
                AxeController.isActivate = false;
                break;
            case "PICKAXE":
                PickaxeController.isActivate = false;
                break;

        }    
    }

    void WaeponChange(string _type, string _name)
    {
        switch (_type)
        {
            case "GUN":
                theGunController.GunChange(gunDictionary[_name]);
                break;
            case "HAND":
                theHandController.CloseWeaponChange(handDictionary[_name]);
                break;
            case "AXE":
                theAxeController.CloseWeaponChange(axeDictionary[_name]);
                break;
            case "PICKAXE":
                thePickaxeController.CloseWeaponChange(pickaxeDictionary[_name]);
                break;

        }
    }
}
