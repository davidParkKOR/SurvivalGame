using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TreeComponent : MonoBehaviour
{
    //깎일 나무 조각들
    [SerializeField]
    private GameObject[] go_treePieces;
    [SerializeField]
    private GameObject go_treeCenter;

    //로그 프리팹
    [SerializeField]
    private GameObject go_Log_Prefabs;

    //도끼질 효과
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    //파편 제거 시간
    [SerializeField]
    private float debrisDestroyTime;

    //나무 제거 시간
    [SerializeField]
    private float destroyTime;
    //쓰러질때 힘의 세기
    [SerializeField]
    private float force;
    //자식 트리
    [SerializeField]
    private GameObject go_ChileTree;

    //부모 트리 파괴되면 캡슐 콜라이더 비활성화
    [SerializeField]
    private CapsuleCollider parentCol;

    //자식 트리 쓰러질때 필요한 컴포넌트 활성화 및 중력 활성화
    [SerializeField]
    private CapsuleCollider childCol;
    [SerializeField]
    private Rigidbody childRigid;

    //필요 사운드
    [SerializeField]
    private string chop_Sound;
    [SerializeField]
    private string falldown_Sound;
    [SerializeField]
    private string logChange_Sound;


    public void Chop(Vector3 _pos, float _angleY)
    {
        Hit(_pos);

        //플레이어가 어디서 도끼질을 했는지 판별하여 나무 깎기 
        AngleCalc(_angleY);

        //Piece들이 남았는지 확인
        if (CheckTreePieces())
            return;

        //가운데 나무 파괴하고 나무 쓰러트리기
        FallDownTree();

    }

    //적중 이펙트
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

        //로그 생성
        StartCoroutine(LogCoroutine());

    }
    IEnumerator LogCoroutine()
    {
        yield return new WaitForSeconds(destroyTime);

        SoundManager.instance.PlaySE(logChange_Sound);

        //나무가 쓰러진 위치에 로그 생성
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
