using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SceneSwitcher : MonoBehaviour
{
    public GameObject[] scenePrefabs;  // 在Unity编辑器中拖放你的场景Prefabs
    public GameManage gameManage;

    public Dictionary<int, GameObject> mapData = new Dictionary<int, GameObject>();
    [Tooltip("这里设置为-2，表示不开启场景功能")]
    public int currentIndex = -1; // 当前场景索引

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


    // 随机选择一个场景Prefab
    private GameObject RandomSelectScene()
    {
        int randomIndex = Random.Range(0, scenePrefabs.Length);
        return scenePrefabs[randomIndex];
    }

    // 循环选择一个场景Prefab
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
        //如果currentIndex为-2，表示不开启场景功能
        if (currentIndex == -2)
        {
            return -1;
        }
        // 销毁当前场景（如果有）
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

        // 实例化选择的场景Prefab
        Instantiate(selectedPrefab, Vector3.zero, Quaternion.identity, transform);
        selectedPrefab.transform.position = new Vector3(-525f, 124.1f, -536.8f);
        return findSceneIndex(scenePrefabs, selectedPrefab);
    }
}
