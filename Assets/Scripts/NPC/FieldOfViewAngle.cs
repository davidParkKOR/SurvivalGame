using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle;//시야각 (120도)
    [SerializeField] private float viewDistance; //시야거리 (10 미터)
    [SerializeField] private LayerMask targetMask; //타켁 마스크 (플레이어)

    private Pig thePig;

    private void Start()
    {
        thePig = GetComponent<Pig>();
    }

    private void Update()
    {
        View();
    }

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    private void View()
    {
        //돼지 시야 부채꼴 만들기
        Vector3 leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        //돼지의 위치 + 살짝위, 나아갈 방향, 색상
        Debug.DrawRay(transform.position + transform.up, leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, rightBoundary, Color.red);

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
                            thePig.Run(hit.transform.position);
                            thePig.Run(hit.transform.position);
                            Debug.DrawRay(transform.position + transform.up, direction, Color.cyan);
                        }
   
                    }
                }
            }
        }
    }



}
