using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Table_Control : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform tableLayout;

    private void Start()
    {
        // 假设这里有一些数据需要统计
        List<string[]> data = new List<string[]>
        {
            new string[] { "Item 1", "Value 1" },
            new string[] { "Item 2", "Value 2" },
            new string[] { "Item 3", "Value 3" }
        };

        // 创建数据表格
        foreach (string[] rowData in data)
        {
            CreateTableRow(rowData);
        }
    }

    private void CreateTableRow(string[] rowData)
    {
        // 实例化行预制体并设置为 TableLayout 的子物体
        GameObject row = Instantiate(rowPrefab, tableLayout);

        // 获取 Text 组件，分别设置为 rowData 中的文本
        Text[] texts = row.GetComponentsInChildren<Text>();
        texts[0].text = rowData[0];
        texts[1].text = rowData[1];
    }
}
