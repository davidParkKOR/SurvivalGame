using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�̰� �־�� ����ǵ��� ��
//[RequireComponent(typeof(GunController)] 
public class WeaponManager : MonoBehaviour
{
    //���� �ߺ� ��ü ���� ����
    public static bool isChangeWeapon;

    //���� ����, ���繫�� �ִϸ��̼�
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

    //���� ���� Ÿ��
    [SerializeField]
    private string currentWeaponType;



    //���� ��ü ������, ���� ��ü�� ������ ���� ����
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    //���� ���� ���� ����
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private CloseWeapon[] hans;
    [SerializeField]
    private CloseWeapon[] axes;
    [SerializeField]
    private CloseWeapon[] pickaxes;

    //������������ ���� ���� ������ �����ϵ��� ����.
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, CloseWeapon> handDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> axeDictionary = new Dictionary<string, CloseWeapon>();
    private Dictionary<string, CloseWeapon> pickaxeDictionary = new Dictionary<string, CloseWeapon>();


    //�ʿ� ������Ʈ
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
    //            //���� ��ü ����(�Ǽ�)
    //            StartCoroutine(ChangeWeaponCoroutine("HAND", "�Ǽ�"));
    //        }
    //        else if(Input.GetKeyDown(KeyCode.Alpha2))
    //        {
    //            //���ⱳü ����(����ӽ�)
    //            StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun1"));
    //        }
    //        else if (Input.GetKeyDown(KeyCode.Alpha3))
    //        {
    //            //���ⱳü ����(Axe)
    //            StartCoroutine(ChangeWeaponCoroutine("AXE", "Axe"));
    //        }
    //        else if (Input.GetKeyDown(KeyCode.Alpha4))
    //        {
    //            //���ⱳü ����(Axe)
    //            StartCoroutine(ChangeWeaponCoroutine("PICKAXE", "Pickaxe"));
    //        }
    //    }
    //}

    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");
        yield return new WaitForSeconds(changeWeaponDelayTime);

        //������ ���� ���� 
        CancelPreWeaponAction();

        //���� ��ü
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
