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

    //设置双方材质
    private Material materialRED;
    private Material materalBLUE;

    //设置双方血条颜色
    private Color SliderRED;
    private Color SliderBLUE;

    public List<TankControl> AgentsList = new();
    public List<TankControl> RedAgentsList = new();
    public List<TankControl> BlueAgentsList = new();
    public List<ManControl> Biolist = new();
    public List<ManControl> TAList = new();//论文对照算法对手列表
    public bool use_sensor;

    public bool useBio = true;
    public bool useTA = false;//是否使用论文对照
    public GameObject initialPos;

    public void SpawnTanks()
    {
        //初始化参数
        TranningSetting = FindObjectOfType<TranningSetting>();
        //如果找到配置文件
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
            Debug.LogError("配置失败");
            SUM_BLUE = 20;
            SUM_RED = 20;
        }
        // Spawn Team Blue tanks(学习方)
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

                //设置 RayPerceptionSensorComponent3D 检测标签

                ChangeRaySensor3D(newTank);

                if (TranningSetting.RedTeam.HumanControl)
                    ChangeTankColor(newTank, materialRED);
                else
                    ChangeTankColor(newTank, materalBLUE);
                BlueAgentsList.Add(newTank);
                AgentsList.Add(newTank);
            }

        }

        // Spawn Team Red tanks(规则方或者学习方)
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

    //修改tank属性，进而修改颜色
    public void ChangeTankColor(TankControl tank, Material mateial)
    {
        // 使用Transform.Find找到TankRenderers/TankFree_Body
        Transform TankRenderTransform = tank.transform.Find("TankRenderers");
        // 从TankFree_Body获取MeshRenderer
        foreach (var renderer in TankRenderTransform.GetComponentsInChildren<MeshRenderer>())
        {
            // 设置新的材质
            renderer.material = mateial;
        }
    }


    public void ChangeManColor(ManControl tank, Material mateial)
    {
        // 使用Transform.Find找到TankRenderers/TankFree_Body
        Transform TankRenderTransform = tank.transform.Find("TankRenderers");
        // 从TankFree_Body获取MeshRenderer
        foreach (var renderer in TankRenderTransform.GetComponentsInChildren<MeshRenderer>())
        {
            // 设置新的材质
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
