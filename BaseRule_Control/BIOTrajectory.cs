using UnityEngine;

public class BIOTrajectory : MonoBehaviour
{
    public float moveSpeed = 5f; // �����ƶ��ٶ�
    private int maxPoints = 500; // �켣�ϵ�������
    private float pointSpacing = 0.5f; // ��������֮��ļ��

    private LineRenderer lineRenderer;
    public ManControl manControl;
    private float timer; // ��������ʱ��
    private Vector3[] positions;
    private Vector3 bufferPosition;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        manControl = GetComponent<ManControl>();
        positions = new Vector3[maxPoints];
        posRestart(transform);
    }

    void Update()
    {
        if (manControl.Isdead) lineRenderer.positionCount = 0; ;
        UpdateTrajectory();      
        timer += Time.deltaTime;
    }

    void MoveObject()
    {
        // ��ȡ���嵱ǰλ��
        Vector3 currentPosition = transform.position;

        // ����������һ֡��λ��
        Vector3 nextPosition = currentPosition + transform.forward * moveSpeed * Time.deltaTime;

        // �ƶ�����
        transform.position = nextPosition;
    }

    void posRestart(Transform trans)
    {
        for (int i = positions.Length - 1; i > 0; i--)
        {
            positions[i] = new Vector3(trans.position.x, trans.position.y + 3, trans.position.z);
        }
    }

    void UpdateTrajectory()
    {
        bufferPosition = positions[0];
        if (timer >= pointSpacing && !manControl.Isdead)
        {       
           positions[0] = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);

            if (Vector3.Distance(positions[0], positions[1]) > 20)
            {
                // ����ǰλ����ӵ��켣������
                for (int i = positions.Length - 1; i > 0; i--)
                {
                    positions[i] = positions[i - 1];
                }
            }
            // ���� LineRenderer ��λ��
            lineRenderer.positionCount = Mathf.Min(maxPoints, positions.Length);
            lineRenderer.SetPositions(positions);
        }
        else if(manControl.Isdead)
        {
            posRestart(transform);
            // ���� LineRenderer ��λ��
            lineRenderer.positionCount = Mathf.Min(maxPoints, positions.Length);
            lineRenderer.SetPositions(positions);
        }
    }
}
