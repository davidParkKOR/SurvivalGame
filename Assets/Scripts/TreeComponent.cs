using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TreeComponent : MonoBehaviour
{
    //���� ���� ������
    [SerializeField]
    private GameObject[] go_treePieces;
    [SerializeField]
    private GameObject go_treeCenter;

    //�α� ������
    [SerializeField]
    private GameObject go_Log_Prefabs;

    //������ ȿ��
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    //���� ���� �ð�
    [SerializeField]
    private float debrisDestroyTime;

    //���� ���� �ð�
    [SerializeField]
    private float destroyTime;
    //�������� ���� ����
    [SerializeField]
    private float force;
    //�ڽ� Ʈ��
    [SerializeField]
    private GameObject go_ChileTree;

    //�θ� Ʈ�� �ı��Ǹ� ĸ�� �ݶ��̴� ��Ȱ��ȭ
    [SerializeField]
    private CapsuleCollider parentCol;

    //�ڽ� Ʈ�� �������� �ʿ��� ������Ʈ Ȱ��ȭ �� �߷� Ȱ��ȭ
    [SerializeField]
    private CapsuleCollider childCol;
    [SerializeField]
    private Rigidbody childRigid;

    //�ʿ� ����
    [SerializeField]
    private string chop_Sound;
    [SerializeField]
    private string falldown_Sound;
    [SerializeField]
    private string logChange_Sound;


    public void Chop(Vector3 _pos, float _angleY)
    {
        Hit(_pos);

        //�÷��̾ ��� �������� �ߴ��� �Ǻ��Ͽ� ���� ��� 
        AngleCalc(_angleY);

        //Piece���� ���Ҵ��� Ȯ��
        if (CheckTreePieces())
            return;

        //��� ���� �ı��ϰ� ���� ����Ʈ����
        FallDownTree();

    }

    //���� ����Ʈ
    void Hit(Vector3 _pos)
    {
        SoundManager.instance.PlaySE(chop_Sound);

        
        GameObject clone = Instantiate(go_hit_effect_prefab, _pos, Quaternion.Euler(Vector3.zero));
        Destroy(clone, debrisDestroyTime);
        
    }

    void AngleCalc(float _angleY)
    {

        if (0 <= _angleY && _angleY <= 70)
            DestroyPiece(2);
        if (70 <= _angleY && _angleY <= 140)
            DestroyPiece(3);
        if (140 <= _angleY && _angleY <= 210)
            DestroyPiece(4);
        if (210 <= _angleY && _angleY <= 280)
            DestroyPiece(0);
        if (280 <= _angleY && _angleY <= 360)
            DestroyPiece(1);
    }

    void DestroyPiece(int _num)
    {
        if (go_treePieces[_num].gameObject != null)
        {
            GameObject clone = Instantiate(go_hit_effect_prefab, transform.position, Quaternion.Euler(Vector3.zero));
            Destroy(clone, debrisDestroyTime);
            Destroy(go_treePieces[_num].gameObject);
        }
    }

    bool CheckTreePieces()
    {
        for (int i = 0; i < go_treePieces.Length; i++)
        {
            if (go_treePieces[i].gameObject != null)         
                return true;           
        }
        return false;
    }

    void FallDownTree()
    {
        SoundManager.instance.PlaySE(falldown_Sound);
        Destroy(go_treeCenter);

        parentCol.enabled = false;
        childCol.enabled = true;
        childRigid.useGravity = true;

        childRigid.AddForce(Random.Range(-force, force), 0f, Random.Range(-force, force));

        //�α� ����
        StartCoroutine(LogCoroutine());

    }
    IEnumerator LogCoroutine()
    {
        yield return new WaitForSeconds(destroyTime);

        SoundManager.instance.PlaySE(logChange_Sound);

        //������ ������ ��ġ�� �α� ����
        Instantiate(go_Log_Prefabs, go_ChileTree.transform.position + (go_ChileTree.transform.up * 3f), Quaternion.LookRotation(go_ChileTree.transform.up));
        Instantiate(go_Log_Prefabs, go_ChileTree.transform.position + (go_ChileTree.transform.up * 6f), Quaternion.LookRotation(go_ChileTree.transform.up));
        Instantiate(go_Log_Prefabs, go_ChileTree.transform.position + (go_ChileTree.transform.up * 9f), Quaternion.LookRotation(go_ChileTree.transform.up));

        Destroy(go_ChileTree.gameObject);
    }

    public Vector3 GetTreeCenterPosition()
    {

        return go_treeCenter.transform.position;
    }

}
