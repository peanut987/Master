using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Rl_Cannon : MonoBehaviour
{
	// Start is called before the first frame update
	public Transform tankTransform;
	public TankSpawner tankSpawner;
	public BaseFunction baseFunction;
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
	void Start()
	{
		tankTransform = transform.parent.parent.parent;
		tankControl = tankTransform.GetComponent<TankControl>();
		baseFunction = FindObjectOfType<BaseFunction>();
		tankSpawner = FindObjectOfType<TankSpawner>();
		cannon_line = GetComponent<LineRenderer>();
		shellPos = transform.Find("ShellPos");
		cannon_line.startWidth = 2.5f;
		cannon_line.endWidth = 2.5f;
		ColliderFlag = false;
	}

	// Update is called once per frame
	void Update()
	{
		Cannon_Ray_Cast(shellPos.position, shellPos.position + transform.forward.normalized * 2000);
	}

	public void Cannon_Ray_Cast(Vector3 Start, Vector3 End)
	{
		// 射线的起点和方向
		Vector3 start = new Vector3(Start.x, Start.y, Start.z);
		Vector3 direction = new Vector3(End.x, End.y, End.z) - start;

		// 发射射线
		Ray ray = new Ray(start, direction);
		int Rmask = LayerMask.GetMask("Tank") | LayerMask.GetMask("GroundLayer") | LayerMask.GetMask("wall");
		RaycastHit hit;

		if (Physics.SphereCast(ray, 0.7f, out hit, 1250) && (hit.collider.tag == "Tank_Red" && !tankControl.Isdead && tankControl.Enemy_len != 0))
		{
			/*cannon_line.enabled = true;
			// 如果射线碰撞到物体，绘制从起点到碰撞点的线段
			cannon_line.SetPosition(0, start);
			cannon_line.SetPosition(1, hit.point);
			Vector3 hitPointXOZ = new Vector3(hit.point.x, tankControl.transform.position.y, hit.point.z);*/
			// 如果射线碰撞到物体，绘制从起点到碰撞点的线段
			cannon_line.SetPosition(0, start);
			cannon_line.SetPosition(1, hit.point);
			Vector3 hitPointXOZ = new Vector3(hit.point.x, tankControl.transform.position.y, hit.point.z);
			Vector3 target = (tankControl.EnemyBiodir.Count != 0) ? tankControl.EnemyBiodir.First().Value.transform.position : Vector3.zero;

			enemyPoint = target;
			hitPoint = hit.point;
			if (tankControl.EnemyBiodir.Count != 0)
			{
				enemyDis = baseFunction.CalculateDisX0Z(target, tankControl.transform.position);
				hitDis = baseFunction.CalculateDisX0Z(hitPointXOZ, tankControl.transform.position);
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
		else
		{
			//// 如果射线未碰撞到物体，绘制从起点到指定长度处的线段
			//cannon_line.SetPosition(0, start);
			//cannon_line.SetPosition(1, start + direction * 15.0f);
			/*cannon_line.enabled = false;
			cannon_line.SetPosition(0, start);
			cannon_line.SetPosition(1, End);*/

			ColliderFlag = false;

		}
	}
}