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
        // ��ȡTerrain Collider���
        terrainCollider = GetComponent<TerrainCollider>();
        if (terrainCollider == null)
        {
            Debug.LogError("Terrain Collider component not found!");
            return;
        }

        // ��ȡ��ǰTerrain Collider���������
       currentMaterial = terrainCollider.material;

        // ���õ���
        currentMaterial.bounciness = 1.0f; // ��������Ϊ����Ҫ�ĵ���ֵ

        // ���޸ĺ���������Ӧ�õ�Terrain Collider��
        terrainCollider.material = currentMaterial;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= 28)
        {
            // ���õ���
            currentMaterial.bounciness = 0.0f; // ��������Ϊ����Ҫ�ĵ���ֵ

            // ���޸ĺ���������Ӧ�õ�Terrain Collider��
            terrainCollider.material = currentMaterial;
        }
        else
        {
            // ���õ���
            currentMaterial.bounciness = 1.0f; // ��������Ϊ����Ҫ�ĵ���ֵ

            // ���޸ĺ���������Ӧ�õ�Terrain Collider��
            terrainCollider.material = currentMaterial;
        }
    }
}
