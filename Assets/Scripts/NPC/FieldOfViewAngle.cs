using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle;//시야각 (120도)
    [SerializeField] private float viewDistance; //시야거리 (10 미터)
    [SerializeField] private LayerMask targetMask; //타켁 마스크 (플레이어)

    private PlayerController thePlayer;
    private NavMeshAgent nav;

    //private Pig thePig;

    private void Start()
    {
        //thePig = GetComponent<Pig>();
        thePlayer = FindObjectOfType<PlayerController>();
        nav = GetComponent<NavMeshAgent>();
    }

    public Vector3 GetTargetPos()
    {
        return thePlayer.transform.position;
    }


    //삭제 Part.34 
    //private Vector3 BoundaryAngle(float _angle)
    //{
    //    _angle += transform.eulerAngles.y;
    //    return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    //}

    public bool View()
    {
        //돼지 시야 부채꼴 만들기
        //Vector3 leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        //Vector3 rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        //돼지의 위치 + 살짝위, 나아갈 방향, 색상
        //Debug.DrawRay(transform.position + transform.up, leftBoundary, Color.red);
        //Debug.DrawRay(transform.position + transform.up, rightBoundary, Color.red);

        //주변에 있는 컬라이더를 뽑아내서 저장시킴
        Collider[] target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < target.Length; i++)
        {
            Transform targetTf = target[i].transform;

            if(targetTf.name == "Player")
            {
                Vector3 direction = (targetTf.position - transform.position).normalized;

                //플레이어와 돼지의 방향의 각도
                float angle = Vector3.Angle(direction, transform.forward);

                if(angle <(viewAngle*0.5f))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + transform.up, direction, out hit, viewDistance))
                    {
                        if (hit.transform.name == "Player")
                        {
                            Debug.Log("플레이어가 돼지 시야 내에 있습니다.");
                            //thePig.Run(hit.transform.position);
                            Debug.DrawRay(transform.position + transform.up, direction, Color.cyan);
                            return true;
                        }
   
                    }
                }
            }

            if(thePlayer.GetRun())
            {
                //플레이어와 자신의 거리 판단
                // 장애물 있을경우 발각되지 않은걸로 판단.

                if(CalcPathLength(thePlayer.transform.position) <= viewDistance)
                {
                    Debug.Log("돼지가 주변에서 뛰고 있는 플레이어의 움직임을 파악했습니다");
                    return true;
                }
            }
        }

        return false;
    }

    private float CalcPathLength(Vector3 _targetPos)
    {
        NavMeshPath path = new NavMeshPath();

        nav.CalculatePath(_targetPos, path);

        Vector3[] wayPoint = new Vector3[path.corners.Length + 2];
        wayPoint[0] = transform.position;
        wayPoint[path.corners.Length + 1] = _targetPos;

        float pathLength = 0;
        for (int i = 0; i < path.corners.Length; i++)
        {
            wayPoint[i + 1] = path.corners[i];
            pathLength += Vector3.Distance(wayPoint[i], wayPoint[i + 1]);
        }

        return pathLength;
    }

}
