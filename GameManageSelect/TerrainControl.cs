using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainControl : MonoBehaviour
{
    private TerrainCollider terrainCollider;
    private PhysicMaterial currentMaterial;
    public GameManage gameManage;
    // Start is called before the first frame update
    void Start()
    {
        gameManage = FindObjectOfType<GameManage>();
        // 获取Terrain Collider组件
        terrainCollider = GetComponent<TerrainCollider>();
        if (terrainCollider == null)
        {
            Debug.LogError("Terrain Collider component not found!");
            return;
        }

        // 获取当前Terrain Collider的物理材质
       currentMaterial = terrainCollider.material;

        // 设置弹性
        currentMaterial.bounciness = 1.0f; // 这里设置为你想要的弹性值

        // 将修改后的物理材质应用到Terrain Collider上
        terrainCollider.material = currentMaterial;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= 28)
        {
            // 设置弹性
            currentMaterial.bounciness = 0.0f; // 这里设置为你想要的弹性值

            // 将修改后的物理材质应用到Terrain Collider上
            terrainCollider.material = currentMaterial;
        }
        else
        {
            // 设置弹性
            currentMaterial.bounciness = 1.0f; // 这里设置为你想要的弹性值

            // 将修改后的物理材质应用到Terrain Collider上
            terrainCollider.material = currentMaterial;
        }
    }
}
