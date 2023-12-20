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
		// 射线的起点和方向
		Vector3 start = new Vector3(Satrt.x, Satrt.y, Satrt.z);
		Vector3 direction = new Vector3(End.x, End.y, End.z) - start;

		// 发射射线
		Ray ray = new Ray(start, direction);
		int Rmask = LayerMask.GetMask("Tank") | LayerMask.GetMask("GroundLayer") | LayerMask.GetMask("wall");
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, castDis) && hit.collider.tag == "Tank_Red" && !tankControl.Isdead && tankControl.firetime > tankControl.cooldowntime)
		{
			cannon_line.enabled = true;
			// 如果射线碰撞到物体，绘制从起点到碰撞点的线段
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
			//// 如果射线未碰撞到物体，绘制从起点到指定长度处的线段
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