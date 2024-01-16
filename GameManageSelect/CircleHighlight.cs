using UnityEngine;

public class CircleHighlight : MonoBehaviour
{
    public bool isTrajectory = false;
    private TranningSetting tranningSetting;
    public Transform targetObject; // 引用需要强调的个体
    public LineRenderer lineRenderer; // 引用LineRenderer组件
    public ManControl man;
    public TankControl tank;
    public int radius = 80;
    public float startwidth = 8f;
    public float endwidth = 8f;
    public int scale = 2;
    public Material material_red;
    public Material material_blue;
    public Material material_dead;

    private int maxPoints = 500; // 轨迹上的最大点数
    private float pointSpacing = 0.5f; // 新增：点之间的间隔
    private float timer; // 新增：计时器
    private Vector3[] positions;
    private void Start()
    {
        targetObject = transform;
        tranningSetting = FindObjectOfType<TranningSetting>();
        lineRenderer = GetComponent<LineRenderer>();
        man = GetComponent<ManControl>();
        tank = GetComponent<TankControl>();
        lineRenderer.positionCount = radius;
        lineRenderer.startWidth = startwidth;
        lineRenderer.endWidth = endwidth;
        scale = 1;
        material_dead.SetFloat("_Metallic", 1.0f);
        material_dead.SetFloat("_Glossiness", 0.0f);

        positions = new Vector3[maxPoints];
        posRestart(transform);
    }

    void Update()
    {
        if(!isTrajectory)
        {
            targetObject = transform;
            if ((man && !man.Isdead && !tranningSetting.RedTeam.HumanControl && man.tankTeam == TankTeam.Tank_Red)
                || ((tank && !tank.Isdead) && tranningSetting.RedTeam.HumanControl))
            {
                lineRenderer.enabled = true;
                lineRenderer.material = material_red;
            }
            else if ((tank && !tank.Isdead && !tranningSetting.RedTeam.HumanControl)
                || (man && !man.Isdead && !tranningSetting.RedTeam.HumanControl && man.tankTeam == TankTeam.Tank_Blue)
                || (man && !man.Isdead && tranningSetting.RedTeam.HumanControl))
            {
                lineRenderer.enabled = true;
                lineRenderer.material = material_blue;
            }
            else lineRenderer.enabled = false;

            if (targetObject != null && lineRenderer != null)
            {
                // 设置LineRenderer的位置为目标个体的位置
                Vector3 newTargetPos = new Vector3(targetObject.position.x, targetObject.position.y + 2, targetObject.position.z);
                lineRenderer.transform.position = targetObject.position;
                // 设置圆圈的半径，这里设置为50
                if ((man && !man.Isdead && !tranningSetting.RedTeam.HumanControl && man.tankTeam == TankTeam.Tank_Red)
                    || ((tank && !tank.Isdead) && tranningSetting.RedTeam.HumanControl))
                {
                    SetCircleRadius(radius);
                }
                else if ((tank && !tank.Isdead && !tranningSetting.RedTeam.HumanControl)
                    || (man && !man.Isdead && !tranningSetting.RedTeam.HumanControl && man.tankTeam == TankTeam.Tank_Blue)
                    || (man && !man.Isdead && tranningSetting.RedTeam.HumanControl))
                {
                    // 设置LineRenderer的位置为目标个体的位置
                    lineRenderer.transform.position = targetObject.position;
                    // 设置圆圈的半径，这里设置为50
                    SetCircleRect(radius);
                    lineRenderer.positionCount = 5;
                    lineRenderer.startWidth = scale * startwidth;
                    lineRenderer.endWidth = scale * endwidth;
                }
            }
        }
        else
        {
            if ((man && !man.Isdead && !tranningSetting.RedTeam.HumanControl && man.tankTeam == TankTeam.Tank_Red)
                || ((tank && !tank.Isdead) && tranningSetting.RedTeam.HumanControl))
            {
                lineRenderer.enabled = true;
                lineRenderer.material = material_red;
            }
            else if ((tank && !tank.Isdead && !tranningSetting.RedTeam.HumanControl)
                || (man && !man.Isdead && !tranningSetting.RedTeam.HumanControl && man.tankTeam == TankTeam.Tank_Blue)
                || (man && !man.Isdead && tranningSetting.RedTeam.HumanControl))
            {
                lineRenderer.enabled = true;
                lineRenderer.material = material_blue;
            }
            else lineRenderer.enabled = false;
            UpdateTrajectory();
            timer += Time.deltaTime;
        }

    }

    // 设置LineRenderer的圆圈半径
    void SetCircleRadius(float radius)
    {
        int positionCount = lineRenderer.positionCount;
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * 2 * Mathf.PI / positionCount;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            Vector3 position = targetObject.position + new Vector3(x, 20.0f, z);

            lineRenderer.SetPosition(i, position);
        }
        lineRenderer.SetPosition(positionCount - 1, lineRenderer.GetPosition(0));
    }

    void SetCircleRect(float sideLength)
    {
        lineRenderer.positionCount = 5;

        // 定义正方形的四个角
        Vector3[] squareCorners = new Vector3[5];
        squareCorners[0] = targetObject.position + new Vector3(-sideLength / 2, 20.0f, -sideLength / 2);
        squareCorners[1] = targetObject.position + new Vector3(sideLength / 2, 20.0f, -sideLength / 2);
        squareCorners[2] = targetObject.position + new Vector3(sideLength / 2, 20.0f, sideLength / 2);
        squareCorners[3] = targetObject.position + new Vector3(-sideLength / 2, 20.0f, sideLength / 2);
        squareCorners[4] = squareCorners[0]; // 闭合正方形

        // 设置LineRenderer的位置
        for (int i = 0; i < squareCorners.Length; i++)
        {
            lineRenderer.SetPosition(i, squareCorners[i]);
        }
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
        if (timer >= pointSpacing && !man.Isdead)
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
        else if (man.Isdead)
        {
            posRestart(transform);
            // 设置 LineRenderer 的位置
            lineRenderer.positionCount = Mathf.Min(maxPoints, positions.Length);
            lineRenderer.SetPositions(positions);
        }
    }
}


