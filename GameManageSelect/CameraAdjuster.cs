
using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    public Camera mainCamera; // ������ͷ

    public void AdjustCamera(Vector3 topLeft, Vector3 bottomRight, float verticalHeight, int index = 0)
    {
        // �����ͼ�����ĵ�ͳߴ�
        Vector3 mapCenter = (topLeft + bottomRight) / 2;
        Vector3 mapSize = 1.5f * (bottomRight - topLeft);

        // ��������ͷλ��
        float cameraHeightAboveMap = Mathf.Max(mapSize.x, mapSize.z) * 0.3f / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);       
        mainCamera.transform.rotation = Quaternion.Euler(90, 180, 0);  // ��֤����ͷ���¿�

        // ��������ͷ��Orthographic Size��ȷ��������ͼ�߶���������ͷ��Ұ��

        mainCamera.orthographicSize = Mathf.Max(mapSize.x, mapSize.z) / 2;
        if (index == 0)
        {
            mainCamera.transform.position = new Vector3(103, 325, 60);
            mainCamera.orthographicSize = 1800;
        }
        else if (index == 1)
        {
            mainCamera.orthographicSize = 2300;
            mainCamera.transform.position = new Vector3(-2, 60, mapCenter.z);
        }

    }
}
