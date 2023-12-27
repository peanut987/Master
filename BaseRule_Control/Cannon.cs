using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
	// Start is called before the first frame update
	public Transform tankTransform;
	public TankSpawner tankSpawner;
	public ManControl manControl;
	public TankControl tankControl;
	public LineRenderer cannon_line;
	public Transform shellPos;
	public float enemyDis;
	public float hitDis;
	public Vector3 hitPoint;
	public Vector3 enemyPoint;
	public bool ColliderFlag = false;
	public bool ColliderFlag1 = false;
	private float castDis = 2000.0f;
	private float sphereRadius = 0.8f; // 设置球体的半径
	void Start()
	{
		tankTransform = transform.parent.parent.parent;
		manControl = tankTransform.GetComponent<ManControl>();
		tankControl = tankTransform.GetComponent<TankControl>();
		tankSpawner = FindObjectOfType<TankSpawner>();
		cannon_line = GetComponent<LineRenderer>();
		shellPos = transform.Find("ShellPos");
		if (manControl.trainingSetting.RedTeam.HumanControl)
		{
			cannon_line.startWidth = 2.5f;
			cannon_line.endWidth = 2.5f;
			castDis = 300.0f;
		}
		else
		{
			cannon_line.startWidth = 0.5f;
			cannon_line.endWidth = 0.5f;
			castDis = 1250.0f;
		}

		ColliderFlag = false;
	}

	// Update is called once per frame
	void Update()
	{
		Cannon_Ray_Cast(shellPos.position, shellPos.position + transform.forward.normalized * castDis);
	}

	public void Cannon_Ray_Cast(Vector3 Satrt, Vector3 End)
	{
		// 射线的起点和方向
		Vector3 start = new Vector3(Satrt.x, Satrt.y, Satrt.z);
		Vector3 direction = Vector3.zero;
		if (manControl.tankTeam == TankTeam.Tank_Red && tankSpawner.useTA)
		{
			if (manControl.tankSpawner.TAList[manControl.MinNum - 1].transform.position.y - manControl.transform.position.y > 5.0f)
				direction = new Vector3(End.x, End.y - 1.0f, End.z) - start;
			else direction = new Vector3(End.x, End.y, End.z) - start;

		}
		else direction = new Vector3(End.x, End.y, End.z) - start;

		// 发射射线
		Ray ray = new Ray(start, direction);
		int Rmask = LayerMask.GetMask("Tank") | LayerMask.GetMask("GroundLayer") | LayerMask.GetMask("wall");
		RaycastHit hit;

		if (Physics.SphereCast(ray, sphereRadius ,out hit, castDis) && ((hit.collider.tag == "Tank_Blue" && manControl.tankTeam == TankTeam.Tank_Red)) && !manControl.Isdead)
		{
			if (manControl.tankTeam == TankTeam.Tank_Red) cannon_line.enabled = true;

			// 如果射线碰撞到物体，绘制从起点到碰撞点的线段
			cannon_line.SetPosition(0, start);
			cannon_line.SetPosition(1, hit.point);

			Vector3 hitPointXOZ = new Vector3(hit.point.x, manControl.transform.position.y, hit.point.z);
			Vector3 target = manControl.tankTeam == TankTeam.Tank_Red ? (tankSpawner.useTA ? tankSpawner.TAList[manControl.MinNum - 1].transform.position : tankSpawner.BlueAgentsList[manControl.MinNum - 1].transform.position) : tankSpawner.Biolist[manControl.MinNum - 1].transform.position;

			enemyPoint = target;
			hitPoint = hit.point;
			if (manControl.MinNum != -1)
			{
				enemyDis = manControl.baseFunction2.CalculateDisX0Z(target, manControl.transform.position);
				hitDis = manControl.baseFunction2.CalculateDisX0Z(hitPointXOZ, manControl.transform.position);
				if (enemyDis - hitDis < 10.0f)
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
		else if (Physics.Raycast(ray, out hit, castDis) && (hit.collider.tag == "Tank_Red" && manControl.tankTeam == TankTeam.Tank_Blue))
        {
			// 如果射线碰撞到物体，绘制从起点到碰撞点的线段
			cannon_line.SetPosition(0, start);
			cannon_line.SetPosition(1, hit.point);

			Vector3 hitPointXOZ = new Vector3(hit.point.x, manControl.transform.position.y, hit.point.z);
			Vector3 target = (manControl.MinNum != 0 && manControl.MinNum != -1) ? tankSpawner.Biolist[manControl.MinNum - 1].transform.position : Vector3.zero;

			enemyPoint = target;
			hitPoint = hit.point;
			if (manControl.MinNum != -1)
			{
				enemyDis = manControl.baseFunction2.CalculateDisX0Z(target, manControl.transform.position);
				hitDis = manControl.baseFunction2.CalculateDisX0Z(hitPointXOZ, manControl.transform.position);
				if (enemyDis - hitDis < 10.0f)
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
		else if (!manControl.Isdead && manControl.trainingSetting.RedTeam.HumanControl && manControl.firetime > manControl.cooldowntime)
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
			ColliderFlag = false;
			ColliderFlag1 = false;
			if (manControl.tankTeam == TankTeam.Tank_Red)
			{
				cannon_line.SetPosition(0, start);
				cannon_line.SetPosition(1, End);
				cannon_line.enabled = false;
			} 
			else
				cannon_line.enabled = false;

		}
	}
}