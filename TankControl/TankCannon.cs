using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCannon : MonoBehaviour
{
	// Start is called before the first frame update
	public Transform tankTransform;
	public TankSpawner tankSpawner;
	public ManControl manControl;
	public TankControl tankControl;
	public LineRenderer cannon_line;
	public Transform shellPos;
	public bool ColliderFlag = false;
	public bool ColliderFlag1 = false;
	private float castDis = 2000.0f;
	void Start()
	{
		tankTransform = transform.parent.parent.parent;
		manControl = tankTransform.GetComponent<ManControl>();
		tankControl = tankTransform.GetComponent<TankControl>();
		tankSpawner = FindObjectOfType<TankSpawner>();
		cannon_line = GetComponent<LineRenderer>();
		shellPos = transform.Find("ShellPos");
		if (tankControl.trainingSetting.BlueTeam.HumanControl)
		{
			cannon_line.startWidth = 2.5f;
			cannon_line.endWidth = 2.5f;
			castDis = 300.0f;
		}
		else
		{
			cannon_line.startWidth = 2.5f;
			cannon_line.endWidth = 2.5f;
			castDis = 2000.0f;
		}
		ColliderFlag = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (tankControl.trainingSetting.BlueTeam.HumanControl)
			Cannon_Ray_Cast(shellPos.position, shellPos.position + transform.forward.normalized * castDis);
	}

	public void Cannon_Ray_Cast(Vector3 Satrt, Vector3 End)
	{
		// ���ߵ����ͷ���
		Vector3 start = new Vector3(Satrt.x, Satrt.y, Satrt.z);
		Vector3 direction = new Vector3(End.x, End.y, End.z) - start;

		// ��������
		Ray ray = new Ray(start, direction);
		int Rmask = LayerMask.GetMask("Tank") | LayerMask.GetMask("GroundLayer") | LayerMask.GetMask("wall");
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, castDis) && hit.collider.tag == "Tank_Red" && !tankControl.Isdead && tankControl.firetime > tankControl.cooldowntime)
		{
			cannon_line.enabled = true;
			// ���������ײ�����壬���ƴ���㵽��ײ����߶�
			cannon_line.SetPosition(0, start);
			cannon_line.SetPosition(1, hit.point);
			Vector3 hitPointXOZ = new Vector3(hit.point.x, tankControl.transform.position.y, hit.point.z);
			if (tankControl.attackEnemy != -1)
			{
				if (tankControl.baseFunction.CalculateDisX0Z(tankSpawner.Biolist[tankControl.attackEnemy - 1].transform.position, tankControl.transform.position)
				- tankControl.baseFunction.CalculateDisX0Z(hitPointXOZ, tankControl.transform.position) < 6.0f)
				{
					ColliderFlag1 = false;
					ColliderFlag = true;
				}
				else
				{
					ColliderFlag = false;
					ColliderFlag1 = true;
				}
			}

		}
		else if (!tankControl.Isdead && tankControl.firetime > tankControl.cooldowntime)
		{
			//// �������δ��ײ�����壬���ƴ���㵽ָ�����ȴ����߶�
			//cannon_line.SetPosition(0, start);
			//cannon_line.SetPosition(1, start + direction * 15.0f);
			cannon_line.enabled = true;
			cannon_line.SetPosition(0, start);
			cannon_line.SetPosition(1, End);
			ColliderFlag = false;
			ColliderFlag1 = false;
		}
		else
		{
			cannon_line.enabled = false;

		}
	}
}