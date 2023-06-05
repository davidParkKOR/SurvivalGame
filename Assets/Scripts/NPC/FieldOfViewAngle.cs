using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle;//�þ߰� (120��)
    [SerializeField] private float viewDistance; //�þ߰Ÿ� (10 ����)
    [SerializeField] private LayerMask targetMask; //Ÿ�� ����ũ (�÷��̾�)

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


    //���� Part.34 
    //private Vector3 BoundaryAngle(float _angle)
    //{
    //    _angle += transform.eulerAngles.y;
    //    return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    //}

    public bool View()
    {
        //���� �þ� ��ä�� �����
        //Vector3 leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        //Vector3 rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        //������ ��ġ + ��¦��, ���ư� ����, ����
        //Debug.DrawRay(transform.position + transform.up, leftBoundary, Color.red);
        //Debug.DrawRay(transform.position + transform.up, rightBoundary, Color.red);

        //�ֺ��� �ִ� �ö��̴��� �̾Ƴ��� �����Ŵ
        Collider[] target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < target.Length; i++)
        {
            Transform targetTf = target[i].transform;

            if(targetTf.name == "Player")
            {
                Vector3 direction = (targetTf.position - transform.position).normalized;

                //�÷��̾�� ������ ������ ����
                float angle = Vector3.Angle(direction, transform.forward);

                if(angle <(viewAngle*0.5f))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + transform.up, direction, out hit, viewDistance))
                    {
                        if (hit.transform.name == "Player")
                        {
                            Debug.Log("�÷��̾ ���� �þ� ���� �ֽ��ϴ�.");
                            //thePig.Run(hit.transform.position);
                            Debug.DrawRay(transform.position + transform.up, direction, Color.cyan);
                            return true;
                        }
   
                    }
                }
            }

            if(thePlayer.GetRun())
            {
                //�÷��̾�� �ڽ��� �Ÿ� �Ǵ�
                // ��ֹ� ������� �߰����� �����ɷ� �Ǵ�.

                if(CalcPathLength(thePlayer.transform.position) <= viewDistance)
                {
                    Debug.Log("������ �ֺ����� �ٰ� �ִ� �÷��̾��� �������� �ľ��߽��ϴ�");
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
