
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class FollowTargetManager : MonoBehaviour
{
    public TankSpawner tankSpawner; // tankSpawner������
    public TranningSetting tranningSetting;
    private const float CameraWidth = 0.2f; // ���л�����ͷ�Ŀ��
    private const float CameraHeight = 0.2f; // ���л�����ͷ�ĸ߶�
    public List<Camera> pipCameras = new List<Camera>(); // �洢���л��л�����ͷ���б�
    void Start()
    {
        tankSpawner.SpawnTanks();
        tranningSetting = FindObjectOfType<TranningSetting>();

        DeleteCamera(tranningSetting.RedTeam.nums, tranningSetting.BlueTeam.nums);

        // ΪBio������������ͷ
        for (int i = 0; i < tankSpawner.Biolist.Count; i++)
        {
            string cameraName = "Bio" + (i + 1);
            Camera camera = GameObject.Find(cameraName).GetComponent<Camera>();
            pipCameras.Add(camera);
            SetCameraViewport(camera, i, 0); // ���������
            SetCameraTarget(camera, tankSpawner.Biolist[i].transform);
            //UnityEngine.Debug.Log("find camera " + camera.name);
        }

        // Ϊ��ɫ��������������ͷ
        if (!tankSpawner.useTA)
        {

            for (int i = 0; i < tankSpawner.BlueAgentsList.Count; i++)
            {
                string cameraName = "RL" + (i + 1);
                Camera camera = GameObject.Find(cameraName).GetComponent<Camera>();
                pipCameras.Add(camera);
                SetCameraViewport(camera, i, 0.8f); // �������Ҳ�
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
                SetCameraViewport(camera, i, 0.8f); // �������Ҳ�
                SetCameraTarget(camera, tankSpawner.TAList[i].transform);
                //UnityEngine.Debug.Log("find camera " + camera.name);
            }
        }

        // Ϊÿ������ͷ�������
        AddNumbersToPiPCameras(tranningSetting.RedTeam.nums);
    }


    private void Update()
    {
        // ��ⰴ��C�Ƿ񱻰���
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
                // ����Ҫɾ���� GameObject��ͨ�����ƻ�������ʽ��
                Camera cameraToDelete = GameObject.Find("Bio4").GetComponent<Camera>();

                if (cameraToDelete != null)
                {
                    // ���٣�ɾ����GameObject
                    Destroy(cameraToDelete);
                }
            }

            Camera cameraToDelete1 = GameObject.Find("Bio5").GetComponent<Camera>();

            if (cameraToDelete1 != null)
            {
                // ���٣�ɾ����GameObject
                Destroy(cameraToDelete1);
            }
        }



        if (RL <= 4)
        {
            if (RL == 3)
            {
                // ����Ҫɾ���� GameObject��ͨ�����ƻ�������ʽ��
                Camera cameraToDelete = GameObject.Find("RL4").GetComponent<Camera>();

                if (cameraToDelete != null)
                {
                    // ���٣�ɾ����GameObject
                    Destroy(cameraToDelete);
                }
            }

            Camera cameraToDelete1 = GameObject.Find("RL5").GetComponent<Camera>();

            if (cameraToDelete1 != null)
            {
                // ���٣�ɾ����GameObject
                Destroy(cameraToDelete1);
            }
        }
    }

    private void TogglePiPCameras()
    {
        foreach (var cam in pipCameras)
        {
            cam.enabled = !cam.enabled; // �л�ÿ������ͷ�ļ���״̬
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
        // �������̹�˶�����һ���Ӷ�����������Transform
        followTarget.firePoint = target.Find("TankRenderers/TankFree_Tower/TankFree_Canon");
    }

    private void AddNumbersToPiPCameras(int Bio)
    {
        //UnityEngine.Debug.Log("RedNums " + Bio);
        for (int i = 0; i < pipCameras.Count; i++)
        {
            //print(i);
            // ����Canvas
            Canvas canvas = new GameObject("Canvas" + i).AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<CanvasScaler>();
            canvas.gameObject.AddComponent<GraphicRaycaster>();

            // ����Canvas��С��λ��
            RectTransform rt = canvas.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Screen.width, Screen.height);

            // ���TextMeshPro�ı�
            TextMeshProUGUI text = new GameObject("Text" + i).AddComponent<TextMeshProUGUI>();
            text.text = (i > (Bio - 1) ? i - (Bio - 1) : i + 1).ToString();
            text.fontSize = 70; // ������С����Ӧ��Ļ
            text.color = Color.white; // �����ı���ɫΪ��ɫ
            text.alignment = TextAlignmentOptions.Center;
            text.transform.SetParent(canvas.transform, false);

            // Position the text in the bottom of the camera view
            RectTransform textRt = text.GetComponent<RectTransform>();
            textRt.sizeDelta = new Vector2(Screen.width * CameraWidth, Screen.height * CameraHeight);
            textRt.anchorMin = new Vector2(i < Bio ? 0f : 0.75f, 0.15f); // Yλ�ø�������ͷ��������
            textRt.anchorMax = new Vector2(i < Bio ? 0.21f : 0.95f, 0.3f); // Yλ�ø�������ͷ��������
            textRt.pivot = new Vector2(i < Bio ? 0 : 0.8f, 0);
            textRt.anchoredPosition = new Vector2(0, (0.8f - (i > (Bio - 1) ? i - (Bio - 1) : i + 1) * CameraHeight) * Screen.height);
        }
    }




}

public class FollowTarget : MonoBehaviour
{
    public Transform target; // Ŀ�����
    public Transform firePoint; // ��̨��Transform
    public Vector3 offset = new Vector3(0f, 8f, -12f); // ��Ŀ������λ��

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
