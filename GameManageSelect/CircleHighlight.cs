using UnityEngine;

public class CircleHighlight : MonoBehaviour
{
    private TranningSetting tranningSetting;
    public Transform targetObject; // ������Ҫǿ���ĸ���
    public LineRenderer lineRenderer; // ����LineRenderer���
    public ManControl man;
    public TankControl tank;
    public int radius = 80;
    public float startwidth = 8f;
    public float endwidth = 8f;
    public int scale = 2;
    public Material material_red;
    public Material material_blue;
    public Material material_dead;
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
        //material_red.SetFloat("_Metallic", 1.0f);
        //material_red.SetFloat("_Glossiness", 0.0f);
        //material_blue.SetFloat("_Metallic", 1.0f);
        //material_blue.SetFloat("_Glossiness", 0.0f);
        material_dead.SetFloat("_Metallic", 1.0f);
        material_dead.SetFloat("_Glossiness", 0.0f);
    }

    void Update()
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
            // ����LineRenderer��λ��ΪĿ������λ��
            Vector3 newTargetPos = new Vector3(targetObject.position.x, targetObject.position.y + 2, targetObject.position.z);
            lineRenderer.transform.position = targetObject.position;
            // ����ԲȦ�İ뾶����������Ϊ50
            if ((man && !man.Isdead && !tranningSetting.RedTeam.HumanControl && man.tankTeam == TankTeam.Tank_Red)
                || ((tank && !tank.Isdead) && tranningSetting.RedTeam.HumanControl))
            {
                SetCircleRadius(radius);
            }
            else if ((tank && !tank.Isdead && !tranningSetting.RedTeam.HumanControl)
                || (man && !man.Isdead && !tranningSetting.RedTeam.HumanControl && man.tankTeam == TankTeam.Tank_Blue)
                || (man && !man.Isdead && tranningSetting.RedTeam.HumanControl))
            {
                // ����LineRenderer��λ��ΪĿ������λ��
                lineRenderer.transform.position = targetObject.position;
                // ����ԲȦ�İ뾶����������Ϊ50
                SetCircleRect(radius);
                lineRenderer.positionCount = 5;
                lineRenderer.startWidth = scale * startwidth;
                lineRenderer.endWidth = scale * endwidth;
            }
        }
    }

    // ����LineRenderer��ԲȦ�뾶
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

        // ���������ε��ĸ���
        Vector3[] squareCorners = new Vector3[5];
        squareCorners[0] = targetObject.position + new Vector3(-sideLength / 2, 20.0f, -sideLength / 2);
        squareCorners[1] = targetObject.position + new Vector3(sideLength / 2, 20.0f, -sideLength / 2);
        squareCorners[2] = targetObject.position + new Vector3(sideLength / 2, 20.0f, sideLength / 2);
        squareCorners[3] = targetObject.position + new Vector3(-sideLength / 2, 20.0f, sideLength / 2);
        squareCorners[4] = squareCorners[0]; // �պ�������

        // ����LineRenderer��λ��
        for (int i = 0; i < squareCorners.Length; i++)
        {
            lineRenderer.SetPosition(i, squareCorners[i]);
        }
    }
}


