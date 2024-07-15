
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;
using Google.Protobuf.WellKnownTypes;
using Unity.Barracuda;
using System.Net;
using System.Linq;

public enum TankTeam
{
    Tank_Red = 0,
    Tank_Blue = 1,
}

public class TankControl : Agent
{
    [Tooltip("判断使用离散移动信息or连续移动信息，" +
        "true 表示离散移动信息，此时需要将continus action 个数-2 " +
        "discrete action个数+1，设置增加discrete action的branch为 5")]
    private bool Usediscrete;

    //人机对抗增加代码
    public bool fireFlag = false;
    public int attackEnemy;
    public Transform Rlcannon;
    public Rl_Cannon Rlcannon_script;
    public TankCannon cannon_script2;
    public TankSpawner tankSpawner;
    public TranningSetting trainingSetting;
    //private GameManage gameManage; 修改前
    public GameManage gameManage; //修改后

    public Vector3 enemy_raycast;

    [Tooltip("记录可见视野范围内的对手")]
    public SortedDictionary<float, TankControl> Enemydir = new();
    public SortedDictionary<float, TankControl> Enemydir1 = new();
    public SortedDictionary<float, ManControl> EnemyBiodir = new();
    public SortedDictionary<float, ManControl> EnemyBiodir1 = new();

    [Tooltip("定义可以获取对手信息的个数，暂时定为10个")]
    public readonly int EnemyCount = 1;
    public int Enemy_len = 0;

    private TranningSetting TranningSetting;
    public TankTeam tankTeam;
    public Transform pos;

    private string inputHorizontalStr;
    private string inputVerticalStr;
    private string inputFirestr;
    private bool Bioflag;

    BehaviorParameters m_BehaviorParameters;

    public Rigidbody _rigidbody;

    private float MaxSpeed;
    public float MaxFireAngle = 25;

    private float rotatespeed;

    private bool UseOldObserve;

    //Fire及血条
    public GameObject shell;
    private float Firevalue;
    public int cooldowntime;
    public int Fireinterval;
    public int Standardinterval;
    public int firetime = 0;
    public float PH = 50;
    public Transform ShellPos;
    public Slider phSlider;
    public int TankNum;

    //tank的可视化信息
    public int fire = 1;
    public int FireSuccess = 0;
    public int FireHurtteammate = 0;
    public int kill = 0;
    public int death = 0;
    public int killBymate = 0;
    public int killSelf = 0;

    public float three_rdX_axis;
    public float Y_rdX_axis;
    private string BehaviorName;

    public TextMeshPro NUM_Text;
    public Image fillImage; // 血条填充图像的引用

    public ParticleSystem tankExplosion;
    private float stuckTime = 0f;
    public float urgetime = 0f;
    private TankControl lastClosestEnemy;  // 用于记录上一次最近的敌人 
    private ManControl lastClosestBioEnemy;
    private float LastCloestDis;
    Dictionary<ManControl, float> lastClosestDis = new Dictionary<ManControl, float>();
    public float vv, rv;

    private Vector3 last_position;

    [Tooltip("改变重力")]
    private float gravity_Y = 0;

    public float isEnemyNear = 0f;
    public float NearDis = 1000;
    public TankControl NearestEnemy;
    public TankControl TargetEnemy;

    public BaseFunction baseFunction;


    private RaycastHit hit;

    public Transform cannon;
    public Transform Tower;

    public float minspeed = 15;

    public LineRenderer lineRenderer;
    public float TowardAngle = 10;

    public float TotalVelocity;
    public bool FireJudge;

    public Vector3 LastPosition;
    public float LastDistance = 999;

    Queue<Vector3> recentPositions = new Queue<Vector3>(15);
    float minimumMoveDistance = 0.5f;
    bool has_wall = false;
    private float checkDistance = 14.0f;
    public List<float> rewards;

    private float moveValue;
    public float vvlue;

    private Vector3 ClosetPosition;
    private float ClosetEnemydistance = 999;
    private ManControl ClosetEnemy;

    public int Clisize = 40;
    public bool Isdead;
    public Vector3 shell_pos;

    public Material Black;
    public Color SliderBlack;
    public ManControl enemY;
    public bool isRuleModle = false;

    public enum RewardType
    {
        NoMovementPenalty,
        MoveTowardsEnemyReward,
        NoMoveTowardsEnemyPenalty,
        KillEnemyReward,
        KilledByEnemyPenalty,
        WinReward,
        SumReward,
    }

    void Start()
    {
        //初始化参数
        TranningSetting = FindObjectOfType<TranningSetting>();
        gameManage = FindObjectOfType<GameManage>();
        baseFunction = FindObjectOfType<BaseFunction>();
        Tower = transform.Find("TankRenderers/TankFree_Tower");
        cannon = transform.Find("TankRenderers/TankFree_Tower/TankFree_Canon");

        Rlcannon = transform.Find("TankRenderers/TankFree_Tower/TankFree_Canon");
        Rlcannon_script = Rlcannon.GetComponent<Rl_Cannon>();

        //人机对抗增加代码
        tankSpawner = FindObjectOfType<TankSpawner>();
        trainingSetting = FindAnyObjectByType<TranningSetting>();
        cannon_script2 = cannon.GetComponent<TankCannon>();


        //射线
        //lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.startWidth = 1.5f;
        //lineRenderer.endWidth = 1.5f;


        //如果找到配置文件
        if (TranningSetting)
        {

            if (tankTeam == TankTeam.Tank_Blue)
            {
                Usediscrete = TranningSetting.BlueTeam.Usediscrete;
                MaxSpeed = TranningSetting.BlueTeam.MaxSpeed;
                rotatespeed = TranningSetting.BlueTeam.rotatespeed;
                cooldowntime = TranningSetting.BlueTeam.cooldowntime;
                Fireinterval = TranningSetting.BlueTeam.Fireinterval;
                Standardinterval = TranningSetting.BlueTeam.Standardinterval;
                UseOldObserve = TranningSetting.BlueTeam.UseOldObserve;
                PH = TranningSetting.BlueTeam.FULLPH;
                Clisize = TranningSetting.BlueTeam.Clisize;

                BehaviorName = TranningSetting.BlueTeam.BehaviorName;

            }
            else if (tankTeam == TankTeam.Tank_Red)
            {
                Usediscrete = TranningSetting.RedTeam.Usediscrete;
                MaxSpeed = TranningSetting.RedTeam.MaxSpeed;
                rotatespeed = TranningSetting.RedTeam.rotatespeed;
                cooldowntime = TranningSetting.RedTeam.cooldowntime;
                Fireinterval = TranningSetting.RedTeam.Fireinterval;
                Standardinterval = TranningSetting.RedTeam.Standardinterval;
                UseOldObserve = TranningSetting.RedTeam.UseOldObserve;
                PH = TranningSetting.RedTeam.FULLPH;
                Clisize = TranningSetting.RedTeam.Clisize;

                //EnemyCount = TranningSetting.EnvInfo.is3VN ? 3 : gameManage.BlueAgentsList.Count;

                BehaviorName = TranningSetting.RedTeam.BehaviorName;
            }
            tankExplosion = TranningSetting.tankExplosion;

        }
        else
        {
            Debug.LogError("配置失败");
        }

        TankInitlization();
    }

    public void FixedUpdate()
    {
        Bioflag = GameObject.Find("TankSpawner").GetComponent<TankSpawner>().useBio;
        if (Bioflag)
        {
            Enemy_len = EnemyBiodir.Count;
        }
        else
        {
            Enemy_len = Enemydir.Count;
        }
        if (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= 27)
        {
            _rigidbody.AddForce(Vector3.down * 2000000f, ForceMode.Impulse);
        }
        else if (GameObject.Find("NVN").GetComponent<GameManage>().Righttime < 27 && GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= 25)
        {
            move(MaxSpeed, 0, 0);
        }
        //TankGround();
    }

    //手动控制代码
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        var continuousActions = actionsOut.ContinuousActions;

        if (trainingSetting.BlueTeam.HumanControl)
        {
            if (TankNum == 0)
            {
                // Handling movement
                if (Input.GetKey(KeyCode.W))
                {
                    continuousActions[0] = 1; // Move forward
                }
                if (Input.GetKey(KeyCode.S))
                {
                    continuousActions[0] = -1; // Move backward
                }
                if (Input.GetKey(KeyCode.A))
                {
                    continuousActions[1] = -0.25f; // Rotate left
                }
                if (Input.GetKey(KeyCode.D))
                {
                    continuousActions[1] = 0.25f; // Rotate right
                }
                if (Input.GetKey(KeyCode.Q) || Input.GetKeyDown(KeyCode.RightShift))
                {
                    fireFlag = true;
                }
                else
                {
                    fireFlag = false;
                }

            }
            else if (TankNum == 1)
            {
                if (Input.GetKey(KeyCode.T))
                {
                    continuousActions[0] = 1; // Move forward
                }
                if (Input.GetKey(KeyCode.G))
                {
                    continuousActions[0] = -1; // Move backward
                }
                if (Input.GetKey(KeyCode.F))
                {
                    continuousActions[1] = -0.25f; // Rotate left
                }
                if (Input.GetKey(KeyCode.H))
                {
                    continuousActions[1] = 0.25f; // Rotate right
                }
                if (Input.GetKey(KeyCode.R) || Input.GetKeyDown(KeyCode.Space))
                {
                    fireFlag = true;
                }
                else
                {
                    fireFlag = false;
                }
            }
            else if (TankNum == 2)
            {
                if (Input.GetKey(KeyCode.I))
                {
                    continuousActions[0] = 1; // Move forward
                }
                if (Input.GetKey(KeyCode.K))
                {
                    continuousActions[0] = -1; // Move backward
                }
                if (Input.GetKey(KeyCode.J))
                {
                    continuousActions[1] = -0.25f; // Rotate left
                }
                if (Input.GetKey(KeyCode.L))
                {
                    continuousActions[1] = 0.25f; // Rotate right
                }
                if (Input.GetKey(KeyCode.U) || Input.GetKeyDown(KeyCode.LeftShift))
                {
                    fireFlag = true;
                }
                else
                {
                    fireFlag = false;
                }
            }
            else if (TankNum == 3)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    continuousActions[0] = 1; // Move forward
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    continuousActions[0] = -1; // Move backward
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    continuousActions[1] = -0.25f; // Rotate left
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    continuousActions[1] = 0.25f; // Rotate right
                }
                if (Input.GetKey(KeyCode.Alpha0) || Input.GetKey(KeyCode.M))
                {
                    fireFlag = true;
                }
                else
                {
                    fireFlag = false;
                }
            }
            else if (TankNum == 4)
            {
                if (Input.GetKey(KeyCode.Alpha8))
                {
                    continuousActions[0] = 1; // Move forward
                }
                if (Input.GetKey(KeyCode.Alpha6))
                {
                    continuousActions[0] = -1; // Move backward
                }
                if (Input.GetKey(KeyCode.Alpha7))
                {
                    continuousActions[1] = -0.25f; // Rotate left
                }
                if (Input.GetKey(KeyCode.Alpha9))
                {
                    continuousActions[1] = 0.25f; // Rotate right
                }
                if (Input.GetKey(KeyCode.Alpha0) || Input.GetKey(KeyCode.B))
                {
                    fireFlag = true;
                }
                else
                {
                    fireFlag = false;
                }
            }
        }



    }
    public void TankInitlization()
    {
        inputHorizontalStr = "Horizontal";
        inputVerticalStr = "Vertical";
        inputFirestr = "Fire";
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        m_BehaviorParameters.TeamId = tankTeam == TankTeam.Tank_Red ? 0 : 1;
        m_BehaviorParameters.BehaviorName = BehaviorName;

        inputHorizontalStr = inputHorizontalStr + TankNum;
        inputVerticalStr = inputVerticalStr + TankNum;
        inputFirestr = inputFirestr + TankNum;

        phSlider.maxValue = PH;
        phSlider.value = PH;

    }

    void TankGround()
    {
        int Rmask = LayerMask.GetMask("GroundLayer");
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 180.0f, Rmask))  //&& (baseFunction.angleOffset(angle.x) > -5 || baseFunction.angleOffset(angle.x) < -35))
        {
            if (Vector3.Distance(transform.position, hit.point) > 5.0f)
            {
                _rigidbody.AddForce(Vector3.down * 2000000f, ForceMode.Impulse);
            }
        }
    }

    //移动控制
    public void move(float v_value, float h_value, float tower_rotate)
    {

        if (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= 27)
        {
            _rigidbody.AddForce(Vector3.down * 20000000f, ForceMode.Impulse);
        }
        if (Isdead || (gameManage.round > 100 && !trainingSetting.EnvInfo.isTrainning))
        {
            return;
        }
        moveValue = v_value;

        vvlue = v_value;
        if (v_value > MaxSpeed) vvlue = MaxSpeed;
        else if (v_value < -MaxSpeed) vvlue = -MaxSpeed;

        //不翻车
        int Rmask = LayerMask.GetMask("GroundLayer");
        if (Physics.Raycast(transform.position, -transform.up, out hit, 40.0f, Rmask))  //&& (baseFunction.angleOffset(angle.x) > -5 || baseFunction.angleOffset(angle.x) < -35))
        {
            //Quaternion NextRot = Quaternion.LookRotation(Vector3.Cross(hit.normal, Vector3.Cross(transform.forward, hit.normal)), hit.normal);
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 30f);

        }

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 40.0f, Rmask))  //&& (baseFunction.angleOffset(angle.x) > -5 || baseFunction.angleOffset(angle.x) < -35))
        {
            if (Vector3.Distance(transform.position, hit.point) > 0.5f)
            {
                _rigidbody.AddForce(Vector3.down * 200000f, ForceMode.Impulse);
                //UnityEngine.Debug.Log(TankNum + " :" + Vector3.Distance(transform.position, hit.point));
            }

        }

        //后退速度只有2/3
        if (v_value < 0)
        {
            v_value = v_value * 2 / 3 * 0.7f;
        }
        Vector3 forceDirection = transform.forward * v_value;

        // Applying force to move the agent
        _rigidbody.AddForce(forceDirection * 0.7f, ForceMode.VelocityChange);

        //限制最大速度为45
        if (_rigidbody.velocity.magnitude > 20 && vv > 0)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * 20 * 0.7f;
        }
        else if (_rigidbody.velocity.magnitude > 20 * 2 / 3 && vv < 0)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * 20 * 2 / 3 * 0.7f;
        }

        if (h_value != 0)
        {
            if (v_value < 0)
            {
                //           Debug.Log(" 倒车需要反向 vvlaue: " + v_vlaue + " h_value: " + h_value);
                h_value = -h_value;
            }

            this.gameObject.transform.Rotate(h_value * rotatespeed * Time.deltaTime * Vector3.up);
        }


        if (tower_rotate != 0)
        {
            Tower.gameObject.transform.Rotate(tower_rotate * rotatespeed * 1.1f * Time.deltaTime * Vector3.up);
        }
    }

    //移动控制2
    public void move2(ActionSegment<int> act)
    {
        var action = act[1];
        switch (action)
        {
            case 0:
                _rigidbody.velocity = Vector3.zero;
                break;
            case 1:
                _rigidbody.velocity = transform.forward * (MaxSpeed);
                break;
            case 2:
                _rigidbody.velocity = transform.forward * (-MaxSpeed);
                break;
            case 3:
                gameObject.transform.Rotate(rotatespeed * Time.deltaTime * Vector3.up);
                break;
            case 4:
                gameObject.transform.Rotate(-rotatespeed * Time.deltaTime * Vector3.up);
                break;
        }
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, gravity_Y, _rigidbody.velocity.z);
    }



    //血量低于0，tank dead
    private void TankDead(bool killbyself, float damage)
    {
        //如果已经在上一帧death,不记录
        if (Isdead) return;
        //记录个体death数量以及被killed数量
        
        PH -= damage;
        if (PH <= 0)
        {
            ++death;
            if (killbyself) killSelf++;
            gameManage.tankSpawner.ChangeTankColor(this, Black);
            NUM_Text.color = SliderBlack;
            Isdead = true;

            if (tankExplosion != null)
            {
                //tankExplosion.transform.parent = null;
                tankExplosion.Play();
                //Destroy(tankExplosion.gameObject, tankExplosion.main.duration);
            }

            //    Debug.Log(name + "is dead");
            gameManage.GiveReward(this);
            AddReward(-0.2f);
        }
        
        //this.gameObject.SetActive(false);
    }

    //炮台旋转
    public void Tower_rotate(int rotate_dir, int rotate_flag)
    {
        if (rotate_flag == 1)
        {
            if (rotate_dir == 1)
            {
                Tower.Rotate(Vector3.up, 0.5f);//向右转
            }
            else
            {
                Tower.Rotate(Vector3.down, 0.5f);//向左转
            }
        }
    }


    /*void OpenFire(float shellAngle, int flags)
    {

        if (shell != null && flags == 1 && (firetime >= cooldowntime))
        {
            GameObject shellObj = Instantiate(shell, ShellPos.position, ShellPos.transform.rotation);
            Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
            if (shellRigidbody != null)
            {
                //Firevalue = (shellSpeed + 1) * 900f + 5.0f;//发炮速度：5-50m/s
                //确保发炮速度不小于移动速度

                shellRigidbody.velocity = ShellPos.forward * 2000;
                shellObj.GetComponent<ShellControl>().Set_father(gameObject);

            }
            firetime = 0;
            Clisize--;
            Fireinterval = 0;
        }
    
    }*/

    public void OpenFire(float shellAngle, int flags)
    {

        Rlcannon = transform.Find("TankRenderers/TankFree_Tower/TankFree_Canon");
        Rlcannon_script = Rlcannon.GetComponent<Rl_Cannon>();

        if (Enemy_len != 0 && Isdead == false)
        {
            enemY = EnemyBiodir.First().Value;
        }

        Vector3 target = enemY.transform.position;
        Vector3 newTarget = new Vector3(target.x, target.y + 1.5f, target.z);
        newTarget = newTarget - ShellPos.position;
        shellAngle = Vector3.Angle(newTarget, ShellPos.forward);
        shellAngle = newTarget.normalized.y > cannon.transform.forward.normalized.y ?
                    shellAngle : -shellAngle;
        //将炮筒旋转角度限制在0 - 35度
        shellAngle = shellAngle < 25.0f ? shellAngle : 25.0f;
        shellAngle = shellAngle > -5.0f ? shellAngle : -5.0f;

        //炮筒旋转

        if (!TranningSetting.BlueTeam.HumanControl)
        {
            if (-(baseFunction.angleOffset(cannon.localRotation.eulerAngles.x)) >= 0.0f)
            {
                if (shellAngle < 0)
                {
                    cannon.Rotate(Vector3.right, 0.3f);
                }
                else if (shellAngle < -2)
                {
                    cannon.Rotate(Vector3.right, 0.6f);
                }
            }
            if (-(baseFunction.angleOffset(cannon.localRotation.eulerAngles.x)) <= 25.0f)
            {
                if (shellAngle >= 2)
                {
                    cannon.Rotate(-Vector3.right, 0.6f);
                }
                else if (shellAngle >= 0)
                {
                    cannon.Rotate(-Vector3.right, 0.3f);
                }
            }
        }
      
        if (shell != null && flags == 1 && (firetime >= cooldowntime) && Rlcannon_script.ColliderFlag && !Isdead) // && Rlcannon_script.ColliderFlag
        {
            // shell_start_pos = ShellPos.position;//记录炮弹发射时的坐标
            GameObject shellObj = Instantiate(shell, ShellPos.position, ShellPos.transform.rotation);

            Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
            ShellControl shell_script = shellObj.GetComponent<ShellControl>();

            if (shellRigidbody != null)
            {
                shellRigidbody.velocity = ShellPos.forward * 1400;
                shellObj.GetComponent<ShellControl>().Set_father(gameObject);

            }
            ++fire;
            Clisize--;
            firetime = 0;
            Fireinterval = 0;
        }

    }

    //发射炮弹代码(人机对抗使用)
    public void OpenFire2(int flags)
    {
        float shellAngle;
        Vector3 target = tankSpawner.Biolist[attackEnemy - 1].transform.position;
        Vector3 newTarget = new Vector3(target.x, target.y + 1.5f, target.z);
        newTarget = newTarget - ShellPos.position;
        shellAngle = Vector3.Angle(newTarget, ShellPos.forward);
        shellAngle = newTarget.normalized.y > cannon.transform.forward.normalized.y ?
                    shellAngle : -shellAngle;
        //将炮筒旋转角度限制在0 - 35度
        shellAngle = shellAngle < 25.0f ? shellAngle : 25.0f;
        shellAngle = shellAngle > -5.0f ? shellAngle : -5.0f;

        if (-(baseFunction.angleOffset(cannon.localRotation.eulerAngles.x)) >= 0.0f)
        {
            if (shellAngle < 0)
            {
                cannon.Rotate(Vector3.right, 0.3f);
            }
            else if (shellAngle < -2)
            {
                cannon.Rotate(Vector3.right, 0.6f);
            }
        }
        if (-(baseFunction.angleOffset(cannon.localRotation.eulerAngles.x)) <= 25.0f)
        {
            if (shellAngle >= 2)
            {
                cannon.Rotate(-Vector3.right, 0.6f);
            }
            else if (shellAngle >= 0)
            {
                cannon.Rotate(-Vector3.right, 0.3f);
            }
        }

        //当炮筒旋转至设定角度后才能开炮
        if (shell != null && flags == 1 && (firetime >= cooldowntime) && Clisize <= 40 &&
            (fireFlag))//0.1为一个补偿值，因为炮筒角度不能真正达到设定角度，只能无限逼近
        {
            GameObject shellObj = Instantiate(shell, ShellPos.position, ShellPos.transform.rotation);
            Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
            ShellControl shell_script = shellObj.GetComponent<ShellControl>();


            if (shellRigidbody != null)
            {
                shellRigidbody.velocity = ShellPos.forward * 1400;
                shellObj.GetComponent<ShellControl>().Set_father(gameObject);
            }
            ++fire;
            Clisize--;
            firetime = 0;
        }
    }
    //发射炮弹代码(炮筒可旋转)
    /*public void OpenFire(float shellAngle, int flags)
    {
        if (Isdead || Clisize <= 0)
        {
            return;
        }
        //将炮筒旋转角度限制在0 - 35度
        shellAngle = shellAngle < 2f ? shellAngle : 2.0f;
        shellAngle = shellAngle > 0f ? shellAngle : 0f;

        if (-(baseFunction.angleOffset(cannon.localRotation.eulerAngles.x)) <= shellAngle && -(baseFunction.angleOffset(cannon.localRotation.eulerAngles.x)) < 2.0f)
        {
            cannon.Rotate(-Vector3.right, 0.5f);
        }
        if (-(baseFunction.angleOffset(cannon.localRotation.eulerAngles.x)) > shellAngle && -(baseFunction.angleOffset(cannon.localRotation.eulerAngles.x)) > 0f)
        {
            cannon.Rotate(Vector3.right, 0.5f);
        }

        //当炮筒旋转至设定角度后才能开炮
        if (shell != null && flags == 1 && (firetime >= cooldowntime) &&
            (-(baseFunction.angleOffset(cannon.localRotation.eulerAngles.x)) > shellAngle - 0.5))//0.1为一个补偿值，因为炮筒角度不能真正达到设定角度，只能无限逼近
        {
            // shell_start_pos = ShellPos.position;//记录炮弹发射时的坐标
            GameObject shellObj = Instantiate(shell, ShellPos.position, ShellPos.transform.rotation);
            Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
            ShellControl shell_script = shellObj.GetComponent<ShellControl>();

            if (shellRigidbody != null)
            {
                //Firevalue = (shellSpeed + 1) * 900f + 5.0f;//发炮速度：5-50m/s
                //确保发炮速度不小于移动速度

                shellRigidbody.velocity = ShellPos.forward * 3000;
                shellObj.GetComponent<ShellControl>().Set_father(gameObject);

            }
            ++fire;
            Clisize--;
            firetime = 0;
            Fireinterval = 0;
        }
    }*/



    //炮弹伤害代码
    public void ShellDamage(float damage)
    {

        TankDead(false, damage);

    }


    //给予奖励函数
    public void Selfreward(int rewardType, float r)
    {
        //添加这部分代码
        if (r is float.NaN) { return; }

        //if (TankNum == 1)
        //    print(rewardType + " : " + r.ToString() + " : " + rewards.Count);

        rewards[rewardType] += r;
        AddReward(r);
    }

    //对撞墙以及冲撞其他个体惩罚
    public void OnCollisionEnter(Collision other)
    {
        /*if (other.transform.CompareTag("Collider"))
        {
            AddReward(-0.05f);           

        }*/
        /*else if (other.transform.CompareTag("Tank_Blue") || other.transform.CompareTag("Tank_Red"))
        {
            AddReward(-0.05f);
            PH -= 1.0f;
            phSlider.value = PH;
        }*/

    }

    //将神经网络的输出转化为具体的控制
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //改变重力
        gravity_Y += Physics.gravity.y * Time.deltaTime;
        gravity_Y = Mathf.Clamp(gravity_Y, Physics.gravity.y, 0);
        firetime += 1;
        Fireinterval += 1;

        if (!Isdead && EnemyBiodir.Count != 0 && UnityEngine.Random.Range(0f, 1f) > 0.1f && trainingSetting.BlueTeam.HumanControl)
        {
            
            attackEnemy = EnemyBiodir.First().Value.TankNum;
            OpenFire2(1);
        }
            if (firetime == 65535)
        {
            firetime = cooldowntime + 1;
        }
        var continuousActions = actionBuffers.ContinuousActions;
        var discreteActions = actionBuffers.DiscreteActions;

        if (Usediscrete)
        {
            move2(discreteActions);
            OpenFire(continuousActions[2] * MaxFireAngle, discreteActions[1]);
            //OpenFire(1, discreteActions[1]);
        }
        else
        {
            if (isRuleModle)
            {
                vv = continuousActions[0] * MaxSpeed;
                rv = continuousActions[1] * rotatespeed;
                if (continuousActions[0] != 0)
                {
                    if (continuousActions[0] < 0.0f)
                        backTargetEnemy(EnemyBiodir);
                    else if (continuousActions[0] > 0.0f)
                        Concentrate();
                }

                if (continuousActions[1] != 0)
                {
                    if (continuousActions[1] < 0.0f)
                        moveTargetEnemy(EnemyBiodir);
                    else
                        CacAvoidDir(EnemyBiodir);
                }


                if (EnemyBiodir.Count != 0)
                {
                    if (continuousActions[2] > 0.0f)
                    {
                        openFireEnemy(EnemyBiodir);
                    }
                }
            }
            else
            {
                vv = continuousActions[0] * MaxSpeed;
                rv = continuousActions[1] * rotatespeed;
                move(continuousActions[0] * MaxSpeed, continuousActions[1] * rotatespeed, 0); //continuousActions[3] * rotatespeed
                if(EnemyBiodir.Count != 0) OpenFire(continuousActions[2] * MaxFireAngle, discreteActions[0]);

            }

            //OpenFire(continuousActions[2], discreteActions[0]);
        }
        //每一帧都扣固定的伤害
        AddReward(-1.0f / 10000);

        //智能体朝向敌军移动，给奖励。
        MovetoEnemy();
        //TowertoEnemy();

        PosChangeReward();

        //添加移动奖励，如果朝向敌方 且有速度，给奖励
        //增加僵持时间惩罚
        if (Enemy_len != 0)
        {
            if (Fireinterval >= Standardinterval && EnemyBiodir.First().Key <= 500)
            {
                AddReward(-0.08f);
                Fireinterval = 0;
            }
        }


    }


    //收集观测信息
    //这里观测信息有两种，一种是原来信息，未添加noamaize，另外一种添加normalize，参考人工智狼
    //这里参考人工智狼的输入，每次输入最近的10个个体的信息。
    //共输入93维度
    public override void CollectObservations(VectorSensor sensor)
    {

        int i = 0;
        Vector3 planeNormal1 = new Vector3(0f, 1f, 0f);
        // 自身信息 1 + 4维度
        //sensor.AddObservation(PH / 10);
        sensor.AddObservation(_rigidbody.velocity.normalized);
        sensor.AddObservation(transform.forward.normalized);

        //sensor.AddObservation(transform.localRotation.normalized);
        //添加新观测
        sensor.AddObservation(-baseFunction.angleOffset(cannon.localRotation.eulerAngles.x));
        //sensor.AddObservation(Tower.forward - Vector3.Dot(Tower.forward, planeNormal1) * planeNormal1);

        sensor.AddObservation(Tower.forward.normalized);

        ////输入敌方信息 4*3=12

        if (Bioflag)
        {
            foreach (var dis in EnemyBiodir.Keys)
            {
                if (i >= EnemyCount) break;
                var Enemy = EnemyBiodir[dis];
                if (Enemy.Isdead) continue;

                Vector3 Enemydata = Enemy.transform.position - transform.position;
                Vector3 TowerToEnemy = Enemy.transform.position - Tower.position;

                Vector3 planeNormal = new Vector3(0f, 1f, 0f);
                Vector3 planeForward = transform.forward - Vector3.Dot(transform.forward, planeNormal) * planeNormal;  //坦克前进方向在XOZ面上的投影
                Vector3 planeEnemydata = Enemydata - Vector3.Dot(Enemydata, planeNormal) * planeNormal;

                //两个向量的内积：-1到1的浮点数（1维）

                //sensor.AddObservation(Vector3.Dot(Enemydata.normalized, transform.forward.normalized));
                //sensor.AddObservation(Vector3.Angle(Enemydata.normalized, transform.forward.normalized));
                //sensor.AddObservation(Vector3.Dot(planeEnemydata.normalized, planeForward.normalized));
                sensor.AddObservation(Vector3.Angle(planeEnemydata.normalized, planeForward.normalized));

                //sensor.AddObservation(Vector3.Angle(Tower.forward.normalized, TowerToEnemy.normalized));

                //指向目标的向量（二维向量）
                //sensor.AddObservation(Enemydata.normalized);
                sensor.AddObservation(planeEnemydata.normalized);

                //距离敌方的距离（一维）
                sensor.AddObservation(Mathf.Clamp(dis / 500, 0, 1));
                //sensor.AddObservation(dis);
                ++i;

            }
        }
        else
        {
            foreach (var dis in Enemydir.Keys)
            {
                if (i >= EnemyCount) break;
                var Enemy = Enemydir[dis];
                if (Enemy.Isdead) continue;

                Vector3 Enemydata = Enemy.transform.position - transform.position;

                Vector3 planeNormal = new Vector3(0f, 1f, 0f);
                Vector3 planeForward = transform.forward - Vector3.Dot(transform.forward, planeNormal) * planeNormal;  //坦克前进方向在XOZ面上的投影
                Vector3 planeEnemydata = Enemydata - Vector3.Dot(Enemydata, planeNormal) * planeNormal;

                //两个向量的内积：-1到1的浮点数（1维）

                sensor.AddObservation(Vector3.Dot(Enemydata.normalized, transform.forward.normalized));
                //sensor.AddObservation(Vector3.Dot(planeEnemydata.normalized, planeForward.normalized));

                //指向目标的向量（二维向量）

                sensor.AddObservation(Enemydata.normalized);
                //sensor.AddObservation(planeEnemydata.normalized);

                //距离敌方的距离（一维）
                sensor.AddObservation(Mathf.Clamp(dis / 500, 0, 1));
                //sensor.AddObservation(dis);
                ++i;
            }
        }


        //如果看不见敌军信息
        if (i < EnemyCount)
        {
            float[] EnemyData;
            //Debug.Log(name + "no army");
            EnemyData = new float[(EnemyCount - i) * 5]; //5
            sensor.AddObservation(EnemyData);
        }
    }



    private float[] GetEnemyData(TankControl EnemyTank, float dis)
    {
        float[] Enemydata = new float[4];
        Vector3 offset = (EnemyTank.transform.position - transform.position).normalized;
        Enemydata[0] = offset.x;
        Enemydata[1] = offset.z;
        Enemydata[2] = dis / 50;  //distance
        Enemydata[0] = (EnemyTank.PH / 50) is float.NaN ? 0 : (EnemyTank.PH / 50);
        return Enemydata;
    }


    public void ClosetEnemyRay()
    {
        if (Bioflag)
        {
            ManControl Enemy = EnemyBiodir.First().Value;
            Vector3 direction = new Vector3(Enemy.transform.position.x, Enemy.transform.position.y + 2, Enemy.transform.position.z) -
                                new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            float Distance = direction.magnitude;
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), direction.normalized * Distance, Color.red);
        }
        else
        {
            TankControl Enemy = Enemydir.First().Value;
            Vector3 direction = new Vector3(Enemy.transform.position.x, Enemy.transform.position.y + 2, Enemy.transform.position.z) -
                                new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            float Distance = direction.magnitude;
            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), direction.normalized * Distance, Color.red);
        }
    }

    public void TestFunction(Vector3 Start, Vector3 End)
    {
        float rayLength = 5;
        // 射线的起点和方向
        Vector3 direction = new Vector3(End.x, End.y, End.z) - Start;

        // 发射射线
        Ray ray = new Ray(Start, direction);

        int Rmask = LayerMask.GetMask("Tank");
        RaycastHit hit;
        float dis = Vector3.Distance(Start, End);

        lineRenderer.SetPosition(0, Start);
        lineRenderer.SetPosition(1, End);

    }


    public void OpenFireTarget(float shellAngle)
    {
        Vector3 EnemyPosition;
        Vector3 TowardEnemy;
        Vector3 planeNormal = new Vector3(0f, 1f, 0f);
        float Enemydistance = 200;
        float Height;
        float FireAngle;
        int i = 0;
        if (Bioflag)
        {
            foreach (var FireEnemy in EnemyBiodir.Values)
            {
                if (i >= 1) break;
                EnemyPosition = FireEnemy.transform.position;
                TowardEnemy = EnemyPosition - transform.position;

                Vector3 planeForward = transform.forward - Vector3.Dot(transform.forward, planeNormal) * planeNormal;
                Vector3 planeTowardEnemy = TowardEnemy - Vector3.Dot(TowardEnemy, planeNormal) * planeNormal;

                if (Vector3.Angle(planeForward, planeTowardEnemy) < 12f)
                {
                    Enemydistance = TowardEnemy.magnitude;
                }

                Height = ShellPos.position.y - EnemyPosition.y;

                if (Enemydistance <= 120)
                {
                    if (Height >= 0)
                    {
                        FireAngle = shellAngle + (90 - Vector3.Angle(TowardEnemy, planeNormal));
                        TotalVelocity = Mathf.Sqrt(Enemydistance * 9.81f / Mathf.Sin(2 * FireAngle));

                    }
                    else
                    {
                        FireAngle = shellAngle - (90 - Vector3.Angle(TowardEnemy, planeNormal));
                        TotalVelocity = Mathf.Sqrt(Enemydistance * 9.81f / Mathf.Sin(2 * FireAngle));
                    }
                }
                else
                {
                    TotalVelocity = Firevalue;
                }

                ++i;
            }
        }
        else
        {
            foreach (var FireEnemy in Enemydir.Values)
            {
                if (i >= 1) break;
                EnemyPosition = FireEnemy.transform.position;
                TowardEnemy = EnemyPosition - transform.position;

                Vector3 planeForward = transform.forward - Vector3.Dot(transform.forward, planeNormal) * planeNormal;
                Vector3 planeTowardEnemy = TowardEnemy - Vector3.Dot(TowardEnemy, planeNormal) * planeNormal;

                if (Vector3.Angle(planeForward, planeTowardEnemy) < 8f)
                {
                    Enemydistance = TowardEnemy.magnitude;
                }

                Height = ShellPos.position.y - EnemyPosition.y;

                if (Enemydistance <= 70)
                {
                    if (Height >= 0)
                    {
                        FireAngle = shellAngle + (90 - Vector3.Angle(TowardEnemy, planeNormal));
                        TotalVelocity = Mathf.Sqrt(Enemydistance * 9.81f / Mathf.Sin(2 * FireAngle));

                    }
                    else
                    {
                        FireAngle = shellAngle - (90 - Vector3.Angle(TowardEnemy, planeNormal));
                        TotalVelocity = Mathf.Sqrt(Enemydistance * 9.81f / Mathf.Sin(2 * FireAngle));
                    }
                }
                else
                {
                    TotalVelocity = Firevalue;
                }

                ++i;
            }
        }


    }


    private void PosChangeReward()
    {
        if (Bioflag)
        {
            // 在你的Step()函数中
            if (recentPositions.Count >= 15)
            {
                recentPositions.Dequeue();
            }
            recentPositions.Enqueue(transform.position);

            // 如果我们已经记录了足够多的位置
            if (recentPositions.Count == 15)
            {
                // 获取最旧的位置
                Vector3 oldestPosition = recentPositions.Peek();

                // 计算智能体从最旧的位置移动的距离
                float moveDistance = Vector3.Distance(oldestPosition, transform.position);
                bool IsBioFar = false;
                if (EnemyBiodir.Count == 0)
                {
                    IsBioFar = true;
                }
                else
                {
                    if (EnemyBiodir.First().Key > 40f)
                    {
                        IsBioFar = true;
                    }
                }
                // 如果智能体没有移动足够的距离，并且周围最近的敌军都 > 60
                if (moveDistance < minimumMoveDistance && IsBioFar)
                {
                    stuckTime += Time.deltaTime;


                    //if (TankNum == 1)
                    //    debugText1 = "" + (transform.name + "get MMMMM reward, with moveDis: " + moveDistance + " and Fist Enemy: " + Enemydir.First().Key);
                }
                else
                {
                    stuckTime = 0;
                    //if (TankNum == 1 && tankTeam == TankTeam.Tank_Blue)
                    //    print("0000");
                }

                if (stuckTime > 2f)
                {
                    //if (TankNum == 1 && tankTeam == TankTeam.Tank_Blue)
                    //    print("stuck ++" + stuckTime );


                    AddReward(-0.003f);
                    stuckTime = 0;
                }

            }
        }

    }


    private void MovetoEnemy()
    {
        if (Bioflag)
        {

            bool moveto = false;
            float movedDistance = (transform.position - last_position).magnitude;

            if (EnemyBiodir.Count > 0 && vv > 0)
            {
                int i = 0;
                Vector3 planeNormal = new Vector3(0f, 1f, 0f);
                foreach (var enemy in EnemyBiodir)
                {
                    if (i >= EnemyCount) { break; };
                    ManControl Enemy = enemy.Value;
                    // 计算智能体这一帧移动的距离

                    Vector3 directionToEnemy = Enemy.transform.position - transform.position;

                    Vector3 planeForward = transform.forward - Vector3.Dot(transform.forward, planeNormal) * planeNormal;
                    Vector3 planeTowardEnemy = directionToEnemy - Vector3.Dot(directionToEnemy, planeNormal) * planeNormal;

                    if ((Vector3.Angle(planeForward.normalized, planeTowardEnemy.normalized) <= 25)) //0.93 Vector3.Dot(planeForward.normalized, planeTowardEnemy.normalized) > 0.85f
                    {
                        // 判断智能体是否朝向这个敌人移动
                        //moveto = true;
                        if (movedDistance > 0.15f)
                        {
                            if (lastClosestDis.ContainsKey(Enemy)) //如果字典中有这个敌人
                            {
                                if (enemy.Key > 400)
                                {
                                    if (enemy.Key - lastClosestDis[Enemy] < -0.2f) // 判断智能体是否靠近这个敌人  // && rv < 2
                                    {
                                        //if (TankNum == 1)
                                        //    print("move_to_enemy_reward:-0.0001 ->" + Enemy.name);

                                        //AddReward(0.015f * Vector3.Dot(planeForward.normalized, planeTowardEnemy.normalized)); //0.008
                                        AddReward(0.0015f + 0.012f * (25 - Vector3.Angle(planeForward.normalized, planeTowardEnemy.normalized)) / 25);
                                        moveto = true;
                                    }
                                }
                                else
                                {
                                    if (400 - enemy.Key <= 50)  // && rv < 2
                                    {
                                        //AddReward(0.008f);
                                        AddReward(0.002f + 0.015f * (25 - Vector3.Angle(planeForward.normalized, planeTowardEnemy.normalized)) / 25);
                                        moveto = true;
                                    }
                                }

                            }
                            else // 如果字典中没有这个敌人，添加这个敌人
                            {

                                lastClosestDis[Enemy] = enemy.Key;

                            }
                        }
                        else
                        {
                            if (400 - enemy.Key <= 50)  // && rv < 2
                            {
                                AddReward(0.003f + 0.015f * (25 - Vector3.Angle(planeForward.normalized, planeTowardEnemy.normalized)) / 25);
                                moveto = true;
                            }
                        }
                        //lastClosestDis[Enemy] = enemy.Key;

                    }
                    // 更新智能体距离这个敌人的距离
                    lastClosestDis[Enemy] = enemy.Key;
                    ++i;
                }

                last_position = transform.position; // 更新位置以计算下一帧的移动距离


            }

            //如果智能体没有朝向敌人移动，惩罚智能体，修改bug
            if (!moveto && movedDistance > 0.15f)
            {
                AddReward(-0.0035f);
            }


        }

    }

    private void TowertoEnemy()
    {
        bool Turnto = false;

        if (EnemyBiodir.Count > 0 && vv > 0)
        {
            int i = 0;
            Vector3 planeNormal = new Vector3(0f, 1f, 0f);
            foreach (var enemy in EnemyBiodir)
            {
                if (i >= EnemyCount) { break; };
                ManControl Enemy = enemy.Value;
                // 计算智能体这一帧移动的距离

                Vector3 TowerToEnemy = Enemy.transform.position - Tower.position;

                Vector3 planeForward = Tower.forward - Vector3.Dot(Tower.forward, planeNormal) * planeNormal;
                Vector3 planeTowerToEnemy = TowerToEnemy - Vector3.Dot(TowerToEnemy, planeNormal) * planeNormal;

                if ((Vector3.Angle(Tower.forward.normalized, TowerToEnemy.normalized) <= 20)) //0.93 Vector3.Dot(planeForward.normalized, planeTowardEnemy.normalized) > 0.85f
                {
                    // 判断炮筒是否朝向这个敌人
                    Turnto = true;
                    if (TowerToEnemy.magnitude <= 700)
                    {
                        AddReward(0.00003f + 0.0001f * (20 - Vector3.Angle(Tower.forward.normalized, TowerToEnemy.normalized)) / 20);
                    }

                }

                ++i;
            }


        }

        if (!Turnto)
        {
            AddReward(-0.0001f);
        }

    }


    //规则部分
    //朝敌人移动
    void moveTargetEnemy(SortedDictionary<float, ManControl> EnemyBiodir)
    {
        if (EnemyBiodir.Count != 0)
        {
            Vector3 moveTarget = (EnemyBiodir.Count != 0 ? EnemyBiodir.First().Value.transform.position : new Vector3(139.653137f, 5.54664707f, 606.299988f)) - transform.position;
            float t = 0.0f;
            t = Mathf.Clamp01(t + (Time.deltaTime * 1.0f));
            //坦克动作执行代码，转向由Slerp函数执行，前进由man.move函数执行
            //Quaternion targetRotation1 = Quaternion.LookRotation(man.TowerDir);
            //man.Tower.transform.rotation = Quaternion.Lerp(man.Tower.rotation, targetRotation1, t1);

            Quaternion targetRotation = Quaternion.LookRotation(moveTarget);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
            move(MaxSpeed, 0, 0);
        }
    }

    //朝敌人移动
    void moveToCenter()
    {
        Vector3 moveTarget = new Vector3(139.653137f, 5.54664707f, 606.299988f);
        float t = 0.0f;
        t = Mathf.Clamp01(t + (Time.deltaTime * 1.0f));
        //坦克动作执行代码，转向由Slerp函数执行，前进由man.move函数执行
        //Quaternion targetRotation1 = Quaternion.LookRotation(man.TowerDir);
        //man.Tower.transform.rotation = Quaternion.Lerp(man.Tower.rotation, targetRotation1, t1);

        Quaternion targetRotation = Quaternion.LookRotation(moveTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
        move(MaxSpeed, 0, 0);
    }

    //后撤
    void backTargetEnemy(SortedDictionary<float, ManControl> EnemyBiodir)
    {
        Vector3 moveTarget = (EnemyBiodir.Count != 0 ? EnemyBiodir.First().Value.transform.position : new Vector3(139.653137f, 5.54664707f, 606.299988f)) - transform.position;
        Vector3 direction = Quaternion.Euler(0f, 225, 0f) * moveTarget.normalized;
        float t = 0.0f;
        t = Mathf.Clamp01(t + (Time.deltaTime * 1.0f));
        //坦克动作执行代码，转向由Slerp函数执行，前进由man.move函数执行
        //Quaternion targetRotation1 = Quaternion.LookRotation(man.TowerDir);
        //man.Tower.transform.rotation = Quaternion.Lerp(man.Tower.rotation, targetRotation1, t1);
        UnityEngine.Debug.Log("back");
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
        move(MaxSpeed, 0, 0);
        //if (moveTarget.magnitude < 200.0f)
        //    move(-MaxSpeed, 0, 0);
    }

    //避障
    void CacAvoidDir(SortedDictionary<float, ManControl> EnemyBiodir)
    {
        Vector3 TeamMateDir = Vector3.zero, MoveDir1 = Vector3.zero;
        Vector3 moveTarget = (EnemyBiodir.Count != 0 ? EnemyBiodir.First().Value.transform.position : new Vector3(139.653137f, 5.54664707f, 606.299988f)) - transform.position;
        float t = 0.0f;
        t = Mathf.Clamp01(t + (Time.deltaTime * 1.0f));
        //坦克动作执行代码，转向由Slerp函数执行，前进由man.move函数执行
        for (int i = 0; i < tankSpawner.BlueAgentsList.Count; i++)
        {
            float TeamMateDis = Vector3.Distance(transform.position, tankSpawner.BlueAgentsList[i].transform.position);
            //if (man.TankNum == 7) print(TeamMateDis);
            if ((TeamMateDis < 50) && TeamMateDis != 0.0f)
            {
                TeamMateDir = (transform.position - tankSpawner.BlueAgentsList[i].transform.position).normalized /
                        Vector3.Distance(transform.position, tankSpawner.BlueAgentsList[i].transform.position);
            }
            else
            {

                TeamMateDir = Vector3.zero;
            }

            MoveDir1 += TeamMateDir;
        }

        moveTarget = (MoveDir1 + moveTarget / moveTarget.magnitude).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(moveTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
        move(MaxSpeed, 0, 0);
    }

    //发炮
    void openFireEnemy(SortedDictionary<float, ManControl> EnemyBiodir)
    {
        Vector3 target = EnemyBiodir.First().Value.transform.position;
        Vector3 TowerDir = (target - transform.position).normalized;
        Vector3 cannonDir = cannon.forward;

        cannonDir = new Vector3(cannonDir.x, 0, cannonDir.z);
        TowerDir = new Vector3(TowerDir.x, 0, TowerDir.z);

        float angle = Vector3.SignedAngle(TowerDir, cannonDir, Vector3.up);//左正右负

        if (angle > 0.1) Tower.Rotate(-Vector3.up, 0.6f);
        else if (angle < -0.1f) Tower.Rotate(Vector3.up, 0.6f);

        OpenFire(angle, 1);
    }

    //凝聚
    void Concentrate()
    {
        Vector3 moveTarget = CalculateCnter(tankSpawner.BlueAgentsList);
        float t = 0.0f;
        t = Mathf.Clamp01(t + (Time.deltaTime * 1.0f));
        //坦克动作执行代码，转向由Slerp函数执行，前进由man.move函数执行
        //Quaternion targetRotation1 = Quaternion.LookRotation(man.TowerDir);
        //man.Tower.transform.rotation = Quaternion.Lerp(man.Tower.rotation, targetRotation1, t1);

        Quaternion targetRotation = Quaternion.LookRotation(moveTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
        move(MaxSpeed, 0, 0);
    }

    //计算队伍存活个体的中心位置
    public Vector3 CalculateCnter(List<TankControl> tank)
    {
        Vector3 teamCenterPos = Vector3.zero;
        int aliveCount = 0;
        foreach (var it in tank)
        {
            if (!it.Isdead)
            {
                teamCenterPos += it.transform.position;
                aliveCount++;

            }
        }
        return teamCenterPos / aliveCount;
    }

}

