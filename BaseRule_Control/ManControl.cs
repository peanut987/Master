using UnityEngine;
using UnityEngine.UI;
using Unity.MLAgents.Policies;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Linq;

public class ManControl : MonoBehaviour
{
	//用于测试与调试的public变量
	public int pos = -1;
	public float target_dis;
	public int HillIndex = -1;
	public int stopTime = -1;
	public int rotateFlag = -1;
	public bool assignFlag = false;

	public bool fireFlag;
	public int switchtime;
	public int switchlimit = 200;

	public int SameFireCount = 0;
	public int CalFireCount = 0;
	public int CalFireCount1 = 0;
	public int[] MinNumBuffer = { -1, -1 };
	public Vector3 shell_start_pos;
	public Vector3 shell_collider_pos;
	public float bio_enemy_dis = 0;
	public float shell_dis = 0;
	public float left_edge = -1;
	public float right_edge = 1;
	public Vector3 point;
	public float[] optimize_result;
	public float fire_speed_buffer;
	public double shellspeed;
	public double theta;
	public int highFlag = 0;
	public float angle_cannon;
	public float enemyDisXOZ;
	public Vector3 target1;
	public float testFireSpeeed = 1;
	public float SetFireSpeeed = 1;
	public int startFlag = 0;
	public bool speedControl;//控制坦克是否移动，用于测试	

	public float enemyAngle;
	public float enemyAngle1;
	public float setEnemyAngle1;

	public class fireParameter
	{

	}

	[Header("坦克对战相关距离角度设置")]
	public float setAvoidDis = 20.0f;
	public float avoidAngle = 0;
	public float BackDistance;//后撤距离
	public float AmbushDis;
	public float OpenFireDis;//可开火距离
	public float TeammateAvoidDis = 5.0f;//队友避障距离阈值
	public float EnemyAvoidDis = 5.0f;//对手避障距离阈值

	public int Enemy_len = 0;//可视范围内存在的对手数量
	[Header("注意，以下的初始化值并不起效，均需要在Inspector ManControl中进行赋值")]
	//两个常量
	public int EnemyNum;//对手数量
	public int TeamNum;//队友数量

	[Header("坦克基本预制体设置")]
	public GameManage gameManage;
	public Rigidbody _rigidbody;
	public TankTeam tankTeam;
	public LineRenderer lineRenderer;//坦克射线
	public GameObject CannonObject;
	public Material Black;
	public Color SliderBlack;


	[Header("炮弹基本预制体设置")]
	public GameObject shell;//炮弹预制体
	public Transform ShellPos;//炮弹发射坐标
	public int cooldowntime = 80;//炮弹冷却时间
	public UnityEngine.UI.Slider PHSlider;//血条，丘陵场景中已隐藏
	public ParticleSystem tankExplosion;//坦克爆炸效果

	[Header("坦克基本属性设置")]
	public float MaxSpeed = 20;
	public float rotatespeed = 12;
	public float PH = 15;
	public float PHFULL = 15;
	public float vvlue;
	public int TankNum;
	public bool Isdead;
	private float Firevalue;

	[Header("坦克手动控制(用于测试)")]
	public float FireMaxspeed = 12.5f;//手动发炮速度
	public float FireMinspeed = 2.5f;

	[Header("坦克对战数据统计")]
	public int fire = 0;
	public int firetime = 0;
	public int FireSuccessNum = 0;
	public int HurtteammateNum = 0;
	public int kill = 0;
	public int death = 0;
	public int KillSelf = 0;
	public int KillByMate = 0;

	[Header("坦克对战信息")]
	public Vector3 offset;//坦克实际运行方向向量
	public Vector3 CannonDir;//炮筒方向向量
	public Vector3 TowerDir;//炮台方向向量
	public float relativespeed;
	public float relativerotate;
	public int EnemyAttackNum = 0;//记录坦克被几个对手锁定
	public int enemylive;//对手场上存活数量
	public int teammatelive;//队友场上存活数量
	public int MinNum;//最终选择攻击的对手ID

	public int LastMinNum;//上一次对手的ID
	public int ResultMinNum;//通过EnemyJudge()算法判断出的对手ID
							//[HideInInspector]
	public int CountMinNum = 0;//锁定对手计数标志位，打完一轮后需要在gamemanage文件的reset函数内重置该标志位

	public int RegularFlag = 0;//坦克拉扯攻击标志位
	public float[] TeamMateSingleRot;//队友角度
	public float[] TeamMateRot;//队友角度
	public float[] Enemyinf = new float[3];//存储的三个变量分别表示对手前后位置、对手相对于自身炮筒角度、自身相对于对手炮筒角度
	public float[] EnemyDis;//
	public float[] EnemyRot;
	public float[] TeamMateDis;//队友距离

	[Tooltip("对手与队友字典")]
	public SortedDictionary<float, TankControl> BioEnemydir = new();
	public SortedDictionary<float, TankControl> BioEnemydir1 = new();
	public SortedDictionary<float, ManControl> BioTeanMateDir = new();
	public SortedDictionary<float, ManControl> BioSameEnemyDir = new();
	public SortedDictionary<float, ManControl> BioSameHighDir = new();
	public SortedDictionary<float, ManControl> BioAllTeammateDir = new();

	//算法比较时用的列表
	public SortedDictionary<float, ManControl> TAEnemydir = new();
	public SortedDictionary<float, ManControl> TAEnemydir1 = new();
	public SortedDictionary<float, ManControl> BioEnemydirTA = new();
	//论文对照算法使用到的变量
	//public int[] enemyList;
	//public Vector3 TATankCenter;
	//public SortedDictionary<float, ManControl> SortEnemyDis = new();
	//public SortedDictionary<float, ManControl> TASameEnemyDir = new();



	[HideInInspector]
	public float timeBetween = 0;//两侧坦克锁定敌军前运行时间，在gamemanage的ResetScene()里面置0
	public float timeBetween1 = 0;

	[Tooltip("发炮控制标志位，用于测试")]
	public bool ControlFlag = true;


	[Tooltip("被选中标志位")]
	public bool isSelected = false;

	//切换到手动控制模式Heuristic()
	private string inputSpeedlStr;
	private string inputRotatelStr;
	private string inputFirestr;
	public float Loading_time = 0.3f;
	public float fireSpeed = 0;

	public Image fillImage;
	public TextMeshPro NUM_Text;
	public Color TextColor1;
	public Color TextColor2;
	private Transform childTransform;


	//创建类的实例化对象
	public TranningSetting trainingSetting;
	public ObstacleAvoid obstacleAvoid2;
	public BaseFunction baseFunction2;
	public InfoCalculate infoCalculate2;
	public EnemyJudge enemyJudge2;
	public FindEnemy findEnemy2;
	public BaseRules baseRules2;
	public TankSpawner tankSpawner;
	public Cannon cannon_script2;
	public Tower2 tower_script;
	//public ShellControl shell_script;

	public bool isNavigate = true;
	public Transform target;
	public float NavigationDis = 60f;
	public bool enemyfromback = false;
	public int team;
	public bool isLeader = false;
	//public int startFlag = 0;

	[Tooltip("改变重力")]
	private float gravity_Y = 0;

	public List<float> teamdis;


	private RaycastHit hit;
	void Start()
	{
		MinNum = -1;
		TankInitialization();
		//enemyList = new int[trainingSetting.BlueTeam.nums];
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.startWidth = 1.0f;
		lineRenderer.endWidth = 1.0f;
		SetFireSpeeed = -1;
		MinNumBuffer = new int[] { -1, -1 };
		//UnityEngine.Debug.Log("tankSpawner.Biolist" + tankSpawner.Biolist.Count);
		StartCoroutine(Coroutine());

	}
	IEnumerator Coroutine()
	{
		yield return new WaitForSeconds(1f);
	}

	void FixedUpdate()
	{
		//改变y轴的重力效果
		gravity_Y += Physics.gravity.y * Time.deltaTime;
		gravity_Y = Mathf.Clamp(gravity_Y, Physics.gravity.y, 0);
		if (!Isdead)
		{
			if (tankTeam == TankTeam.Tank_Red)
			{
				//计算基本信息
				if (MinNum != -1 && MinNum != 0)
				{
					if (tankSpawner.useTA && tankSpawner.TAList[MinNum - 1].Isdead) switchtime = switchlimit + 1;
					else if (!tankSpawner.useTA && tankSpawner.BlueAgentsList[MinNum - 1].Isdead) switchtime = switchlimit + 1;
				}
				infoCalculate2.Calculate(this, transform);
				if (!trainingSetting.RedTeam.HumanControl && (MinNum == -1 || switchtime > switchlimit))
				{
					switchtime = 0;
					enemyJudge2.Judge(this);
				}
				if (trainingSetting.RedTeam.HumanControl)
				{
					if (BioEnemydir.Count != 0)
					{
						MinNum = BioEnemydir.First().Value.TankNum + 1;
					}

				}
			}
			else
			{
				//if (tankTeam == TankTeam.Tank_Blue)
				//MinNum = enemyJudge2.enemyList[TankNum];
			}

			if (gameManage.round <= 100 || trainingSetting.EnvInfo.isTrainning)
			{
				if (!trainingSetting.RedTeam.HumanControl)
				{
					if (tankTeam == TankTeam.Tank_Red) findEnemy2.Scout(this, transform);
					else findEnemy2.TAScout(this, transform);
				}
				else
				{
					ManControlTank();
				}
			}
			BioGround();//防止坦克浮空

			//parameterTest();
			firetime += 1;
			switchtime += 1;
		}
	}

	void BioGround()
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

	void ManControlTank()
	{
		if (TankNum - 1 == 0)
		{
			// Handling movement
			if (Input.GetKey(KeyCode.W))
			{
				move(MaxSpeed, 0); // Move forward
			}
			if (Input.GetKey(KeyCode.S))
			{
				move(-MaxSpeed, 0); // Move backward
			}
			if (Input.GetKey(KeyCode.A))
			{
				move(0, -0.25f * rotatespeed); // Rotate left
			}
			if (Input.GetKey(KeyCode.D))
			{
				move(0, 0.25f * rotatespeed); // Rotate right
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
		else if (TankNum - 1 == 1)
		{
			if (Input.GetKey(KeyCode.T))
			{
				move(MaxSpeed, 0); // Move forward
			}
			if (Input.GetKey(KeyCode.G))
			{
				move(-MaxSpeed, 0); // Move backward
			}
			if (Input.GetKey(KeyCode.F))
			{
				move(0, -0.25f * rotatespeed); // Rotate left
			}
			if (Input.GetKey(KeyCode.H))
			{
				move(0, 0.25f * rotatespeed); // Rotate right
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
		else if (TankNum - 1 == 2)
		{
			if (Input.GetKey(KeyCode.I))
			{
				move(MaxSpeed, 0); // Move forward
			}
			if (Input.GetKey(KeyCode.K))
			{
				move(-MaxSpeed, 0); // Move backward
			}
			if (Input.GetKey(KeyCode.J))
			{
				move(0, -0.25f * rotatespeed); // Rotate left
			}
			if (Input.GetKey(KeyCode.L))
			{
				move(0, 0.25f * rotatespeed); // Rotate right
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
		else if (TankNum - 1 == 3)
		{
			if (Input.GetKey(KeyCode.UpArrow))
			{
				move(MaxSpeed, 0); // Move forward
			}
			if (Input.GetKey(KeyCode.DownArrow))
			{
				move(-MaxSpeed, 0); // Move backward
			}
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				move(0, -0.25f * rotatespeed); // Rotate left
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				move(0, 0.25f * rotatespeed); // Rotate right
			}
			if (Input.GetKey(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.M))
			{
				fireFlag = true;
			}
			else
			{
				fireFlag = false;
			}
		}
		else if (TankNum - 1 == 4)
		{
			if (Input.GetKey(KeyCode.Alpha8))
			{
				move(MaxSpeed, 0); // Move forward
			}
			if (Input.GetKey(KeyCode.Alpha6))
			{
				move(-MaxSpeed, 0); // Move backward
			}
			if (Input.GetKey(KeyCode.Alpha7))
			{
				move(0, -0.25f * rotatespeed); // Rotate left
			}
			if (Input.GetKey(KeyCode.Alpha9))
			{
				move(0, 0.25f * rotatespeed); // Rotate right
			}
			if (Input.GetKey(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.B))
			{
				fireFlag = true;
			}
			else
			{
				fireFlag = false;
			}
		}

		float shellAngle;
		Vector3 target = tankSpawner.BlueAgentsList[MinNum - 1].transform.position;
		Vector3 newTarget = new Vector3(target.x, target.y + 1.5f, target.z);
		newTarget = newTarget - ShellPos.position;
		shellAngle = Vector3.Angle(newTarget, ShellPos.forward);
		shellAngle = newTarget.normalized.y > cannon.transform.forward.normalized.y ?
					shellAngle : -shellAngle;
		//将炮筒旋转角度限制在0 - 35度
		shellAngle = shellAngle < 25.0f ? shellAngle : 25.0f;
		shellAngle = shellAngle > -5.0f ? shellAngle : -5.0f;
		OpenFire3(1, shellAngle, 1);
	}

	void parameterTest()
	{
		//_rigidbody.AddForce(new Vector3(0, -1000, 0));
		//OpenFire(1,1);
		//cannon.Rotate(Vector3.up, 0.2f);
		//OpenFire(SetFireSpeeed, 1);
		//shell_dis = Vector3.Distance(shell_start_pos,shell_collider_pos);
		if (firetime < 100 && firetime >= 0 && setEnemyAngle1 == 1)
		{
			if (firetime == 1) shell_start_pos = transform.position;
			move(MaxSpeed, 0);
			//cannon.Rotate(-Vector3.right, 0.6f);
		}
		else if (firetime >= 100 && firetime < 200)
		{
			//shell_dis = Vector3.Angle(transform.forward,cannon.forward);
			setEnemyAngle1 = 0.0f;

			if (firetime == 100) shell_collider_pos = transform.position;
			//shell_dis = Vector3.Distance(shell_start_pos, shell_collider_pos);
			//move(-MaxSpeed, 0);
		}
		else
			firetime = 0;
		//OpenFire1(1, setEnemyAngle1, 1);
	}

	public void Ray_Cast(Vector3 Satrt, Vector3 End)
	{
		// 射线的起点和方向
		Vector3 start = new Vector3(Satrt.x, Satrt.y, Satrt.z);
		Vector3 direction = new Vector3(End.x, End.y, End.z) - start;

		// 发射射线
		Ray ray = new Ray(start, direction);
		int Rmask = LayerMask.GetMask("Tank");
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 1000, Rmask))
		{
			lineRenderer.enabled = true;
			// 如果射线碰撞到物体，绘制从起点到碰撞点的线段
			lineRenderer.SetPosition(0, start);
			lineRenderer.SetPosition(1, hit.point);
		}
		else
		{
			//// 如果射线未碰撞到物体，绘制从起点到指定长度处的线段
			lineRenderer.enabled = false;
			lineRenderer.SetPosition(0, start);
			lineRenderer.SetPosition(1, start + direction * 15.0f);
		}
	}

	public void Muti_Ray_Cast(Vector3 Satrt, SortedDictionary<float, TankControl> EnemyList)
	{
		for (int i = 0; i < EnemyList.Count; i++)
		{
			Vector3 End = EnemyList.ElementAt(i).Value.transform.position;
			// 射线的起点和方向
			Vector3 start = new Vector3(Satrt.x, Satrt.y, Satrt.z);
			Vector3 direction = new Vector3(End.x, End.y, End.z) - start;

			lineRenderer.positionCount = EnemyList.Count * 2;
			lineRenderer.startWidth = 0.1f;
			lineRenderer.endWidth = 0.1f;
			// 发射射线
			Ray ray = new Ray(start, direction);
			int Rmask = LayerMask.GetMask("Tank");
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 1000, Rmask))
			{
				lineRenderer.enabled = true;
				// 如果射线碰撞到物体，绘制从起点到碰撞点的线段
				lineRenderer.SetPosition(i * 2, start);
				lineRenderer.SetPosition(i * 2 + 1, hit.point);
			}
			else
			{
				//// 如果射线未碰撞到物体，绘制从起点到指定长度处的线段
				lineRenderer.enabled = false;
			}
		}
	}


	public Transform cannon;
	public Transform Tower;
	//public 
	private int cannonRotSpeed = 0;

	//移动代码
	public void move(float v_value, float h_value)
	{
		Vector3 angle = transform.eulerAngles;
		int Rmask = LayerMask.GetMask("GroundLayer");
		if (baseFunction2.angleOffset(angle.x) > -5)
		{
			MaxSpeed = 20;
			_rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
		}
		else
		{
			_rigidbody.freezeRotation = false;
		}
		vvlue = v_value;
		if (v_value > MaxSpeed) vvlue = MaxSpeed;
		else if (v_value < -MaxSpeed) vvlue = -MaxSpeed;

		//通过射线检测地面调整角度
		if (Physics.Raycast(transform.position, -transform.up, out hit, 25.0f, Rmask))  //&& (baseFunction.angleOffset(angle.x) > -5 || baseFunction.angleOffset(angle.x) < -35))
		{
			//Quaternion NextRot = Quaternion.LookRotation(Vector3.Cross(hit.normal, Vector3.Cross(transform.forward, hit.normal)), hit.normal);
			//Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
			//transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 30f);

			// 获取地面法线
			Vector3 groundNormal = hit.normal;

			// 使物体与地面平行
			transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;

		}

		//直接赋值速度方式实现移动
		//_rigidbody.velocity = transform.forward * (v_value);
		//_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, gravity_Y, _rigidbody.velocity.z);

		//力的方式实现移动
		Vector3 moveDirection = transform.forward * v_value * 0.6f;
		//_rigidbody.AddForce(moveDirection, ForceMode.Acceleration);
		_rigidbody.AddForce(moveDirection, ForceMode.VelocityChange);

		//限制最大速度为45
		if (_rigidbody.velocity.magnitude > 20)
		{
			_rigidbody.velocity = _rigidbody.velocity.normalized * 20 * 0.6f;
		}
		else if (_rigidbody.velocity.magnitude > 20 * 2 / 3 && v_value < 0)
		{
			_rigidbody.velocity = _rigidbody.velocity.normalized * 20 * 2 / 3 * 0.6f;
		}

		if (h_value != 0)
		{
			transform.Rotate(h_value * rotatespeed * Time.deltaTime * Vector3.up);
		}

	}

	//发射炮弹代码(炮筒可旋转)
	public void OpenFire(float shellSpeed, float shellAngle, int flags)
	{
		//将炮筒旋转角度限制在0 - 35度
		shellAngle = shellAngle < 25.0f ? shellAngle : 25.0f;
		shellAngle = shellAngle > -5.0f ? shellAngle : -5.0f;

		//炮筒旋转
		if (-(baseFunction2.angleOffset(cannon.localRotation.eulerAngles.x)) >= -5.0f)
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
		if (-(baseFunction2.angleOffset(cannon.localRotation.eulerAngles.x)) <= 25.0f)
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
		if (shell != null && flags == 1 && (firetime >= cooldowntime) && fire <= 40 &&
			(cannon_script2.ColliderFlag))//0.1为一个补偿值，因为炮筒角度不能真正达到设定角度，只能无限逼近
		{
			CalFireCount = 0;
			CalFireCount1 = 0;
			shell_collider_pos = Vector3.zero;
			SameFireCount++;
			GameObject shellObj = Instantiate(shell, ShellPos.position, ShellPos.transform.rotation);
			Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
			ShellControl shell_script = shellObj.GetComponent<ShellControl>();
			shell_collider_pos = Vector3.zero;
			shell_start_pos = ShellPos.position;//记录炮弹发射时的坐标			


			if (shellRigidbody != null)
			{
				shellRigidbody.velocity = ShellPos.forward * 1400;
				shellObj.GetComponent<ShellControl>().Set_father(gameObject);
			}
			++fire;
			firetime = 0;
		}
	}

	//发射炮弹代码(炮筒可旋转)
	public void OpenFire3(float shellSpeed, float shellAngle, int flags)
	{
		//将炮筒旋转角度限制在0 - 35度
		shellAngle = shellAngle < 25.0f ? shellAngle : 25.0f;
		shellAngle = shellAngle > -5.0f ? shellAngle : -5.0f;

		//炮筒旋转
		if (-(baseFunction2.angleOffset(cannon.localRotation.eulerAngles.x)) >= -5.0f)
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
		if (-(baseFunction2.angleOffset(cannon.localRotation.eulerAngles.x)) <= 25.0f)
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
		if (shell != null && flags == 1 && (firetime >= cooldowntime) && fire <= 40 && 
			enemyAngle1 < 0.5f)//0.1为一个补偿值，因为炮筒角度不能真正达到设定角度，只能无限逼近
		{
			CalFireCount = 0;
			CalFireCount1 = 0;
			shell_collider_pos = Vector3.zero;
			SameFireCount++;
			GameObject shellObj = Instantiate(shell, ShellPos.position, ShellPos.transform.rotation);
			Rigidbody shellRigidbody = shellObj.GetComponent<Rigidbody>();
			ShellControl shell_script = shellObj.GetComponent<ShellControl>();
			shell_collider_pos = Vector3.zero;
			shell_start_pos = ShellPos.position;//记录炮弹发射时的坐标			


			if (shellRigidbody != null)
			{
				shellRigidbody.velocity = ShellPos.forward * 1400;
				shellObj.GetComponent<ShellControl>().Set_father(gameObject);
			}
			++fire;
			firetime = 0;
		}

	}

	//炮弹造成伤害代码
	public void ShellDamage(float damage)
	{
		if (Isdead) return;
		if (tankTeam == TankTeam.Tank_Red) gameManage.BIO_PH_Loss += damage;
		if (PH > 0)
		{

			PH -= damage;
			PHSlider.value = PH;
		}
		//print(storeDeadPos.FireEnemy.First().Key.TankNum + " "+ storeDeadPos.FireEnemy.First().Value.TankNum);
		if (PH <= 0)
		{
			PH = 0;

			if (tankExplosion != null)
			{
				tankExplosion.transform.parent = null;
				tankExplosion.Play();
				Destroy(tankExplosion.gameObject, tankExplosion.main.duration);
			}
			++death;
			//Isdead = true;
			//gameManage.num_Red--;
			//gameObject.SetActive(false);
			if (tankTeam == TankTeam.Tank_Red) gameManage.BIO_Dead_Num++;
			Isdead = true;
			gameManage.tankSpawner.ChangeManColor(this, Black);
			NUM_Text.color = SliderBlack;
			if (tankTeam == TankTeam.Tank_Red) gameManage.num_Red--;
			else gameManage.num_Blue--;


		}
	}

	//根据右键点击的位置判断接下来的锁定目标
	public void MoveToTarget(Vector3 target_point)
	{
		int TempMinNum = 0; float MinDis = 100;
		foreach (var itemID in tankSpawner.BlueAgentsList)
		{
			ManControl item = tankSpawner.BlueAgentsList[MinNum - 1].GetComponent<ManControl>();
			float TempDis = (item.transform.position - target_point).magnitude;
			if (!item.Isdead && TempDis < MinDis)
			{
				MinDis = TempDis;
				TempMinNum = item.TankNum;
			}
		}
		float point_dis = baseFunction2.CalculateDisX0Z(this.transform.position, target_point);//计算坦克到标定点的距离
		if (trainingSetting.RedTeam.HumanControl)
		{
			if (point_dis < 10)//到达标定点后停车
			{
				relativespeed = 0.0f;
				TeammateAvoidDis = 0;
			}
			else
				relativespeed = 1.0f;

			//移动函数
			offset = obstacleAvoid2.CacPosition(this, transform, target_point, 180);//通过避障函数求出运行方向
			offset[1] = 0;//令y方向为0

			float t = 0.0f;
			t = Mathf.Clamp01(t + (Time.deltaTime * 3.0f));

			//坦克动作执行代码，转向由Slerp函数执行，前进由man.move函数执行
			Quaternion targetRotation = Quaternion.LookRotation(offset);
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
			if (point_dis > OpenFireDis)
			{
				move(MaxSpeed, 0);
			}
			//else move(0, 0);


			//findEnemy2.attack_point(this, target_point);

		}
	}

	////被框选的物体颜色改变
	//public void ChangeColor()
	//{
	//	if (isSelected != true && childTransform != null)
	//	{
	//		TextMeshProUGUI tmp = childTransform.GetComponent<TextMeshProUGUI>();
	//		// 创建一个新的材质实例
	//		Material newMaterial = new Material(tmp.fontSharedMaterial);
	//		// 改变新材质的颜色
	//		newMaterial.SetColor("_FaceColor", new Color(100,0,255,0));
	//		// 应用新的材质
	//		tmp.fontSharedMaterial = newMaterial;
	//	}
	//}

	// 限制角度在指定范围内的辅助函数
	public float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f) angle += 360f;
		if (angle > 360f) angle -= 360f;
		return Mathf.Clamp(angle, min, max);
	}

	private void TankInitialization()
	{
		//在实例化时给坦克添加脚本
		findEnemy2 = FindObjectOfType<FindEnemy>();
		enemyJudge2 = FindObjectOfType<EnemyJudge>();
		infoCalculate2 = FindObjectOfType<InfoCalculate>();
		baseFunction2 = FindObjectOfType<BaseFunction>();
		obstacleAvoid2 = FindObjectOfType<ObstacleAvoid>();
		gameManage = FindObjectOfType<GameManage>();
		baseRules2 = FindObjectOfType<BaseRules>();
		trainingSetting = FindObjectOfType<TranningSetting>();
		tankSpawner = FindObjectOfType<TankSpawner>();
		Tower = transform.Find("TankRenderers/TankFree_Tower");
		cannon = transform.Find("TankRenderers/TankFree_Tower/TankFree_Canon");
		CannonObject = cannon.gameObject;
		cannon_script2 = CannonObject.GetComponent<Cannon>();
		tower_script = Tower.GetComponent<Tower2>();
		MaxSpeed = trainingSetting.RedTeam.MaxSpeed;
		rotatespeed = trainingSetting.RedTeam.rotatespeed;

		_rigidbody = gameObject.GetComponent<Rigidbody>();
		tankTeam = transform.tag == "Tank_Blue" ? TankTeam.Tank_Blue : TankTeam.Tank_Red;
		if (trainingSetting.RedTeam.nums == 3 && trainingSetting.BlueTeam.nums == 5 && tankTeam == TankTeam.Tank_Red)
		{
			PHFULL = trainingSetting.RedTeam.FULLPH * 2;
		}
		else if (trainingSetting.RedTeam.nums == 5 && trainingSetting.BlueTeam.nums == 3 && tankTeam == TankTeam.Tank_Blue)
		{
			PHFULL = trainingSetting.BlueTeam.FULLPH * 2;
		}
		else if (trainingSetting.RedTeam.nums == 3 && trainingSetting.BlueTeam.nums == 4)
		{
			if (tankTeam == TankTeam.Tank_Red) PHFULL = trainingSetting.RedTeam.FULLPH * 3;
			else PHFULL = trainingSetting.RedTeam.FULLPH * 2;
		}
		else if (trainingSetting.RedTeam.nums == 4 && trainingSetting.BlueTeam.nums == 3)
		{
			if (tankTeam == TankTeam.Tank_Red) PHFULL = trainingSetting.RedTeam.FULLPH * 2;
			else PHFULL = trainingSetting.RedTeam.FULLPH * 3;
		}
		else
		{
			PHFULL = trainingSetting.RedTeam.FULLPH;
		}
		PHSlider.maxValue = PH;
		PHSlider.value = PH;

		//变量初始化
		EnemyNum = trainingSetting.BlueTeam.nums;
		TeamNum = trainingSetting.RedTeam.nums;

		EnemyDis = new float[EnemyNum];
		EnemyRot = new float[EnemyNum];
		TeamMateDis = new float[TeamNum];
		TeamMateRot = new float[TeamNum];
		TeamMateSingleRot = new float[TeamNum]; ;
		optimize_result = new float[2];

		BackDistance = 400.0f;
		OpenFireDis = 2000.0f;
		switchlimit = 100;
		setEnemyAngle1 = -1;
		isNavigate = true;
		speedControl = true;
		cooldowntime = 300;

		MinNum = 1;  //防止初始化数组越界

		childTransform = transform.Find("Text");

		//手动输入
		inputSpeedlStr = "inputSpeedlStr" + TankNum;
		inputRotatelStr = "inputRotatelStr" + TankNum;
		inputFirestr = "inputFirestr" + TankNum;

		//手动控制和自动控制
		if (trainingSetting.RedTeam.HumanControl) TeammateAvoidDis = 5.0f;
		else TeammateAvoidDis = 100.0f;
	}

	// 设置该单位是否被选中
	public void SetSelected(bool selected)
	{
		isSelected = selected;
	}

	// 检查该单位是否被选中
	public bool IsSelected()
	{
		return isSelected;
	}

	// 设置该单位是否被选中
	public void SetTeam(int teamIndex)
	{
		team = teamIndex;
	}

	// 检查该单位是否被选中
	public int GetTeam()
	{
		return team;
	}
}

