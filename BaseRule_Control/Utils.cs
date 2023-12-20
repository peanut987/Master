using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
	public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
	{
		// 将起始点从左下移动至右上
		screenPosition1.y = Screen.height - screenPosition1.y;
		screenPosition2.y = Screen.height - screenPosition2.y;
		// 计算顶点
		var topLeft = Vector3.Min(screenPosition1, screenPosition2);
		var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
		// 生成矩形
		return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
	}

	public static void DrawScreenRect(Rect rect, Color color)
	{
		GUI.color = color;
		GUI.DrawTexture(rect, Texture2D.whiteTexture);
		GUI.color = Color.white;
	}

	public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
	{
		// Top
		Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
		// Left
		Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
		// Right
		Utils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
		// Bottom
		Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
	}
}

