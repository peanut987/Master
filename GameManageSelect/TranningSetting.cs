using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranningSetting : MonoBehaviour
{


    [System.Serializable]
    public class Envinfo
    {
        /*
       * ���²������÷����ڵ���ز���
       */
        public float explosionRadius = 2;
        public float MaxDamage = 15;
        public int offet = 5;

        [Tooltip("�����λ�����ɸ����ʱ�򣬱�֤�������distanceFromEach�ľ���")]
        public int distanceFromEach = 15;
        public int TankCreateRange = 100;

        /*
        * ���²�������gamemanager��ز���
        */

        [Tooltip("Max Environment Steps")]
        public int m_ResetTimer = 20000;

        [Tooltip("�Ƿ�ʹ�����λ�ã�1��ʾ���λ�� 2��ʾ�̶�λ�ã�3��ʾ80%ʹ�����λ�ã�Ŀǰѵ�������г���ֻʹ�ù̶�λ�ã���������ʹ�����λ��")]
        public int UseRandomPos = 1;
        public float ViewDis = 999;

        [Tooltip("�Ƿ��ǳ��л�������Ҫ�ж� ʹ���ĸ������������߼�")]
        public bool is_City = false;

        [Tooltip("�Ƿ���ӻ���ʾ�Ծ��������ѵ��ʱ���Թرգ���Ϊһֱ�������ֿ��ܻ�Ӱ��ѵ��")]
        public bool Show_Mode;

        [Tooltip("�Ƿ�ʹ��3VN ģʽ����ģʽ�£�3�ĸ���ֻ����30��ά�ȵ���Ϣ����")]
        public bool is3VN = false;

        public bool allview = false;

        [Tooltip("�Ƿ�ʹ��λ���л�ģʽ����ģʽ�£�ÿ�ֻᰴ˳ʱ���л�̹�˳�ʼλ��")]
        public bool isChangePosition = true;

        [Tooltip("�Ƿ�����ѵ������ģʽ�£��Ծִ�������������100��")]
        public bool isTrainning = true;

    }

    [System.Serializable]
    public class AlgorithmSelect
    {
        [Tooltip("�Ƿ�ѡ�������������㷨")]
        public bool BioGeneral = false;

        [Tooltip("�Ƿ�ѡ����������Ż��㷨")]
        public bool BioOptimized = false;

        [Tooltip("�Ƿ�ѡ�����Ķ��վͽ���������㷨")]
        public bool NRStandard = false;

        [Tooltip("�Ƿ�ѡ�����Ķ���ǿ��ѧϰ�㷨")]
        public bool RLStandard = false;
    }

    [System.Serializable]
    public class TeamInfo
    {
        [Tooltip("�����������")]
        public int nums;

        [Tooltip("������ɫ����")]
        public Material Matera;

        [Tooltip("����Ѫ��")]
        public Color Slidercolor;

        [Tooltip("����ѵ�������и���Ѫ����")]
        public float FULLPH;

        public int Clisize = 40;


        /*
        * ���²�������tankcontrol��ز���
        */

        [Tooltip("�ж�ʹ����ɢ�ƶ���Ϣor�����ƶ���Ϣ��" +
            "true ��ʾ��ɢ�ƶ���Ϣ����ʱ��Ҫ��continus action ����-2 " +
            "discrete action����+1����������discrete action��branchΪ 5")]
        public bool Usediscrete = true;
        public float MaxSpeed = 30;
        public float rotatespeed = 5;
        public int cooldowntime = 80;
        public int Fireinterval = 0;
        public int Standardinterval = 600;

        [Tooltip("ѡ���Ƿ�ʹ�þɵĹ۲�ֵ��" +
        "true ��ʾʹ�þɵĹ۲���Ϣ���۲�з�ά��Ϊ4����ʱBehavior Parameters��space size ��СΪ��3������۲���Ϣ�� + 4���з���Ϣά�ȣ�* �з����������� 3V3�У���ֵ����Ϊ 15. " +
        "false ��ʾʹ���µĹ۲���Ϣ���۲�з�ά��Ϊ9 ��ʱBehavior Parameters��space size ��СΪ��3������۲���Ϣ�� + 9���з���Ϣά�ȣ�* �з����������� 3V3�У���ֵ����Ϊ 30.")]
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
