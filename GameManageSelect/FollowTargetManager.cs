
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class FollowTargetManager : MonoBehaviour
{
    public TankSpawner tankSpawner; // tankSpawner的引用
    public TranningSetting tranningSetting;
    private const float CameraWidth = 0.2f; // 画中画摄像头的宽度
    private const float CameraHeight = 0.2f; // 画中画摄像头的高度
    public List<Camera> pipCameras = new List<Camera>(); // 存储所有画中画摄像头的列表
    void Start()
    {
        tankSpawner.SpawnTanks();
        tranningSetting = FindObjectOfType<TranningSetting>();

        DeleteCamera(tranningSetting.RedTeam.nums, tranningSetting.BlueTeam.nums);

        // 为Bio队伍设置摄像头
        for (int i = 0; i < tankSpawner.Biolist.Count; i++)
        {
            string cameraName = "Bio" + (i + 1);
            Camera camera = GameObject.Find(cameraName).GetComponent<Camera>();
            pipCameras.Add(camera);
            SetCameraViewport(camera, i, 0); // 设置在左侧
            SetCameraTarget(camera, tankSpawner.Biolist[i].transform);
            //UnityEngine.Debug.Log("find camera " + camera.name);
        }

        // 为蓝色方队伍设置摄像头
        if (!tankSpawner.useTA)
        {

            for (int i = 0; i < tankSpawner.BlueAgentsList.Count; i++)
            {
                string cameraName = "RL" + (i + 1);
                Camera camera = GameObject.Find(cameraName).GetComponent<Camera>();
                pipCameras.Add(camera);
                SetCameraViewport(camera, i, 0.8f); // 设置在右侧
                SetCameraTarget(camera, tankSpawner.BlueAgentsList[i].transform);
                //UnityEngine.Debug.Log("find camera " + camera.name);
            }
        }
        else
        {
            for (int i = 0; i < tankSpawner.TAList.Count; i++)
            {
                string cameraName = "RL" + (i + 1);
                Camera camera = GameObject.Find(cameraName).GetComponent<Camera>();
                pipCameras.Add(camera);
                SetCameraViewport(camera, i, 0.8f); // 设置在右侧
                SetCameraTarget(camera, tankSpawner.TAList[i].transform);
                //UnityEngine.Debug.Log("find camera " + camera.name);
            }
        }

        // 为每个摄像头添加数字
        AddNumbersToPiPCameras(tranningSetting.RedTeam.nums);
    }


    private void Update()
    {
        // 检测按键C是否被按下
        if (Input.GetKeyDown(KeyCode.C))
        {
            TogglePiPCameras();
        }
    }

    private void DeleteCamera(int Bio, int RL)
    {
        if (Bio <= 4)
        {
            if (Bio == 3)
            {
                // 查找要删除的 GameObject（通过名称或其他方式）
                Camera cameraToDelete = GameObject.Find("Bio4").GetComponent<Camera>();

                if (cameraToDelete != null)
                {
                    // 销毁（删除）GameObject
                    Destroy(cameraToDelete);
                }
            }

            Camera cameraToDelete1 = GameObject.Find("Bio5").GetComponent<Camera>();

            if (cameraToDelete1 != null)
            {
                // 销毁（删除）GameObject
                Destroy(cameraToDelete1);
            }
        }



        if (RL <= 4)
        {
            if (RL == 3)
            {
                // 查找要删除的 GameObject（通过名称或其他方式）
                Camera cameraToDelete = GameObject.Find("RL4").GetComponent<Camera>();

                if (cameraToDelete != null)
                {
                    // 销毁（删除）GameObject
                    Destroy(cameraToDelete);
                }
            }

            Camera cameraToDelete1 = GameObject.Find("RL5").GetComponent<Camera>();

            if (cameraToDelete1 != null)
            {
                // 销毁（删除）GameObject
                Destroy(cameraToDelete1);
            }
        }
    }

    private void TogglePiPCameras()
    {
        foreach (var cam in pipCameras)
        {
            cam.enabled = !cam.enabled; // 切换每个摄像头的激活状态
        }
    }

    private void SetCameraViewport(Camera camera, int index, float startX)
    {
        camera.rect = new Rect(startX, 1 - (index + 1) * CameraHeight, CameraWidth, CameraHeight);
    }

    private void SetCameraTarget(Camera camera, Transform target)
    {
        FollowTarget followTarget = camera.gameObject.GetComponent<FollowTarget>();
        if (followTarget == null)
        {
            followTarget = camera.gameObject.AddComponent<FollowTarget>();
        }

        followTarget.target = target;
        // 假设你的坦克对象有一个子对象是炮塔的Transform
        followTarget.firePoint = target.Find("TankRenderers/TankFree_Tower/TankFree_Canon");
    }

    private void AddNumbersToPiPCameras(int Bio)
    {
        //UnityEngine.Debug.Log("RedNums " + Bio);
        for (int i = 0; i < pipCameras.Count; i++)
        {
            //print(i);
            // 创建Canvas
            Canvas canvas = new GameObject("Canvas" + i).AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<CanvasScaler>();
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            // 设置Canvas大小和位置
            RectTransform rt = canvas.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Screen.width, Screen.height);

            // 添加TextMeshPro文本
            TextMeshProUGUI text = new GameObject("Text" + i).AddComponent<TextMeshProUGUI>();
            text.text = (i > (Bio - 1) ? i - (Bio - 1) : i + 1).ToString();
            text.fontSize = 70; // 调整大小以适应屏幕
            text.color = Color.white; // 设置文本颜色为红色
            text.alignment = TextAlignmentOptions.Center;
            text.transform.SetParent(canvas.transform, false);

            // Position the text in the bottom of the camera view
            RectTransform textRt = text.GetComponent<RectTransform>();
            textRt.sizeDelta = new Vector2(Screen.width * CameraWidth, Screen.height * CameraHeight);
            textRt.anchorMin = new Vector2(i < Bio ? 0f : 0.75f, 0.15f); // Y位置根据摄像头索引调整
            textRt.anchorMax = new Vector2(i < Bio ? 0.21f : 0.95f, 0.3f); // Y位置根据摄像头索引调整
            textRt.pivot = new Vector2(i < Bio ? 0 : 0.8f, 0);
            textRt.anchoredPosition = new Vector2(0, (0.8f - (i > (Bio - 1) ? i - (Bio - 1) : i + 1) * CameraHeight) * Screen.height);
        }
    }




}

public class FollowTarget : MonoBehaviour
{
    public Transform target; // 目标个体
    public Transform firePoint; // 炮台的Transform
    public Vector3 offset = new Vector3(0f, 8f, -12f); // 与目标的相对位置

    private void Update()
    {
        if (target)
        {
            Vector3 adjustedOffset = offset;
            if (firePoint)
            {
                adjustedOffset = Quaternion.Euler(0, firePoint.rotation.eulerAngles.y, 0) * offset;
            }
            else
            {
                UnityEngine.Debug.LogWarning("No fire point found!");
            }
            transform.position = target.position + adjustedOffset;
            transform.LookAt(target.position);
        }
    }
}
