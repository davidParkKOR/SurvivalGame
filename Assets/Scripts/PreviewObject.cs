using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public Building.Type needType;
    private bool needTypeFlag;

    private const int IGNORE_RAYCAST_LAYER = 2; // �浹�ص� �ݶ��̴� ����Ʈ�� ���� �ʱ� ����

    [SerializeField] private int layerGround;//�����̾�(�����ϵ�����)
    [SerializeField] private Material green;
    [SerializeField] private Material red;

    private List<Collider> colliderList = new List<Collider>(); //�浹�� ������Ʈ �ݶ��̴�


    private void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (needType == Building.Type.NORMAL)
        {
            if (colliderList.Count > 0)
                //����� ���� 
                SetColor(red);
            else
                //�ʷ����� ����
                SetColor(green);
        }
        else
        {
            if (colliderList.Count > 0 || !needTypeFlag)
                //����� ���� 
                SetColor(red);
            else
                //�ʷ����� ����
                SetColor(green);
        }
    }


    //��� �ڽ� ��ü�� �÷� ����
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
