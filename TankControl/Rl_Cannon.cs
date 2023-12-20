using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rl_Cannon : MonoBehaviour
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
	void Start()
	{
		tankTransform = transform.parent.parent.parent;
		tankControl = tankTransform.GetComponent<TankControl>();
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
		// ���ߵ����ͷ���
		Vector3 start = new Vector3(Start.x, Start.y, Start.z);
		Vector3 direction = new Vector3(End.x, End.y, End.z) - start;

		// ��������
		Ray ray = new Ray(start, direction);
		int Rmask = LayerMask.GetMask("Tank") | LayerMask.GetMask("GroundLayer") | LayerMask.GetMask("wall");
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 2000) && hit.collider.tag == "Tank_Red" && !tankControl.Isdead && tankControl.Enemy_len != 0)
		{
			/*cannon_line.enabled = true;
			// ���������ײ�����壬���ƴ���㵽��ײ����߶�
			cannon_line.SetPosition(0, start);
			cannon_line.SetPosition(1, hit.point);
			Vector3 hitPointXOZ = new Vector3(hit.point.x, tankControl.transform.position.y, hit.point.z);*/


			if (tankControl.trainingSetting.RedTeam.HumanControl)
			{
				ColliderFlag = false;
			}
			else
			{
				ColliderFlag = true;
			}
			ColliderFlag = true;
		}
		else
		{
			//// �������δ��ײ�����壬���ƴ���㵽ָ�����ȴ����߶�
			//cannon_line.SetPosition(0, start);
			//cannon_line.SetPosition(1, start + direction * 15.0f);
			/*cannon_line.enabled = false;
			cannon_line.SetPosition(0, start);
			cannon_line.SetPosition(1, End);*/

			ColliderFlag = false;

		}
	}
}