using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranningSetting : MonoBehaviour
{


    [System.Serializable]
    public class Envinfo
    {
        /*
       * 以下部分配置发射炮弹相关参数
       */
        public float explosionRadius = 2;
        public float MaxDamage = 15;
        public int offet = 5;

        [Tooltip("在随机位置生成个体的时候，保证个体间间隔distanceFromEach的距离")]
        public int distanceFromEach = 15;
        public int TankCreateRange = 100;

        /*
        * 以下部分配置gamemanager相关参数
        */

        [Tooltip("Max Environment Steps")]
        public int m_ResetTimer = 20000;

        [Tooltip("是否使用随机位置，1表示随机位置 2表示固定位置，3表示80%使用随机位置，目前训练过程中城市只使用固定位置，其他场景使用随机位置")]
        public int UseRandomPos = 1;
        public float ViewDis = 999;

        [Tooltip("是否是城市环境，主要判断 使用哪个环境的生成逻辑")]
        public bool is_City = false;

        [Tooltip("是否可视化显示对局情况，在训练时可以关闭，因为一直更新文字可能会影响训练")]
        public bool Show_Mode;

        [Tooltip("是否使用3VN 模式，此模式下，3的个体只接受30个维度的信息输入")]
        public bool is3VN = false;

        public bool allview = false;

        [Tooltip("是否使用位置切换模式，此模式下，每轮会按顺时针切换坦克初始位置")]
        public bool isChangePosition = true;

        [Tooltip("是否用于训练，此模式下，对局次数不会限制在100局")]
        public bool isTrainning = true;

    }

    [System.Serializable]
    public class AlgorithmSelect
    {
        [Tooltip("是否选择仿生规则基础算法")]
        public bool BioGeneral = false;

        [Tooltip("是否选择仿生规则优化算法")]
        public bool BioOptimized = false;

        [Tooltip("是否选择论文对照就近分配对手算法")]
        public bool NRStandard = false;

        [Tooltip("是否选择论文对照强化学习算法")]
        public bool RLStandard = false;
    }

    [System.Serializable]
    public class TeamInfo
    {
        [Tooltip("队伍个体数量")]
        public int nums;

        [Tooltip("队伍颜色材质")]
        public Material Matera;

        [Tooltip("队伍血条")]
        public Color Slidercolor;

        [Tooltip("配置训练环境中个体血量和")]
        public float FULLPH;

        public int Clisize = 40;


        /*
        * 以下部分配置tankcontrol相关参数
        */

        [Tooltip("判断使用离散移动信息or连续移动信息，" +
            "true 表示离散移动信息，此时需要将continus action 个数-2 " +
            "discrete action个数+1，设置增加discrete action的branch为 5")]
        public bool Usediscrete = true;
        public float MaxSpeed = 30;
        public float rotatespeed = 5;
        public int cooldowntime = 80;
        public int Fireinterval = 0;
        public int Standardinterval = 600;

        [Tooltip("选择是否使用旧的观测值，" +
        "true 表示使用旧的观测信息，观测敌方维度为4，此时Behavior Parameters中space size 大小为：3（自身观测信息） + 4（敌方信息维度）* 敌方个数，例如 3V3中，该值设置为 15. " +
        "false 表示使用新的观测信息，观测敌方维度为9 此时Behavior Parameters中space size 大小为：3（自身观测信息） + 9（敌方信息维度）* 敌方个数，例如 3V3中，该值设置为 30.")]
        public bool UseOldObserve = true;

        public string BehaviorName = "TANK";

        public bool HumanControl = false;
    }

    public AlgorithmSelect algorithmSelect;
    public TeamInfo RedTeam;
    public TeamInfo BlueTeam;
    public Envinfo EnvInfo;
    public ParticleSystem tankExplosion;

}
