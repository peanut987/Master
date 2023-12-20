
using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    public Camera mainCamera; // 主摄像头

    public void AdjustCamera(Vector3 topLeft, Vector3 bottomRight, float verticalHeight, int index = 0)
    {
        // 计算地图的中心点和尺寸
        Vector3 mapCenter = (topLeft + bottomRight) / 2;
        Vector3 mapSize = 1.5f * (bottomRight - topLeft);

        // 设置摄像头位置
        float cameraHeightAboveMap = Mathf.Max(mapSize.x, mapSize.z) * 0.3f / Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);       
        mainCamera.transform.rotation = Quaternion.Euler(90, 180, 0);  // 保证摄像头朝下看

        // 设置摄像头的Orthographic Size，确保整个地图高度能在摄像头视野中

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
