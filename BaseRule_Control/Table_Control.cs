using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Table_Control : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform tableLayout;

    private void Start()
    {
        // ����������һЩ������Ҫͳ��
        List<string[]> data = new List<string[]>
        {
            new string[] { "Item 1", "Value 1" },
            new string[] { "Item 2", "Value 2" },
            new string[] { "Item 3", "Value 3" }
        };

        // �������ݱ��
        foreach (string[] rowData in data)
        {
            CreateTableRow(rowData);
        }
    }

    private void CreateTableRow(string[] rowData)
    {
        // ʵ������Ԥ���岢����Ϊ TableLayout ��������
        GameObject row = Instantiate(rowPrefab, tableLayout);

        // ��ȡ Text ������ֱ�����Ϊ rowData �е��ı�
        Text[] texts = row.GetComponentsInChildren<Text>();
        texts[0].text = rowData[0];
        texts[1].text = rowData[1];
    }
}
