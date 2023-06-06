using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public Building.Type needType;
    private bool needTypeFlag;

    private const int IGNORE_RAYCAST_LAYER = 2; // 충돌해도 콜라이더 리스트에 담지 않기 위함

    [SerializeField] private int layerGround;//지상레이어(무시하도록함)
    [SerializeField] private Material green;
    [SerializeField] private Material red;

    private List<Collider> colliderList = new List<Collider>(); //충돌한 오브젝트 콜라이더


    private void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (needType == Building.Type.NORMAL)
        {
            if (colliderList.Count > 0)
                //레드로 변경 
                SetColor(red);
            else
                //초록으로 변경
                SetColor(green);
        }
        else
        {
            if (colliderList.Count > 0 || !needTypeFlag)
                //레드로 변경 
                SetColor(red);
            else
                //초록으로 변경
                SetColor(green);
        }
    }


    //모든 자식 객체의 컬러 변경
    void SetColor(Material _mat)
    { 
        foreach(Transform tf_child in this.transform)
        {
            var newMaterials = new Material[tf_child.GetComponent<Renderer>().materials.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = _mat;
            }

            tf_child.GetComponent<Renderer>().materials = newMaterials;
        }    
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Structuere : " + other.transform.tag);

        if(other.transform.tag == "Structure")
        {
            if (other.GetComponent<Building>().type != needType)
                colliderList.Add(other);
            else
                needTypeFlag = true;
        }
        else
        {
            if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
                colliderList.Add(other);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Structure")
        {
            if (other.GetComponent<Building>().type != needType)
                colliderList.Remove(other);
            else
                needTypeFlag = false;
        }
        else
        {
            if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
                colliderList.Remove(other);
        }
    }


    public bool isbuildable()
    {
        if (needType == Building.Type.NORMAL)
            return colliderList.Count == 0;
        else
            return colliderList.Count == 0 && needTypeFlag;
    }


}
