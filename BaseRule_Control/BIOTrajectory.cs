using UnityEngine;

public class BIOTrajectory : MonoBehaviour
{
    public float moveSpeed = 5f; // 物体移动速度
    private int maxPoints = 500; // 轨迹上的最大点数
    private float pointSpacing = 0.5f; // 新增：点之间的间隔

    private LineRenderer lineRenderer;
    public ManControl manControl;
    private float timer; // 新增：计时器
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
        // 获取物体当前位置
        Vector3 currentPosition = transform.position;

        // 计算物体下一帧的位置
        Vector3 nextPosition = currentPosition + transform.forward * moveSpeed * Time.deltaTime;

        // 移动物体
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
                // 将当前位置添加到轨迹点数组
                for (int i = positions.Length - 1; i > 0; i--)
                {
                    positions[i] = positions[i - 1];
                }
            }
            // 设置 LineRenderer 的位置
            lineRenderer.positionCount = Mathf.Min(maxPoints, positions.Length);
            lineRenderer.SetPositions(positions);
        }
        else if(manControl.Isdead)
        {
            posRestart(transform);
            // 设置 LineRenderer 的位置
            lineRenderer.positionCount = Mathf.Min(maxPoints, positions.Length);
            lineRenderer.SetPositions(positions);
        }
    }
}
