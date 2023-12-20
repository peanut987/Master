using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject[] scenePrefabs;  // ��Unity�༭�����Ϸ���ĳ���Prefabs
    public GameManage gameManage;

    public Dictionary<int, GameObject> mapData = new Dictionary<int, GameObject>();
    [Tooltip("��������Ϊ-2����ʾ��������������")]
    public int currentIndex = -1; // ��ǰ��������

    void start()
    {
        print("start");
        gameManage = FindObjectOfType<GameManage>();
        for (int i = 0; i < gameManage.scenePrefabs.Length; i++)
        {
            scenePrefabs[i] = gameManage.scenePrefabs[i];
        }
        print("success");
    }


    // ���ѡ��һ������Prefab
    private GameObject RandomSelectScene()
    {
        int randomIndex = Random.Range(0, scenePrefabs.Length);
        return scenePrefabs[randomIndex];
    }

    // ѭ��ѡ��һ������Prefab
    private GameObject CycleSelectScene()
    {
        currentIndex = (currentIndex + 1) % scenePrefabs.Length;
        return scenePrefabs[currentIndex];
    }

    private int findSceneIndex(GameObject[] scenePrefabList, GameObject scenePrefab)
    {
        for(int i = 0; i < scenePrefabList.Length; i++)
        {
            if(scenePrefab == scenePrefabList[i])
            {
                return i;
            }
        }
        return -1;
    }

    public int LoadNewScene(string strategy = "random")
    {
        //���currentIndexΪ-2����ʾ��������������
        if (currentIndex == -2)
        {
            return -1;
        }
        // ���ٵ�ǰ����������У�
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GameObject selectedPrefab;
        if (strategy == "random")
        {
            selectedPrefab = RandomSelectScene();
            //Debug.Log("Random select scene: " + selectedPrefab.name);
        }
        else if (strategy == "cycle")
        {
            selectedPrefab = CycleSelectScene();
            //Debug.Log("Cycle select scene: " + selectedPrefab.name);
        }
        else
        {
            Debug.LogError("Unknown strategy: " + strategy);
            return -1;
        }

        // ʵ����ѡ��ĳ���Prefab
        Instantiate(selectedPrefab, Vector3.zero, Quaternion.identity, transform);
        selectedPrefab.transform.position = new Vector3(-525f, 124.1f, -536.8f);
        return findSceneIndex(scenePrefabs, selectedPrefab);
    }
}
