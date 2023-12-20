using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TankSpawner : MonoBehaviour
{
    public TranningSetting TranningSetting;

    public TankControl tankPrefab;
    public ManControl tankPrefab1;

    private int SUM_RED;
    private int SUM_BLUE;

    private string BehaviorBLUE;
    private string BehaviorRED;

    //����˫������
    private Material materialRED;
    private Material materalBLUE;

    //����˫��Ѫ����ɫ
    private Color SliderRED;
    private Color SliderBLUE;

    public List<TankControl> AgentsList = new();
    public List<TankControl> RedAgentsList = new();
    public List<TankControl> BlueAgentsList = new();
    public List<ManControl> Biolist = new();
    public List<ManControl> TAList = new();//���Ķ����㷨�����б�
    public bool use_sensor;

    public bool useBio = true;
    public bool useTA = false;//�Ƿ�ʹ�����Ķ���
    public GameObject initialPos;

    public void SpawnTanks()
    {
        //��ʼ������
        TranningSetting = FindObjectOfType<TranningSetting>();
        //����ҵ������ļ�
        if (TranningSetting)
        {

            SUM_BLUE = TranningSetting.BlueTeam.nums;
            materalBLUE = TranningSetting.BlueTeam.Matera;
            SUM_RED = TranningSetting.RedTeam.nums;
            materialRED = TranningSetting.RedTeam.Matera;
            BehaviorBLUE = TranningSetting.BlueTeam.BehaviorName;
            BehaviorRED = TranningSetting.RedTeam.BehaviorName;
            SliderBLUE = TranningSetting.BlueTeam.Slidercolor;
            SliderRED = TranningSetting.RedTeam.Slidercolor;
            if (TranningSetting.algorithmSelect.NRStandard && !TranningSetting.algorithmSelect.RLStandard) useTA = true;
            else if (!TranningSetting.algorithmSelect.NRStandard && TranningSetting.algorithmSelect.RLStandard) useTA = false;
        }
        else
        {
            Debug.LogError("����ʧ��");
            SUM_BLUE = 20;
            SUM_RED = 20;
        }
        // Spawn Team Blue tanks(ѧϰ��)
        for (int i = 0; i < SUM_BLUE; i++)
        {
            if (useTA)
            {
                ManControl newTank = Instantiate(tankPrefab1, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                newTank.tankTeam = TankTeam.Tank_Blue;
                newTank.TankNum = i;
                newTank.name = "BlueTeanm" + i;
                newTank.NUM_Text.text = (newTank.TankNum + 1).ToString();
                newTank.fillImage.color = SliderBLUE;
                newTank.tag = "Tank_Blue";
                newTank.Isdead = false;

                if (TranningSetting.RedTeam.HumanControl)
                    ChangeManColor(newTank, materialRED);
                else
                    ChangeManColor(newTank, materalBLUE);
                TAList.Add(newTank);
            }
            else
            {
                TankControl newTank = Instantiate(tankPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                newTank.tankTeam = TankTeam.Tank_Blue;
                newTank.TankNum = i;
                newTank.name = "BlueTeanm" + i;
                newTank.NUM_Text.text = (newTank.TankNum + 1).ToString();
                newTank.fillImage.color = SliderBLUE;
                newTank.tag = "Tank_Blue";
                newTank.Isdead = false;

                //���� RayPerceptionSensorComponent3D ����ǩ

                ChangeRaySensor3D(newTank);

                if (TranningSetting.RedTeam.HumanControl)
                    ChangeTankColor(newTank, materialRED);
                else
                    ChangeTankColor(newTank, materalBLUE);
                BlueAgentsList.Add(newTank);
                AgentsList.Add(newTank);
            }

        }

        // Spawn Team Red tanks(���򷽻���ѧϰ��)
        if (useBio)
        {
            for (int i = 0; i < SUM_RED; i++)
            {
                ManControl newTank = Instantiate(tankPrefab1, new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
                newTank.tankTeam = TankTeam.Tank_Red;
                newTank.TankNum = i + 1;
                newTank.name = "BioTeam" + newTank.TankNum;
                newTank.NUM_Text.text = newTank.TankNum.ToString();
                newTank.fillImage.color = SliderRED;
                newTank.tag = "Tank_Red";
                newTank.Isdead = false;
                if (TranningSetting.RedTeam.HumanControl)
                {
                    ChangeManColor(newTank, materalBLUE);
                }
                else
                    ChangeManColor(newTank, materialRED);

                Biolist.Add(newTank);

            }
        }
        else
        {
            for (int i = 0; i < SUM_RED; i++)
            {
                TankControl newTank = Instantiate(tankPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                newTank.tankTeam = TankTeam.Tank_Red;
                newTank.TankNum = i;
                newTank.name = "RedTeanm" + i;
                newTank.NUM_Text.text = newTank.TankNum.ToString();
                newTank.fillImage.color = SliderRED;
                newTank.tag = "Tank_Red";
                newTank.Isdead = false;

                ChangeRaySensor3D(newTank);
                ChangeTankColor(newTank, materialRED);
                RedAgentsList.Add(newTank);
                AgentsList.Add(newTank);
            }
        }

    }

    //�޸�tank���ԣ������޸���ɫ
    public void ChangeTankColor(TankControl tank, Material mateial)
    {
        // ʹ��Transform.Find�ҵ�TankRenderers/TankFree_Body
        Transform TankRenderTransform = tank.transform.Find("TankRenderers");
        // ��TankFree_Body��ȡMeshRenderer
        foreach (var renderer in TankRenderTransform.GetComponentsInChildren<MeshRenderer>())
        {
            // �����µĲ���
            renderer.material = mateial;
        }
    }


    public void ChangeManColor(ManControl tank, Material mateial)
    {
        // ʹ��Transform.Find�ҵ�TankRenderers/TankFree_Body
        Transform TankRenderTransform = tank.transform.Find("TankRenderers");
        // ��TankFree_Body��ȡMeshRenderer
        foreach (var renderer in TankRenderTransform.GetComponentsInChildren<MeshRenderer>())
        {
            // �����µĲ���
            renderer.material = mateial;
        }
    }


    private void ChangeRaySensor3D(TankControl tank)
    {
        if (!use_sensor) { return; }
        RayPerceptionSensorComponent3D[] rayPerceptionSensor = tank.GetComponents<RayPerceptionSensorComponent3D>();
        if (tank.tankTeam == TankTeam.Tank_Red)
        {
            rayPerceptionSensor[0].DetectableTags[0] = "Tank_Red";
            rayPerceptionSensor[0].DetectableTags[1] = "Tank_Blue";
        }
        else
        {
            rayPerceptionSensor[0].DetectableTags[0] = "Tank_Blue";
            rayPerceptionSensor[0].DetectableTags[1] = "Tank_Red";
        }

    }

}
