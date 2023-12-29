using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using Unity.Mathematics;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.AI;
using System.Diagnostics;
using System.IO;
using static TankControl;
using UnityEngine.UIElements;
using Unity.VisualScripting;



public class GameManage : MonoBehaviour
{
    private int distanceFromEach;

    //���ýű��ļ�
    public TankSpawner tankSpawner;
    public FindEnemy findEnemy;
    public SceneSwitcher sceneSwitcher;
    //д�������ļ�
    private TranningSetting TranningSetting;

    [Tooltip("Environment Steps left")]
    public int left_Step;

    private int MaxEnvironmentSteps;


    [Tooltip("�Ƿ�ʹ�����λ�ã�1��ʾ���λ�� 2��ʾ�̶�λ�ã�3��ʾ50%ʹ�����λ��")]
    private int UseRandomPos = 1;

    [Tooltip("�Ƿ�ʹ��λ���л�ģʽ����ģʽ�£�ÿ�ֻᰴ˳ʱ���л�̹�˳�ʼλ��")]
    public bool isChangePosition = true;

    public List<TextMeshProUGUI> TextList = new();//����������ʾ�б�
    public GameObject[] shell_all;//�洢�ڵ����Ծֽ���ʱ�����ڵ�ȫ������

    //Ⱥ�影��
    public SimpleMultiAgentGroup m_BlueAgentGroup;
    public SimpleMultiAgentGroup m_RedAgentGroup;

    //���Ѫ������ǰΪһ�ڻ���
    private float FULLPH_RED;
    private float FULLPH_BLUE;

    //���ڴ������
    public int num_Blue = 0;
    public int num_Red = 0;

    //�Ծ�˫������
    private int SUM_RED;
    private int SUM_BLUE;

    //�Ծ�����
    public int round = 1;
    public int RecordRounds = 100;

    public float eff;

    public bool isRecord;

    //�Ծ�ʤ��ͳ��
    public int Blue_win = 0;
    public int Red_win = 0;
    public int Both_win = 0;
    public int Time_Out = 0;

    public GameObject initialPos;

    private float PH_Red = 0;
    private float PH_Blue = 0;
    private float RedScore = 0;
    private float BlueScore = 0;
    private string currentButton;//��ǰ���µİ���
    public int m_RedAgentGroup_num = 0;
    public int m_BlueAgentGroup_num = 0;
    private bool Show_Mode;
    private bool is_City = false;
    public bool isqiuling = true;
    private float randomy = 0;

    private int TankCreateRange; //Tank �������λ�÷�Χ
    private List<Vector3[]> spawnLocations = new List<Vector3[]>();
    List<Vector3> spawnPositions = new List<Vector3>();

    //�Ծ�λ�������б�
    public List<Vector3> BornPointRed = new List<Vector3>();
    public List<Vector3> BornPointBlue = new List<Vector3>();

    private List<int> Priority = new List<int>();
    private Dictionary<int, List<float>> agentRewards = new();
    private Transform Text;
    private float[] TextScale = { 1.5f, 1, 3.5f, 1, 2 };//�����ͼ��Ӧ���ı����ɴ�С
    
    public float Righttime = 30;//���ڼ�ʱ
    public int limitWaitTime = 500;//�Ծֽ�������ʱʱ�䣬���ڹ۲�ʤ��
    public bool is_training = true;

    public class MapInfo
    {
        public Vector3 topLeft;
        public Vector3 bottomRight;
    }

    public Dictionary<int, MapInfo> mapData = new Dictionary<int, MapInfo>();
    public GameObject[] scenePrefabs; // ��ͼ��Prefab����
    public int MapIndex = -1;
    public float BIO_PH_Loss;
    public float BIO_PH_Cacul_Loss;
    public int BIO_Dead_Num;
    public int BIO_Dead_Cacul_Num;

    void Start()
    {
        spawnLocations.Add(new Vector3[] { new Vector3(-678.0f, 0.0f, -550.0f), new Vector3(-710.0f, 0.0f, 450.0f) });
        spawnLocations.Add(new Vector3[] { new Vector3(-710.0f, 0.0f, 535.0f), new Vector3(610.0f, 0.0f, 585.0f) });
        spawnLocations.Add(new Vector3[] { new Vector3(810.0f, 0.0f, -455.0f), new Vector3(-650.0f, 0.0f, -590.0f) });
        spawnLocations.Add(new Vector3[] { new Vector3(260.0f, 0.0f, -500.0f), new Vector3(-92.0f, 0.0f, 403.0f) });
        spawnLocations.Add(new Vector3[] { new Vector3(630.0f, 0.0f, 570.0f), new Vector3(831.0f, 0.0f, -450.0f) });
        spawnLocations.Add(new Vector3[] { new Vector3(-92.0f, 0.0f, 403.0f), new Vector3(412.0f, 0.0f, 430.0f) });
        spawnLocations.Add(new Vector3[] { new Vector3(-8f, 0.0f, 105.0f), new Vector3(-50f, 0.0f, 109.0f) });
        spawnLocations.Add(new Vector3[] { new Vector3(124f, 0.0f, -190.0f), new Vector3(-89.0f, 0.0f, -205.0f) });
        spawnLocations.Add(new Vector3[] { new Vector3(250.0f, 0.0f, -295.0f), new Vector3(397.0f, 0.0f, -299.0f) });

        //��ʼ������
        TranningSetting = FindObjectOfType<TranningSetting>();
        findEnemy = FindObjectOfType<FindEnemy>();
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();
        
        //����ҵ������ļ�
        if (TranningSetting)
        {
            UseRandomPos = TranningSetting.EnvInfo.UseRandomPos;
            isChangePosition = TranningSetting.EnvInfo.isChangePosition;
            Show_Mode = TranningSetting.EnvInfo.Show_Mode;
            MaxEnvironmentSteps = TranningSetting.EnvInfo.m_ResetTimer;
            distanceFromEach = TranningSetting.EnvInfo.distanceFromEach;
            TankCreateRange = TranningSetting.EnvInfo.TankCreateRange;
            is_City = TranningSetting.EnvInfo.is_City;
            SUM_BLUE = TranningSetting.BlueTeam.nums;
            FULLPH_BLUE = TranningSetting.BlueTeam.FULLPH;

            FULLPH_RED = TranningSetting.RedTeam.FULLPH;
            SUM_RED = TranningSetting.RedTeam.nums;

        }

        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_RedAgentGroup = new SimpleMultiAgentGroup();
        //tankSpawner.SpawnTanks();
        left_Step = MaxEnvironmentSteps;
        //��ʼ����������ĸߴ�����
        Transform BornPoint = GameObject.Find("blue").transform;
        for (int i = 0; i < BornPoint.childCount; i++)
        {
            BornPointBlue[i] = BornPoint.GetChild(i).transform.position;
        }

        for (int i = 0; i < BornPoint.childCount; i++)
        {
            int index = i >= 2 ? i - 2 : i + 2;
            BornPointRed[i] = BornPointBlue[index];
        }
        Transform Text = GameObject.Find("Text").transform;
        for (int i = 0; i < Text.childCount; i++)
        {
            TextList[i] = Text.GetChild(i).GetComponent<TextMeshProUGUI>();
        }
        UpdateTitle(SUM_RED, SUM_BLUE);
        //UnityEngine.Debug.Log("game tankSpawner.Biolist" + tankSpawner.Biolist.Count);
        //InitMapData();
        setTimeScale(SUM_RED, SUM_BLUE);
        ResetScene();

    }

    void Update()
    {
        Righttime -= Time.deltaTime;
    }
    void FixedUpdate()
    {
        if (round >= 101)
        {
            if(isRecord)
                RecordMode();//ѭ������
            else 
                Time.timeScale = 0;
        }
        left_Step -= 1;
        RedScore = 0;
        BlueScore = 0;

        //�ж��Ƿ���һ��ʤ��
        Whether_win();

        //�����ʱ
        if (left_Step <= 0)
        {
            ++Time_Out;
            m_BlueAgentGroup.AddGroupReward(-0.2f);
            m_RedAgentGroup.AddGroupReward(-0.2f);
            m_RedAgentGroup.GroupEpisodeInterrupted();
            m_BlueAgentGroup.GroupEpisodeInterrupted();

            //����AgentList��ÿ����������Ϊfalse
            foreach (var item in tankSpawner.AgentsList)
            {
                if (item.isActiveAndEnabled)
                {
                    item.gameObject.SetActive(false);
                }
            }
            num_Red = 0;
            num_Blue = 0;


            ResetScene();
        }

        if (!GameObject.Find("TankSpawner").GetComponent<TankSpawner>().useBio)
        {
            foreach (var item in tankSpawner.AgentsList)
            {
                if (item.tankTeam == TankTeam.Tank_Red)
                    RedScore += item.GetCumulativeReward();
                else
                    BlueScore += item.GetCumulativeReward();

                item.Enemydir.Clear();
                item.Enemydir1.Clear();
            }
        }
        else
        {
            foreach (var item in tankSpawner.BlueAgentsList)
            {

                BlueScore += item.GetCumulativeReward();
                item.EnemyBiodir.Clear();
                item.EnemyBiodir1.Clear();
            }

            foreach (var item in tankSpawner.Biolist)
            {
                item.BioEnemydir.Clear();
                item.BioEnemydir1.Clear();

                item.BioEnemydirTA.Clear();
            }

            foreach (var item in tankSpawner.TAList)
            {
                item.TAEnemydir.Clear();
                item.BioEnemydir1.Clear();
            }
        }
        CalcDistance();


        UpdateText();


    }


    public void ResetScene()
    {
        ++round;
        left_Step = MaxEnvironmentSteps;
        shell_all = GameObject.FindGameObjectsWithTag("Shell");
        Righttime = 30;
        foreach (var item in shell_all)
        {
            GameObject.Destroy(item);
        }
        num_Red = SUM_RED;
        num_Blue = SUM_BLUE;
        int BlueCount = 0;
        int RedCount = 0;

        bool israndom = true;
        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
        {
            israndom = false;
        }

        foreach (var item in tankSpawner.AgentsList)
        {
            if (item.GetComponent<TankControl>().tankTeam == TankTeam.Tank_Blue)
            {
                BlueCount++;
            }
            else
            {
                RedCount++;
            }
            item.gameObject.SetActive(true);
            item.Enemydir.Clear();
            //����������λ��
            //��ͼ��Χ�Ƚϴ���δ����̶�λ�ô���

            //ģʽ1 ʹ�����λ��
            if (UseRandomPos == 1)
            {
                MoveToSafeRandomPosition(item);
            }
            else if (UseRandomPos == 2)    //ģʽ2 ʹ�ù̶�λ��
            {
                item.transform.position = setPosition(round, item.TankNum, item.tankTeam, isChangePosition, BornPointRed, BornPointBlue, 40, 40)[0];
                item.transform.forward = setPosition(round, item.TankNum, item.tankTeam, isChangePosition, BornPointRed, BornPointBlue)[1];

            }
            else                          //ģʽ3 70%���λ��+30%�̶�λ��
            {
                if (israndom)
                {
                    MoveToSafeRandomPosition(item);
                }
                else
                {
                    item.transform.position = setPosition(round, item.TankNum, item.tankTeam, isChangePosition, BornPointRed, BornPointBlue)[0];
                    item.transform.forward = setPosition(round, item.TankNum, item.tankTeam, isChangePosition, BornPointRed, BornPointBlue)[1];
                }
            }

            //������ע�����
            if (item.tankTeam == TankTeam.Tank_Red)
            {
                m_RedAgentGroup.RegisterAgent(item);
                //����Ѫ��
                item.PH = FULLPH_RED;
                item.phSlider.maxValue = FULLPH_RED;
                item.phSlider.value = FULLPH_RED;
                item.Isdead = false;
                item.Clisize = 40;
                item.Fireinterval = 0;
            }

            else
            {
                m_BlueAgentGroup.RegisterAgent(item);

                //��ʼ��Ѫ��
                if (TranningSetting.RedTeam.nums == 5 && TranningSetting.BlueTeam.nums == 3)
                {
                    item.PH = TranningSetting.BlueTeam.FULLPH * 2;
                }
                else if (TranningSetting.RedTeam.nums == 3 && TranningSetting.BlueTeam.nums == 4)
                {
                    item.PH = TranningSetting.BlueTeam.FULLPH * 2;
                }
                else if (TranningSetting.RedTeam.nums == 4 && TranningSetting.BlueTeam.nums == 3)
                {
                    item.PH = TranningSetting.BlueTeam.FULLPH * 3;
                }
                else
                {
                    item.PH = TranningSetting.BlueTeam.FULLPH;
                }
                item.phSlider.maxValue = FULLPH_BLUE;
                item.phSlider.value = FULLPH_BLUE;
                item.Isdead = false;
                item.Clisize = 40;
                item.Fireinterval = 0;
                item.NUM_Text.color = TranningSetting.BlueTeam.Slidercolor;
                if (TranningSetting.RedTeam.HumanControl)
                {
                    item.NUM_Text.color = TranningSetting.RedTeam.Slidercolor;
                    tankSpawner.ChangeTankColor(item, TranningSetting.RedTeam.Matera);
                }
                else
                {
                    tankSpawner.ChangeTankColor(item, TranningSetting.BlueTeam.Matera);
                    item.NUM_Text.color = TranningSetting.BlueTeam.Slidercolor;
                }
            }


        }
        foreach (var item1 in tankSpawner.Biolist)
        {
            RedCount++;
        }

        if (GameObject.Find("TankSpawner").GetComponent<TankSpawner>().useBio)
        {
            foreach (var item in tankSpawner.Biolist)
            {
                bool safePositionFound = false;
                int attemptsRemaining = 100;
                Vector3 potentialPosition = Vector3.zero;
                Quaternion potentialRotation = new();
                if (UseRandomPos == 1)
                {
                    while (!safePositionFound && attemptsRemaining > 0)
                    {
                        attemptsRemaining--;
                        potentialPosition = new Vector3(
                        UnityEngine.Random.Range(-100, 1000),
                        60,
                        UnityEngine.Random.Range(-100, 1000));

                        //�������ù�λ�õ�������
                        foreach (var item1 in tankSpawner.Biolist)
                        {
                            if (item1 != item)
                            {
                                if (Vector3.Distance(potentialPosition, item1.transform.position) <= distanceFromEach)
                                {
                                    //Debug.Log(SetTank.name + " : " + potentialPosition + "-" + item.tank.name + " " + item.tank.transform.position + " dis : " + Vector3.Distance(potentialPosition, item.tank.transform.position));                       
                                    break;
                                }
                            }
                            else
                            {
                                safePositionFound = true;
                                break;
                            }
                        }
                    }

                    potentialRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f);
                    item.transform.SetPositionAndRotation(potentialPosition + initialPos.transform.position, potentialRotation);
                }
                else if (UseRandomPos == 2)
                {
                    item.gameObject.SetActive(true);
                    item.transform.position = setPosition(round, item.TankNum, item.tankTeam, isChangePosition, BornPointRed, BornPointBlue)[0];
                    item.transform.forward = setPosition(round, item.TankNum, item.tankTeam, isChangePosition, BornPointRed, BornPointBlue)[1];
                }

                //��ʼ��Ѫ��
                if (TranningSetting.RedTeam.nums == 3 && TranningSetting.BlueTeam.nums == 5)
                {
                    item.PH = TranningSetting.RedTeam.FULLPH * 2;
                }
                else if (TranningSetting.RedTeam.nums == 3 && TranningSetting.BlueTeam.nums == 4)
                {
                    item.PH = TranningSetting.RedTeam.FULLPH * 3;
                }
                else if (TranningSetting.RedTeam.nums == 4 && TranningSetting.BlueTeam.nums == 3)
                {
                    item.PH = TranningSetting.RedTeam.FULLPH * 2;
                }
                else
                {
                    item.PH = TranningSetting.RedTeam.FULLPH;
                }
                item.PHSlider.maxValue = FULLPH_RED;
                item.PHSlider.value = FULLPH_RED;
                //item.line.enabled = false;
                item.CountMinNum = 0;
                item.timeBetween = 0;
                item.startFlag = 0;
                item.MinNum = -1;
                item.HillIndex = -1;
                item.isNavigate = true;
                item.Isdead = false;
                item.stopTime = 0;
                item.rotateFlag = -1;
                item.firetime = 300;
                item.relativespeed = 1.0f;
                item.fire = 0;

                if (TranningSetting.RedTeam.HumanControl)
                {
                    item.NUM_Text.color = TranningSetting.BlueTeam.Slidercolor;
                    tankSpawner.ChangeManColor(item, TranningSetting.BlueTeam.Matera);
                }
                else
                {
                    item.NUM_Text.color = TranningSetting.RedTeam.Slidercolor;
                    tankSpawner.ChangeManColor(item, TranningSetting.RedTeam.Matera);
                }

                //item.Clisize = 40;

            }

            if (tankSpawner.useTA)
            {
                foreach (var item in tankSpawner.TAList)
                {
                    item.gameObject.SetActive(true);
                    item.transform.position = setPosition(round, item.TankNum, item.tankTeam, isChangePosition, BornPointRed, BornPointBlue)[0];
                    item.transform.forward = setPosition(round, item.TankNum, item.tankTeam, isChangePosition, BornPointRed, BornPointBlue)[1];

                    //��ʼ��Ѫ��
                    if (TranningSetting.RedTeam.nums == 5 && TranningSetting.BlueTeam.nums == 3)
                    {
                        item.PH = TranningSetting.BlueTeam.FULLPH * 2;
                    }
                    else if (TranningSetting.RedTeam.nums == 3 && TranningSetting.BlueTeam.nums == 4)
                    {
                        item.PH = TranningSetting.BlueTeam.FULLPH * 2;
                    }
                    else if (TranningSetting.RedTeam.nums == 4 && TranningSetting.BlueTeam.nums == 3)
                    {
                        item.PH = TranningSetting.BlueTeam.FULLPH * 3;
                    }
                    else
                    {
                        item.PH = TranningSetting.BlueTeam.FULLPH;
                    }
                    item.PHSlider.maxValue = FULLPH_RED;
                    item.PHSlider.value = FULLPH_RED;
                    //item.line.enabled = false;
                    item.CountMinNum = 0;
                    item.timeBetween = 0;
                    item.startFlag = 0;
                    item.MinNum = -1;
                    item.HillIndex = -1;
                    item.isNavigate = true;
                    item.Isdead = false;
                    item.fire = 0;
                    item.assignFlag = false;
                    if (TranningSetting.RedTeam.HumanControl)
                    {
                        item.NUM_Text.color = TranningSetting.RedTeam.Slidercolor;
                        tankSpawner.ChangeManColor(item, TranningSetting.RedTeam.Matera);
                    }
                    else
                    {
                        tankSpawner.ChangeManColor(item, TranningSetting.BlueTeam.Matera);
                        item.NUM_Text.color = TranningSetting.BlueTeam.Slidercolor;
                    }
                }
            }
        }

    }

    public void setTimeScale(int redNum, int blueNum)
    {
        if (redNum == 5 && blueNum == 5) Time.timeScale = 18;
        else Time.timeScale = 18;
    }

    public void RecordMode()
    {
        ManControl man = FindObjectOfType<ManControl>();
        if (isRecord)
        {
            if (round > RecordRounds)
            {
                UnityEngine.Debug.Log(" Rounds: " + (round - 1) + " Blue_win: " + Blue_win + " Red_win: " + Red_win + "  " + ((float)Red_win / (Red_win + Blue_win + Both_win)).ToString("f3") + "% " + "NUM_BIO: " 
                    + TranningSetting.RedTeam.nums + " NUM_RL: " + TranningSetting.BlueTeam.nums + " eff: " + eff);
                //�Ծ�����ʱ���ݳ�ʼ��
                round = 1;
                Blue_win = 0;
                Red_win = 0;
                Both_win = 0;
                BIO_PH_Loss = 0;
                BIO_PH_Cacul_Loss = 0;
                BIO_Dead_Num = 0;
                BIO_Dead_Cacul_Num = 0;

                SUM_RED = TranningSetting.RedTeam.nums;
                SUM_BLUE = TranningSetting.BlueTeam.nums;
                //Time.timeScale = 0;//������100�غ���ͣ
                if (TranningSetting.RedTeam.nums == 5)
                {
                    if (TranningSetting.BlueTeam.nums == 5)
                    {
                        TranningSetting.BlueTeam.nums = 3;
                    }
                    else
                    {
                        TranningSetting.RedTeam.nums = 4;
                    }
                }
                else if (TranningSetting.RedTeam.nums == 4)
                {
                    if (TranningSetting.BlueTeam.nums == 3)
                    {
                        TranningSetting.RedTeam.nums = 3;
                        TranningSetting.BlueTeam.nums = 5;
                    }
                }
                else if (TranningSetting.RedTeam.nums == 3)
                {
                    if (TranningSetting.BlueTeam.nums == 5)
                    {
                        TranningSetting.BlueTeam.nums = 4;
                    }
                    else if (TranningSetting.BlueTeam.nums == 4)
                    {
                        TranningSetting.BlueTeam.nums = 3;
                    }
                    else if (TranningSetting.BlueTeam.nums == 3)
                    {
                        TranningSetting.RedTeam.nums = 5;
                        TranningSetting.BlueTeam.nums = 5;
                    }

                }
                foreach (var item in tankSpawner.Biolist)
                {
                    if(item.TankNum > TranningSetting.RedTeam.nums) item.gameObject.SetActive(false);
                    else item.gameObject.SetActive(true);
                }
                foreach (var item in tankSpawner.TAList)
                {
                    if (item.TankNum + 1 > TranningSetting.BlueTeam.nums) item.gameObject.SetActive(false);
                    else item.gameObject.SetActive(true);
                }

                man.cannon_script2.setRadius(TranningSetting.RedTeam.nums, TranningSetting.BlueTeam.nums, tankSpawner.useTA, TranningSetting.algorithmSelect.BioOptimized);
                man.findEnemy2.setDis(TranningSetting.RedTeam.nums, TranningSetting.BlueTeam.nums, tankSpawner.useTA, TranningSetting.algorithmSelect.BioOptimized);
                man.obstacleAvoid2.setParameter(TranningSetting.RedTeam.nums, TranningSetting.BlueTeam.nums, tankSpawner.useTA, TranningSetting.algorithmSelect.BioOptimized);
                
            }
        }
    }

    public Vector3[] setPosition(int rounds, int tankNum, TankTeam tankTeam, bool changePos, List<Vector3> BornPointRed, List<Vector3> BornPointBlue, int scale1 = 60, int scale2 = 60, int size = 5)
    {
        Vector3[] result = new Vector3[2];
        int bornPointIndex = 0;
        if (changePos)
            bornPointIndex = rounds % 4 - 1 == -1 ? 3 : rounds % 4 - 1;
        else
            bornPointIndex = 0;

        int isLessThan0 = (bornPointIndex == 0 || bornPointIndex == 3) ? 1 : -1;
        int isLessThan1 = (bornPointIndex == 0 || bornPointIndex == 1) ? -1 : 1;
        if (tankTeam == 0)
        {
            result[0] = new Vector3(BornPointRed[bornPointIndex].x + isLessThan1 * scale1 * (int)((tankNum - 1) % size) - isLessThan0 * scale2 * (int)((tankNum - 1) / size), BornPointRed[bornPointIndex].y + 5, BornPointRed[bornPointIndex].z - isLessThan0 * scale1 * ((tankNum - 1) % size - 1) - isLessThan1 * scale2 * (int)((tankNum - 1) / size));
            result[1] = new Vector3(isLessThan0, 0, isLessThan1);
        }
        else
        {
            isLessThan0 = -isLessThan0;
            isLessThan1 = -isLessThan1;
            result[0] = new Vector3(BornPointBlue[bornPointIndex].x + isLessThan1 * scale1 * (int)((tankNum - 1) % size) - isLessThan0 * scale2 * (int)((tankNum - 1) / size), BornPointBlue[bornPointIndex].y + 5, BornPointBlue[bornPointIndex].z - isLessThan0 * scale1 * ((tankNum - 1) % size - 1) - isLessThan1 * scale2 * (int)((tankNum - 1) / size));
            result[1] = new Vector3(isLessThan0, 0, isLessThan1);
        }


        return result;
    }

    //�������λ��,�ú������ڱ�֤����tank���һ������
    //attemptsRemaining ��ʾ���ԵĴ�����DIS��ʾ���ɾ���˳��Ĭ��30
    //�����������ѡ�������ڳ��л���������Ҫ�̶�λ�����ɡ������ƽԭ����������Ҫ�����λ�����ɡ�������Ҫ����ѡ��
    public void MoveToSafeRandomPosition(TankControl SetTank)
    {
        bool safePositionFound = false;
        int attemptsRemaining = 100;
        if (is_City)
        {

            spawnPositions.Clear();
            foreach (var item in tankSpawner.AgentsList)
            {

                item.gameObject.SetActive(true);
                item.Enemydir.Clear();
                //var newStartPos = new Vector3(-695.0f + (item.TankNum - 1) * -0.1f, 0.0f, -550.0f + (item.TankNum - 1) * 50.0f);
                var newStartPos = GenerateSpawnPoint();
                //var newRot = Quaternion.Euler(0, -5.0f, 0);
                var newRot = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360.0f), 0);
                item.transform.SetPositionAndRotation(newStartPos, newRot);

            }
        }


        //�ǳ���λ�ã�ֱ�����ô�С
        else
        {
            if (isqiuling)
            {
                randomy = -85;
            }
            else
            {
                randomy = -0.25f;
            }
            Vector3 potentialPosition = Vector3.zero;
            Quaternion potentialRotation = new();

            while (!safePositionFound && attemptsRemaining > 0)
            {
                attemptsRemaining--;
                potentialPosition = new Vector3(
                UnityEngine.Random.Range(-100, 1000),
                60,
                UnityEngine.Random.Range(-100, 1000));

                //�������ù�λ�õ�������
                foreach (var item in tankSpawner.AgentsList)
                {
                    if (SetTank != item)
                    {
                        if (Vector3.Distance(potentialPosition, item.transform.position) <= distanceFromEach)
                        {
                            //Debug.Log(SetTank.name + " : " + potentialPosition + "-" + item.tank.name + " " + item.tank.transform.position + " dis : " + Vector3.Distance(potentialPosition, item.tank.transform.position));                       
                            break;
                        }
                    }
                    else
                    {
                        safePositionFound = true;
                        break;
                    }
                }
            }

            //��������Ƕ��Լ�λ��
            potentialRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f);
            SetTank.transform.SetPositionAndRotation(potentialPosition + initialPos.transform.position, potentialRotation);
        }
    }

    //�̶�����λ��
    public void fixedPosition(TankControl SetTank, bool ischangeposition, int Bluecount = -1, int Redcount = -1)
    {
        if (is_City)
        {
            Vector3 currentPositionBlue = BornPointBlue[0];
            Vector3 currentPositionRed = BornPointRed[0];
            //
            if (SetTank.GetComponent<TankControl>().tankTeam == TankTeam.Tank_Blue)
            {
                SetTank.transform.position = new Vector3(currentPositionBlue.x - 2 * Bluecount, currentPositionBlue.y, currentPositionBlue.z + 10 * Bluecount);
            }
            else
            {
                SetTank.transform.position = new Vector3(currentPositionRed.x - Redcount, currentPositionRed.y, currentPositionRed.z + 10 * Redcount);
            }
        }
        else
        {

            Vector3 currentPositionBlue = BornPointBlue[0];
            Vector3 currentPositionRed = BornPointRed[0];
            Vector3 currentPositionBlue1 = BornPointBlue[1];
            Vector3 currentPositionBlue2 = BornPointBlue[2];
            Vector3 currentPositionBlue3 = BornPointBlue[3];

            if (ischangeposition)
            {
                switch (round % 4 - 1)
                {
                    case 0:
                        switch (SetTank.TankNum % 5)
                        {
                            case 0:
                                SetTank.transform.position = new Vector3(currentPositionBlue.x, currentPositionBlue.y, currentPositionBlue.z + 50 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(-1, 0, 1);
                                break;
                            case 1:
                                SetTank.transform.position = new Vector3(currentPositionBlue.x + 60, currentPositionBlue.y, currentPositionBlue.z + 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(-1, 0, 1);
                                break;
                            case 2:
                                SetTank.transform.position = new Vector3(currentPositionBlue.x + 120, currentPositionBlue.y, currentPositionBlue.z + 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(-1, 0, 1);
                                break;
                            case 3:
                                SetTank.transform.position = new Vector3(currentPositionBlue.x + 180, currentPositionBlue.y, currentPositionBlue.z + 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(-1, 0, 1);
                                break;
                            case 4:
                                SetTank.transform.position = new Vector3(currentPositionBlue.x + 240, currentPositionBlue.y, currentPositionBlue.z + 20 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(-1, 0, 1);
                                break;
                        }
                        break;
                    case 1:
                        switch (SetTank.TankNum % 5)
                        {
                            case 0:
                                SetTank.transform.position = new Vector3(currentPositionBlue1.x, currentPositionBlue1.y, currentPositionBlue1.z + 50 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(1, 0, 1);
                                break;
                            case 1:
                                SetTank.transform.position = new Vector3(currentPositionBlue1.x - 60, currentPositionBlue1.y, currentPositionBlue1.z + 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(1, 0, 1);
                                break;
                            case 2:
                                SetTank.transform.position = new Vector3(currentPositionBlue1.x - 120, currentPositionBlue1.y, currentPositionBlue1.z + 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(1, 0, 1);
                                break;
                            case 3:
                                SetTank.transform.position = new Vector3(currentPositionBlue1.x - 180, currentPositionBlue1.y, currentPositionBlue1.z + 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(1, 0, 1);
                                break;
                            case 4:
                                SetTank.transform.position = new Vector3(currentPositionBlue1.x - 240, currentPositionBlue1.y, currentPositionBlue1.z + 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(1, 0, 1);
                                break;
                        }
                        break;
                    case 2:
                        switch (SetTank.TankNum % 5)
                        {
                            case 0:
                                SetTank.transform.position = new Vector3(currentPositionBlue2.x, currentPositionBlue2.y, currentPositionBlue2.z - 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(1, 0, -1);
                                break;
                            case 1:
                                SetTank.transform.position = new Vector3(currentPositionBlue2.x - 60, currentPositionBlue2.y, currentPositionBlue2.z - 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(1, 0, -1);
                                break;
                            case 2:
                                SetTank.transform.position = new Vector3(currentPositionBlue2.x - 120, currentPositionBlue2.y, currentPositionBlue2.z - 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(1, 0, -1);
                                break;
                            case 3:
                                SetTank.transform.position = new Vector3(currentPositionBlue2.x - 180, currentPositionBlue2.y, currentPositionBlue2.z - 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(1, 0, -1);
                                break;
                            case 4:
                                SetTank.transform.position = new Vector3(currentPositionBlue2.x - 240, currentPositionBlue2.y, currentPositionBlue2.z - 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(1, 0, -1);
                                break;
                        }
                        break;
                    case -1:
                        switch (SetTank.TankNum % 5)
                        {
                            case 0:
                                SetTank.transform.position = new Vector3(currentPositionBlue3.x, currentPositionBlue3.y, currentPositionBlue3.z - 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(-1, 0, -1);
                                break;
                            case 1:
                                SetTank.transform.position = new Vector3(currentPositionBlue3.x + 60, currentPositionBlue3.y, currentPositionBlue3.z - 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(-1, 0, -1);
                                break;
                            case 2:
                                SetTank.transform.position = new Vector3(currentPositionBlue3.x + 120, currentPositionBlue.y, currentPositionBlue3.z - 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(-1, 0, -1);
                                break;
                            case 3:
                                SetTank.transform.position = new Vector3(currentPositionBlue3.x + 180, currentPositionBlue3.y, currentPositionBlue3.z - 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(-1, 0, -1);
                                break;
                            case 4:
                                SetTank.transform.position = new Vector3(currentPositionBlue3.x + 240, currentPositionBlue3.y, currentPositionBlue3.z - 30 * (SetTank.TankNum % 5 - 1));
                                SetTank.transform.forward = new Vector3(-1, 0, -1);
                                break;
                        }
                        break;
                }

            }
            else
            {
                switch (SetTank.TankNum % 5)
                {
                    case 0:
                        SetTank.transform.position = new Vector3(currentPositionBlue.x, currentPositionBlue.y, currentPositionBlue.z + 20 * (SetTank.TankNum % 5 - 1));
                        SetTank.transform.forward = new Vector3(-1, 0, 1);
                        break;
                    case 1:
                        SetTank.transform.position = new Vector3(currentPositionBlue.x + 20, currentPositionBlue.y, currentPositionBlue.z + 20 * (SetTank.TankNum % 5 - 1));
                        SetTank.transform.forward = new Vector3(-1, 0, 1);
                        break;
                    case 2:
                        SetTank.transform.position = new Vector3(currentPositionBlue.x + 40, currentPositionBlue.y, currentPositionBlue.z + 20 * (SetTank.TankNum % 5 - 1));
                        SetTank.transform.forward = new Vector3(-1, 0, 1);
                        break;
                    case 3:
                        SetTank.transform.position = new Vector3(currentPositionBlue.x + 60, currentPositionBlue.y, currentPositionBlue.z + 20 * (SetTank.TankNum % 5 - 1));
                        SetTank.transform.forward = new Vector3(-1, 0, 1);
                        break;
                    case 4:
                        SetTank.transform.position = new Vector3(currentPositionBlue.x + 80, currentPositionBlue.y, currentPositionBlue.z + 20 * (SetTank.TankNum % 5 - 1));
                        SetTank.transform.forward = new Vector3(-1, 0, 1);
                        break;
                }
            }


        }

    }



    //}
    //��killed tank���
    public void GiveReward(TankControl tank)
    {
        // Debug.Log(tank.name + "give reward �� r:" + num_Red + " B: " + num_Blue);
        tank.AddReward(-0.1f);

        if (tank.tankTeam == TankTeam.Tank_Red)
        {
            --num_Red;
        }
        else
        {
            --num_Blue;
        }

        // Debug.Log(tank.name + "give reward over �� r:" + num_Red + " B: " + num_Blue);

    }

    public void ManGiveReward(ManControl tank)
    {
        --num_Red;
    }

    public int waitTime = 0;
    public void Whether_win()
    {
        if (num_Blue == 0 && num_Red == 0)
        {
            waitTime++;
            if (waitTime > limitWaitTime)
            {
                waitTime = 0;
                if (GameObject.Find("TankSpawner").GetComponent<TankSpawner>().useBio == false)
                {
                    m_RedAgentGroup.AddGroupReward(-0.5f);
                }
                m_BlueAgentGroup.AddGroupReward(-0.5f);
                m_BlueAgentGroup.EndGroupEpisode();
                m_RedAgentGroup.EndGroupEpisode();
                Both_win++;
                ResetScene();
            }

        }
        //�����������Ϊ0�����ٺ췽��������
        else if (num_Blue == 0)
        {
            waitTime++;
            if (waitTime > limitWaitTime)
            {
                BIO_Dead_Cacul_Num = BIO_Dead_Num;
                BIO_PH_Cacul_Loss = BIO_PH_Loss;
                waitTime = 0;
                PH_Red = 0;
                //�����췽����
                foreach (var item in tankSpawner.RedAgentsList)
                {
                    if (!item.Isdead)
                    {
                        PH_Red += item.PH;
                        item.AddReward(0.9f);//0.6
                    }
                    //else
                    //{
                    //    item.tank.gameObject.SetActive(true);
                    //    item.tank.AddReward(0.3f);
                    //    item.tank.gameObject.SetActive(false);
                    //}
                }

                Red_win++;
                //TextList[0].text = "Red_win: " + Red_win.ToString() + " ( " + (Red_win / round).ToString() + " )";
                if (GameObject.Find("TankSpawner").GetComponent<TankSpawner>().useBio == false)
                {
                    SelfGroupReward(m_RedAgentGroup, 1.5f);
                }

                m_BlueAgentGroup.EndGroupEpisode();
                m_RedAgentGroup.EndGroupEpisode();
                num_Red = 0;
                ResetScene();
            }

        }
        else if (num_Red == 0)
        {

            waitTime++;
            if (waitTime > limitWaitTime)
            {
                BIO_Dead_Num = BIO_Dead_Cacul_Num;
                BIO_PH_Loss = BIO_PH_Cacul_Loss;
                waitTime = 0;
                PH_Blue = 0;
                foreach (var item in tankSpawner.BlueAgentsList)
                {
                    if (!item.Isdead)
                    {
                        PH_Blue += item.PH;
                        item.AddReward(0.9f);
                    }
                }
                Blue_win++;
                //     TextList[1].text = "Blue_win: " + Blue_win.ToString() + " ( " + (Blue_win / round).ToString() + " )";
                SelfGroupReward(m_BlueAgentGroup, 1.5f);
                m_BlueAgentGroup.EndGroupEpisode();
                m_RedAgentGroup.EndGroupEpisode();
                num_Blue = 0;
                ResetScene();
            }

        }
    }

    //����Group Reward
    public void SelfGroupReward(SimpleMultiAgentGroup Group, float r)
    {
        if (r is float.NaN)
        {
            //Debug.Log("NaN group Reward");
            return;
        }
        Group.AddGroupReward(r);
    }


    //����������֮�����
    void CalcDistance()
    {
        float distance1;

        //�ȸ������������б�

        if (!GameObject.Find("TankSpawner").GetComponent<TankSpawner>().useBio)
        {
            foreach (var red in tankSpawner.RedAgentsList)
            {
                if (!red.Isdead)
                {
                    foreach (var blue in tankSpawner.BlueAgentsList)
                    {
                        if (!blue.Isdead)
                        {
                            Vector3 direction1 = new Vector3(red.transform.position.x, red.transform.position.y + 2, red.transform.position.z) -
                               new Vector3(blue.transform.position.x, blue.transform.position.y + 2, blue.transform.position.z);
                            distance1 = direction1.magnitude;
                            blue.Enemydir.TryAdd(distance1, red);
                        }

                    }
                }
            }

            foreach (var blue in tankSpawner.BlueAgentsList)
            {
                if (!blue.Isdead)
                {
                    foreach (var red in tankSpawner.RedAgentsList)
                    {
                        if (!red.Isdead)
                        {
                            Vector3 direction1 = new Vector3(blue.transform.position.x, blue.transform.position.y + 2, blue.transform.position.z) -
                                new Vector3(red.transform.position.x, red.transform.position.y + 2, red.transform.position.z);
                            distance1 = direction1.magnitude;
                            red.Enemydir.TryAdd(distance1, blue);


                        }
                    }
                }
            }
        }

        else if (!tankSpawner.useTA)
        {
            foreach (var red in tankSpawner.Biolist)
            {
                if (!red.Isdead)
                {
                    foreach (var blue in tankSpawner.BlueAgentsList)
                    {
                        if (!blue.Isdead)
                        {
                            Vector3 direction1 = new Vector3(red.transform.position.x, red.transform.position.y + 2, red.transform.position.z) -
                                new Vector3(blue.transform.position.x, blue.transform.position.y + 2, blue.transform.position.z);
                            distance1 = direction1.magnitude;
                            blue.EnemyBiodir.TryAdd(distance1, red);
                        }
                    }
                }
            }

            foreach (var blue in tankSpawner.BlueAgentsList)
            {
                if (!blue.Isdead)
                {
                    foreach (var red in tankSpawner.Biolist)
                    {
                        if (!red.Isdead)
                        {
                            Vector3 direction1 = new Vector3(blue.transform.position.x, blue.transform.position.y + 2, blue.transform.position.z) -
                                new Vector3(red.transform.position.x, red.transform.position.y + 2, red.transform.position.z);
                            distance1 = direction1.magnitude;
                            red.BioEnemydir.TryAdd(distance1, blue);

                        }
                    }
                }
            }
        }
        else
        {
            foreach (var red in tankSpawner.Biolist)
            {
                if (!red.Isdead)
                {
                    foreach (var blue in tankSpawner.TAList)
                    {
                        if (!blue.Isdead)
                        {
                            Vector3 direction1 = new Vector3(red.transform.position.x, red.transform.position.y + 2, red.transform.position.z) -
                                new Vector3(blue.transform.position.x, blue.transform.position.y + 2, blue.transform.position.z);
                            distance1 = direction1.magnitude;
                            blue.TAEnemydir.TryAdd(distance1, red);
                        }
                    }
                }
            }

            foreach (var blue in tankSpawner.TAList)
            {
                if (!blue.Isdead)
                {
                    foreach (var red in tankSpawner.Biolist)
                    {
                        if (!red.Isdead)
                        {
                            Vector3 direction1 = new Vector3(blue.transform.position.x, blue.transform.position.y + 2, blue.transform.position.z) -
                                new Vector3(red.transform.position.x, red.transform.position.y + 2, red.transform.position.z);
                            distance1 = direction1.magnitude;
                            red.BioEnemydirTA.TryAdd(distance1, blue);

                        }
                    }
                }
            }
        }

    }




    private void UpdateText()
    {
        string BlueAlgorithm;
        string RedAlgorithm;
        string isOptimized;

        if (TranningSetting.algorithmSelect.BioOptimized) isOptimized = "-Optimized-";
        else isOptimized = "-General-";

        if (TranningSetting.algorithmSelect.NRStandard && !TranningSetting.algorithmSelect.RLStandard)
        {
            if (TranningSetting.BlueTeam.nums == 3 && (TranningSetting.RedTeam.nums == 4 || TranningSetting.RedTeam.nums == 5))
            {
                RedAlgorithm = "BIO" + isOptimized + "More";
                BlueAlgorithm = "NR-Standard-Less";
            }
            else if (TranningSetting.RedTeam.nums == 3 && (TranningSetting.BlueTeam.nums == 4 || TranningSetting.BlueTeam.nums == 5))
            {
                RedAlgorithm = "BIO" + isOptimized + "Less";
                BlueAlgorithm = "NR-Standard-More";
            }
            else
            {
                RedAlgorithm = "BIO" + isOptimized + "Even";
                BlueAlgorithm = "NR-Standard-Even";
            }
        }
        else if (!TranningSetting.algorithmSelect.NRStandard && TranningSetting.algorithmSelect.RLStandard)
        {
            if (TranningSetting.BlueTeam.nums == 3 && (TranningSetting.RedTeam.nums == 4 || TranningSetting.RedTeam.nums == 5))
            {
                RedAlgorithm = "BIO" + isOptimized + "More";
                BlueAlgorithm = "RL-Standard-Less";
            }
            else if (TranningSetting.RedTeam.nums == 3 && (TranningSetting.BlueTeam.nums == 4 || TranningSetting.BlueTeam.nums == 5))
            {
                RedAlgorithm = "BIO" + isOptimized + "Less";
                BlueAlgorithm = "RL-Standard-More";
            }
            else
            {
                RedAlgorithm = "BIO" + isOptimized + "Even";
                BlueAlgorithm = "RL-Standard-Even";
            }
        }
        else
        {
            RedAlgorithm = "";
            BlueAlgorithm = " ";
        }



        ManControl man = FindAnyObjectByType<ManControl>();
        TextList[1].text = "����: " + (round - 1).ToString() + " / 100 ";// + ((float)Red_win / (Red_win + Blue_win)).ToString("f3");// + " - " +

        TextList[2].text = "�췽��ǰ���: " + num_Red.ToString();
        TextList[3].text = "������ǰ���: " + num_Blue.ToString();


        TextList[4].text = "�췽ʤ��: " + ((float)Red_win / (Red_win + Blue_win + Both_win)).ToString("f3");

        float a1 = (float)BIO_Dead_Cacul_Num / (Red_win * TranningSetting.RedTeam.nums);
        float a2 = (float)BIO_PH_Cacul_Loss / (Red_win * TranningSetting.RedTeam.nums * man.PHFULL);

        eff = (1 / (a1 + 0.1f)) * 0.5f +(1 / (a2 + 0.1f)) * 0.5f;
        if (!TranningSetting.RedTeam.HumanControl && !TranningSetting.BlueTeam.HumanControl)
            TextList[5].text = "�췽Ч��ָ��: " + ((1 / (a1 + 0.1f)) * 0.5f +
             (1 / (a2 + 0.1f)) * 0.5f).ToString("f3");
        else
            TextList[5].text = "";

        TextList[6].text = "�췽�㷨: " + RedAlgorithm;
        TextList[7].text = "�����㷨: " + BlueAlgorithm;
        TextList[8].text = "����: ����";
    }

    private void UpdateTitle(int redNum, int blueNum)
    {
        string RedTitle;
        string BlueTitle;
        if (TranningSetting.RedTeam.HumanControl && !TranningSetting.BlueTeam.HumanControl)
        {
            RedTitle = " RL";
            BlueTitle = " MAN";
        }
        else if (!TranningSetting.RedTeam.HumanControl && TranningSetting.BlueTeam.HumanControl)
        {
            RedTitle = " BIO";
            BlueTitle = " MAN";
        }
        else
        {
            RedTitle = " BIO";
            BlueTitle = " RL";
        }
        TextList[0].text = "��ģ��" + redNum.ToString() + "(�췽)" + " VS " + blueNum.ToString() + "(����)";
    }
    public Vector3 GenerateSpawnPoint()
    {
        Vector3 newPosition;
        for (int i = 0; i < 100; i++)
        {
            // ���ѡ��һ��������λ�ö�
            Vector3[] selectedLocation = spawnLocations[UnityEngine.Random.Range(0, spawnLocations.Count)];
            Vector3 pointA = selectedLocation[0]; // ��A
            Vector3 pointB = selectedLocation[1]; // ��B

            // ȷ������֮��ķ���
            Vector3 direction = (pointB - pointA).normalized;

            // ��A��B֮�����ѡ��һ��λ��
            float randomDistance = UnityEngine.Random.Range(0.0f, Vector3.Distance(pointA, pointB));
            newPosition = pointA + direction * randomDistance;

            // �����λ���Ƿ�������λ�ù��ڽӽ�
            bool isTooClose = false;
            foreach (Vector3 existingPosition in spawnPositions)
            {
                if (Vector3.Distance(newPosition, existingPosition) < 10.0f)
                {
                    isTooClose = true;
                    break;
                }
            }

            // �����λ��������λ�ò�̫�ӽ����������λ�ò�����
            if (!isTooClose || i == 99)
            {
                spawnPositions.Add(newPosition);
                return newPosition;
            }
        }

        // �����������������δ�ҵ����ʵ�λ�ã�����һ��Ĭ��λ��
        return new Vector3(0, 0, 0);
    }

    public void SetTagRecursively(GameObject obj, string newTag)
    {
        if (obj == null)
        {
            return;
        }

        obj.tag = newTag;

        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, newTag);
        }
    }

    public void SetPriority()
    {
        Priority = new List<int>();
        // ����һ���ֵ����洢������ı�źͶ�Ӧ�ĵ�������
        Dictionary<int, float> distances = new Dictionary<int, float>();

        // ���� BioList �б��е�ÿ�������壬���㵼�����룬���洢�� distances �ֵ���
        foreach (ManControl man in tankSpawner.Biolist)
        {
            if (!man.Isdead && man.MinNum != 0 && GameObject.Find("TankSpawner").GetComponent<TankSpawner>().useBio)
            {
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(man.transform.position, tankSpawner.BlueAgentsList[man.MinNum - 1].transform.position, NavMesh.AllAreas, path))
                {
                    float distance = GetPathLength(path);
                    distances.Add(man.TankNum, distance);
                }
            }
        }

        // ���ݵ�������Ĵ�С���� distances �ֵ�������򣬲���������ı����䵽 Priority �б���
        foreach (var kvp in distances.OrderBy(x => x.Value))
        {
            Priority.Add(kvp.Key);
        }
    }

    // ��������������·���ĳ���
    private static float GetPathLength(NavMeshPath path)
    {
        float pathLength = 0f;
        if (path.corners.Length < 2)
            return pathLength;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return pathLength;
    }


    //log����
    /*public void LogToTensorBoard(string teamName, string rewardType, float avgValue, int step)
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = pythonPath;
        start.Arguments = $"{scriptPath} {teamName} {rewardType} {avgValue.ToString()} {step.ToString()}";
        start.UseShellExecute = false; // Do not use OS shell
        start.CreateNoWindow = true; // We don't need a new window
        start.RedirectStandardOutput = true; // Any output, generated by application will be redirected back
        start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")

                // handle results or exceptions if needed
            }
        }
    }*/

    //RegisterAgentRewards
    public void RegisterAgentRewards(TankControl TankAgent, List<float> rewards)
    {
        if (is_training)
            return; // ѵ��ʱ����¼����

        // 1. ��¼����
        TankAgent.rewards[(int)TankControl.RewardType.SumReward] = TankAgent.GetCumulativeReward();
        agentRewards[TankAgent.TankNum] = new List<float>(rewards); // �����µ�Listȷ����������������
    }

    public int SelectMap(int mapIndex)
    {
        if (mapIndex == -1) // -1 ��ʾ���ѡ��
        {
            mapIndex = UnityEngine.Random.Range(0, scenePrefabs.Length);
        }
        else if (mapIndex < 0 && mapIndex >= scenePrefabs.Length)
        {
            UnityEngine.Debug.LogWarning($"Invalid mapIndex: {mapIndex}. Defaulting to index 0.");
            mapIndex = 0;
        }
        // ��ȡ��ѡ��ͼ����Ϣ
        MapInfo selectedMapInfo = mapData[mapIndex];

        // ��������ͷ��λ�úʹ�С����Ӧ��ѡ��ͼ
        CameraAdjuster cameraAdjuster = GetComponent<CameraAdjuster>(); //����CameraAdjuster�����ͬһ����Ϸ������
        if (cameraAdjuster != null)
        {
            cameraAdjuster.AdjustCamera(selectedMapInfo.topLeft, selectedMapInfo.bottomRight, scenePrefabs[mapIndex].transform.position.y, mapIndex);
            if (mapIndex == 1) Text.transform.position = new Vector3(55, -2085, -1011);
            else if (mapIndex == 0) Text.transform.position = new Vector3(151, -2496, -1436);
            //TextPos = scenePrefabs[mapIndex].transform.Find("textpos");
            //if (TextPos == null) UnityEngine.Debug.Log("Can't find textpos!");
            //AdjustTextListPos(TextPos.position, mapIndex);
        }
        else
        {
            UnityEngine.Debug.LogError("CameraAdjuster component not found!");
        }

        return mapIndex;
    }
    private void AdjustTextListPos(Vector3 pos, int mapindex)
    {
        Text.position = pos;
        Text.localScale = new Vector3(TextScale[mapindex], 0, TextScale[mapindex]);
        //mask.position = new Vector3(Text.position.x, Text.position.y - 10f, Text.position.z);
    }

    private void InitMapData()
    {
        for (int i = 0; i < scenePrefabs.Length; i++)
        {
            GameObject map = scenePrefabs[i];

            // ��ȡÿ����ͼPrefab�µ�Pos GameObject
            Transform mapPos = map.transform.Find("Pos");

            if (mapPos != null)
            {
                MapInfo info = new MapInfo();

                // ��Pos�������л�ȡλ����Ϣ�����ĳЩGameObjectδ�ҵ�����ʹ��Ĭ��ֵ
                Transform temp;

                temp = mapPos.Find("topLeft");
                info.topLeft = temp ? temp.position : Vector3.zero;

                temp = mapPos.Find("bottomRight");
                info.bottomRight = temp ? temp.position : Vector3.zero;


                // ʹ�ö�ȡ����λ����Ϣ����ʼ����ͼ����
                mapData.Add(i, info);
            }
        }
    }
    //LogAverageRewardsToTensorBoard
    /*public void LogAverageRewardsToTensorBoard(int step)
    {
        if (agentRewards.Count == 0 || is_training)
            return; // ������������

        int rewardTypesCount = System.Enum.GetValues(typeof(RewardType)).Length;
        List<float> totalRewards = new List<float>(new float[rewardTypesCount]);

        // 1. �����ܽ���
        foreach (var rewardsList in agentRewards.Values)
        {
            for (int i = 0; i < rewardTypesCount; i++)
            {
                totalRewards[i] += rewardsList[i];
            }
        }

        // 2. ����ƽ������
        for (int i = 0; i < rewardTypesCount; i++)
        {
            totalRewards[i] /= agentRewards.Count;
        }

        // 3. ��ƽ������д��TensorBoard
        foreach (RewardType rewardType in System.Enum.GetValues(typeof(RewardType)))
        {
            LogToTensorBoard("RL", rewardType.ToString(), totalRewards[(int)rewardType], step);
        }
        //д��Ծ�ʱ��
        LogToTensorBoard("RL", "epoch_len", m_ResetTimer / 200, step);
        //д��ʤ��
        LogToTensorBoard("RL", "Win_rate", RL_win / (RL_win + Bio_win + 1), step);
        // 4. ��յ�ǰ������Ϊ��һ����׼��
        agentRewards.Clear();
    }*/
}




