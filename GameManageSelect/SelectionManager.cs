using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
	private bool isSelecting = false;
	private Vector3 startMousePosition;

	void Update()
	{
		//����1~5�ֱ�������ӿ���1��5��̹�ˣ�������Rʱȡ�����п���
		foreach (var selectableObject in FindObjectsOfType<ManControl>())
		{
			if (Input.GetKeyDown(KeyCode.Alpha1) && selectableObject.TankNum == 1) selectableObject.SetSelected(true);
			else if (Input.GetKeyDown(KeyCode.Alpha2) && selectableObject.TankNum == 2) selectableObject.SetSelected(true);
			else if (Input.GetKeyDown(KeyCode.Alpha3) && selectableObject.TankNum == 3) selectableObject.SetSelected(true);
			else if (Input.GetKeyDown(KeyCode.Alpha4) && selectableObject.TankNum == 4) selectableObject.SetSelected(true);
			else if (Input.GetKeyDown(KeyCode.Alpha5) && selectableObject.TankNum == 5) selectableObject.SetSelected(true);
			if (Input.GetKeyDown(KeyCode.R)) selectableObject.SetSelected(false);
			//if (Input.GetKeyDown(KeyCode.B)) selectableObject.agent.enabled = true;
		}

		// �����������ʱ�����浱ǰλ�ã�����ʼ���п�ѡ
		if (Input.GetMouseButtonDown(0))
		{
			isSelecting = true;
			startMousePosition = Input.mousePosition;
		}
		// �������ɿ�������ʱ��������ѡ
		if (Input.GetMouseButtonUp(0))
		{
			isSelecting = false;

			Vector2 min = Camera.main.ScreenToViewportPoint(startMousePosition);
			Vector2 max = Camera.main.ScreenToViewportPoint(Input.mousePosition);
			Vector2 minWorld = Camera.main.ViewportToWorldPoint(min);
			Vector2 maxWorld = Camera.main.ViewportToWorldPoint(max);

			foreach (var selectableObject in FindObjectsOfType<ManControl>())
			{
				Vector3 viewportPoint = Camera.main.WorldToViewportPoint(selectableObject.transform.position);
				// Check if it's within the rectangle
				if (viewportPoint.x >= Mathf.Min(min.x, max.x) && viewportPoint.x <= Mathf.Max(min.x, max.x)
					&& viewportPoint.y >= Mathf.Min(min.y, max.y) && viewportPoint.y <= Mathf.Max(min.y, max.y))
				{
					selectableObject.SetSelected(true);
				}
				else
				{
					selectableObject.SetSelected(false);
				}
			}
		}

		//�������Ҽ���ָ���ƶ�����
		if (Input.GetMouseButtonDown(1))
		{
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
			{
				foreach (var selectedObject in FindObjectsOfType<ManControl>())
				{
					if (selectedObject.IsSelected()) //ֻ���Ѿ�ѡ�еĵ�λ�´��ƶ�����
					{
						selectedObject.MoveToTarget(hit.point);
						selectedObject.point = hit.point;
					}
				}
			}
		}
	}

	void OnGUI()
	{
		if (isSelecting)
		{
			// ����һ�����ε�ѡ���
			Rect rect = Utils.GetScreenRect(startMousePosition, Input.mousePosition);
			Utils.DrawScreenRect(rect, new Color(1.0f, 0f, 0f, 0.25f));
			Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
		}
	}
}
