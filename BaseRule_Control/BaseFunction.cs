using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFunction : MonoBehaviour
{
	//ע������ű�Ҫ���ص���Ӧ�������ϣ�����transform�Ǵ��
	//ͨ������жϵз�λ��(��<0/��>0)(ʵ��������forward��z�᷽���Ƿ��ģ�Ӧ������>0��<0)
	public float CrossCalculate(Transform X1)
	{
		Vector3 relativePosition = X1.position - transform.position;
		Vector3 result = Vector3.Cross(transform.forward, relativePosition);
		return result.y;
	}

	public float CrossCalculate(Transform X1, Transform X2)
	{
		Vector3 relativePosition = X1.position - X2.position;
		Vector3 result = Vector3.Cross(transform.forward, relativePosition);
		return result.y;
	}

	public float CrossCalculate1(Transform X1, Transform X2)
	{
		Vector3 X12 = new Vector3(X1.position.x, X2.position.y, X1.position.z);
		Vector3 relativePosition = X12 - X2.position;
		Vector3 result = Vector3.Cross(transform.forward, relativePosition);
		return result.y;
	}

	public Vector3 x123;
	public Vector3 x121;
	public float[] DotCalculate(Transform X1, Transform X2)
	{
		float[] result = new float[2];
		Vector3 X12 = new Vector3(X2.position.x, X2.position.y, X2.position.z);
		Vector3 X11 = new Vector3(X1.transform.forward.x, X12.y, X1.transform.forward.z);
		//x123 = X12;
		Vector3 relativePosition = X2.position - X1.position;
		relativePosition = new Vector3(relativePosition.x, 0, relativePosition.z);
		x123 = relativePosition;
		Vector3 X10 = new Vector3(X1.transform.forward.x, 0, X1.transform.forward.z);
		x121 = X10;
		//relativePosition = Vector3(relativePosition.x,0,0);
		float result0 = Vector3.Dot(X10, relativePosition);
		float cos = Vector3.Dot(X10.normalized, relativePosition.normalized);
		float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;
		result[0] = result0;//(ǰ>0/��<0)
		result[1] = angle;//�Ƕ�(0~180)
		return result;
	}

	float[] DotCalculate1(Transform X1, Transform X2)
	{
		float[] result = new float[3];
		float result1 = CrossCalculate(X1);
		// �������������ĵ��,�������0˵������������ǰ��,���С��0˵���������������,�������0˵����������������
		float result0 = Vector3.Dot(X1.forward, X2.forward);
		float cos = Vector3.Dot(X1.forward.normalized, X2.forward.normalized);
		float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;
		result[0] = result0;//(ǰ>0/��<0)
		result[1] = angle;//�Ƕ�(0~180)
		result[2] = result1;
		return result;
	}

	//ͨ������жϵз�λ��
	public float[] DotCalculate2(Transform X1, Transform X2)
	{
		float[] result = new float[2];
		Vector3 relativePosition = X1.position - X2.transform.position;
		float result0 = Vector3.Dot(relativePosition, X2.transform.forward);// �������������ĵ��,�������0˵������������ǰ��,���С��0˵���������������,�������0˵����������������
		float cos = Vector3.Dot(relativePosition.normalized, X2.transform.forward.normalized);
		float angle = Mathf.Acos(cos) * Mathf.Rad2Deg;
		result[0] = result0;
		result[1] = angle;//��������������ǰ������ĽǶ�(����forward������prefabԤ�Ƽ���̹�˵�ǰ���Ƿ��ģ����ʵ�ʽǶ���180-�ýǶ�)
		return result;
	}

	//���������ߴ����ֵļн�
	public float DotCalculate3(Transform X1, Transform X2)
	{
		float angle = 0;
		Vector3 X12 = new Vector3(X1.position.x, X2.position.y, X1.position.z);
		Vector3 relativePosition = X1.position - X2.position;
		Vector3 relativePosition1 = X12 - X2.position;
		float cos = Vector3.Dot(relativePosition.normalized, relativePosition1.normalized);
		if (X1.position.y < X2.position.y) angle = -Mathf.Acos(cos) * Mathf.Rad2Deg;
		else angle = Mathf.Acos(cos) * Mathf.Rad2Deg;
		return angle;
	}

	//���������ߴ����ֵļн�
	public float DotCalculate3_1(Vector3 X1, Vector3 X2)
	{
		float angle = 0;
		Vector3 X12 = new Vector3(X1.x, X2.y, X1.z);
		Vector3 relativePosition = X1 - X2;
		Vector3 relativePosition1 = X12 - X2;
		float cos = Vector3.Dot(relativePosition.normalized, relativePosition1.normalized);
		if (X1.y < X2.y) angle = -Mathf.Acos(cos) * Mathf.Rad2Deg;
		else angle = Mathf.Acos(cos) * Mathf.Rad2Deg;
		return angle;
	}
	public float DotCalculate4(Transform X1, Vector3 X2)
	{
		float angle = 0;
		Vector3 X12_XOZ = X2 - X1.position;
		X12_XOZ = new Vector3(X12_XOZ.x, 0, X12_XOZ.z);

		Vector3 X1_forward_XOZ = new Vector3(X1.forward.x, 0, X1.forward.z);
		angle = Vector3.Angle(X12_XOZ, X1_forward_XOZ);
		return angle;
	}

	public float DotCalculate5(Transform X1, Transform X2)
	{
		float angle = 0;
		Vector3 X12_XOZ = X2.position - X1.position;
		X12_XOZ = new Vector3(X12_XOZ.x, 0, X12_XOZ.z);

		Vector3 X1_forward_XOZ = new Vector3(X1.forward.x, 0, X1.forward.z);
		angle = Vector3.Angle(X12_XOZ, X1_forward_XOZ);
		return angle;
	}

	public float CalculateDisX0Z(Vector3 X1, Vector3 X2)
	{
		X1 = new Vector3(X1.x, 0, X1.z);
		X2 = new Vector3(X2.x, 0, X2.z);
		return (X1 - X2).magnitude;
	}

	public float angleOffset(float inputAngle)
	{
		float outputAngle = 0;
		if (inputAngle > 180.0f) outputAngle = inputAngle - 360.0f;
		else outputAngle = inputAngle;
		return outputAngle;
	}

	public float VectorAngle(Transform X1, Transform X2)//X1Ϊ��ǰ���壬X2ΪҪ��Ƕȵ�����
	{
		Vector3 X1_position = X1.position;
		Vector3 X2_position = X2.position;

		Vector3 vectorToA = new Vector3(X2_position.x - X1_position.x, 0f, X2_position.z - X1_position.z);
		Vector3 forwardB = X1.transform.forward;

		float angle = Vector3.SignedAngle(vectorToA, forwardB, Vector3.up);

		return angle;
	}

	public Vector3 Set_point(Transform rPoint, Vector3 origin, float angle = 45, float distance = 30, float backDis = 0)
	{
		// ������������� angle�Ƕȵķ�������
		Vector3 direction = Quaternion.Euler(0f, angle, 0f) * origin.normalized;

		// ����ƫ��λ��
		Vector3 offsetPosition = Vector3.zero;
		if (Mathf.Abs(angle) == 90)
			offsetPosition = rPoint.position - origin.normalized * backDis + direction * distance;
		else
			offsetPosition = rPoint.position + direction * distance;

		return offsetPosition;

	}
}
