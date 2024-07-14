using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoid : MonoBehaviour
{
    public TankSpawner tankSpawner;
    public Transform obstacleRoot;
    public TranningSetting trainingSetting;
    public int teamnum, enemynum;
    public float[] roundTime = new float[4];
    public float[] angle = new float[4];//对局开始时个体向左或者向右的转向角度
    public float[] dis = new float[3];
    public float[] speed = new float[2];
    private RaycastHit hit;
    //private Vector3 MoveDir, MoveDir1, TeamMateDir, OtherEnemyDir = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        tankSpawner = FindObjectOfType<TankSpawner>();
        trainingSetting = FindObjectOfType<TranningSetting>();
        teamnum = trainingSetting.RedTeam.nums;
        enemynum = trainingSetting.BlueTeam.nums;
        setParameter(trainingSetting.RedTeam.nums, trainingSetting.BlueTeam.nums, tankSpawner.useTA, trainingSetting.algorithmSelect.BioOptimized);
        //if (teamnum == 3)
        //    roundTime = new float[] { 22, 20, 25, 22 };
        //else if(teamnum > 3)
        //    roundTime = new float[] { 5, 20, 15, 5 };//91:6 5、15、5、5
         

    }

     public void setParameter(int redNum, int blueNum, bool isNR, bool isOptimized)
    {
        if(isNR)
        {
            if (redNum == 3 && blueNum == 3)
            {
                if(isOptimized)
                {
                    roundTime = new float[] { 20, 20, 22, 25 };
                    angle = new float[] { -60, 60, 70, -70 };
                    dis = new float[] { 800, 100, 300 };
                    speed = new float[] { 0.2f, 0.2f };
                }
                else
                {
                    //roundTime = new float[] { 15, 20, 20, 22 };
                    //angle = new float[] { -60, 60, 45, -45 };
                    //dis = new float[] { 800, 100, 200 };
                    //speed = new float[] { 0.2f, 0.2f };
                    roundTime = new float[] { 30, 30, 30, 30 };
                    angle = new float[] { -0, 0, 0, -0 };
                    dis = new float[] { 100, 100, 300 };
                    speed = new float[] { 0.2f, 0.2f };
                }
            }
            else if (redNum == 3 && blueNum == 4)
            {
                if (isOptimized)
                {
                    //roundTime = new float[] { 15, 20, 25, 25 };
                    //angle = new float[] { -60, 60, 60, -60 };
                    ////dis = new float[] { 800, 100, 300 };
                    //dis = new float[] { 1000, 200, 300 };
                    //speed = new float[] { 0.2f, 0.2f };
                    roundTime = new float[] { 25, 15, 15, 15 };
                    angle = new float[] { -45, 45, 60, -60 };
                    dis = new float[] { 800, 100, 200 };
                    speed = new float[] { 0.1f, 0.1f };
                }
                else
                {
                    roundTime = new float[] { 15, 20, 25, 25 };
                    angle = new float[] { -60, 60, 45, -45 };
                    dis = new float[] { 800, 100, 100 };
                    speed = new float[] { 0.2f, 0.2f };
                }
            }
            else if (redNum == 3 && blueNum == 5)
            {
                if (isOptimized)
                {
                    roundTime = new float[] { 25, 15, 15, 20 };
                    angle = new float[] { -60, 60, 70, -70 };
                    dis = new float[] { 1000, 400, 200 };
                    speed = new float[] { 0.1f, 0.2f };
                    //roundTime = new float[] { 25, 15, 15, 20 };
                    //angle = new float[] { -60, 60, 70, -70 };
                    //dis = new float[] { 800, 100, 200 };
                    //speed = new float[] { 0.1f, 0.2f };
                }
                else
                {
                    roundTime = new float[] { 15, 20, 20, 22 };
                    angle = new float[] { -60, 60, 45, -45 };
                    dis = new float[] { 800, 100, 200 };
                    speed = new float[] { 0.2f, 0.2f };
                }
            }
            else if (redNum == 4 && blueNum == 3)
            {
                if (isOptimized)
                {
                    roundTime = new float[] { 5, 20, 15, 5 };
                    angle = new float[] { -45, 30, 45, -45};
                    dis = new float[] { 1000, 200, 200 };
                    speed = new float[] { 0.1f, 0.2f };
                }
                else
                {
                    roundTime = new float[] { 22, 20, 25, 25 };
                    angle = new float[] { -60, 30, 70, -70 };
                    dis = new float[] { 800, 100, 200 };
                }
            }
            else if (redNum == 5 && blueNum == 3)
            {
                if (isOptimized)
                {
                    roundTime = new float[] { 15, 20, 15, 15 }; ;
                    angle = new float[] { -60, 60, 70, -70 };
                    //dis = new float[] { 800, 100, 150 };
                    dis = new float[] { 1000, 200, 200 };
                    speed = new float[] { 0.1f, 0.2f };
                }
                else
                {
                    roundTime = new float[] { 22, 20, 25, 25 };
                    angle = new float[] { -60, 45, 70, -70 };
                    dis = new float[] { 800, 100, 300 };
                    speed = new float[] { 0.2f, 0.2f };
                }
            }
            else
            {
                if (isOptimized)
                {
                    roundTime = new float[] { 25, 25, 25, 25 };
                    angle = new float[] { -60, 60, 45, -45 };
                    dis = new float[] { 1000, 400, 200 };
                    speed = new float[] { 1.0f, 1.0f };
                    //roundTime = new float[] { 5, 20, 5, 5 };
                    //angle = new float[] { -60, 60, 45, -45 };
                    //dis = new float[] { 800, 100, 200 };
                    //speed = new float[] { 0.1f, 0.2f };
                    //roundTime = new float[] { 25, 20, 20, 20 };
                    //angle = new float[] { -60, 60, 45, -45 };
                    //dis = new float[] { 1000, 200, 200 };
                    //speed = new float[] { 0.1f, 0.2f };
                }
                else
                {
                    roundTime = new float[] { 22, 20, 25, 25 };
                    angle = new float[] { -60, 45, 70, -70 };
                    dis = new float[] { 800, 100, 300 };
                    speed = new float[] { 0.2f, 0.2f };

                }
            }
        }
        else
        {
            dis[0] = 800;
            dis[1] = 100;
            angle = new float[] { 60, -60, 45, -45 };
            speed = new float[] {0.1f, 0.2f};
            if (redNum == 3)
                roundTime = new float[] { 22, 20, 25, 22 };  //表示规则方在右上、右下、左下、右上位置时两侧坦克开局时向两侧移动的时间（22,20,25,22）
            else if (redNum > 3)
                roundTime = new float[] { 5, 20, 5, 5 };//91:6 5、15、5、5
            if ((redNum == 3 && blueNum == 4) || (redNum == 4 && blueNum == 3))
            {
                if (isOptimized)
                {
                    dis[2] = 300;
                }
                else
                {
                    dis[2] = 100;
                }
            }
            else
            {
                if (isOptimized)
                {
                    dis[2] = 500;
                }
                else
                {
                    dis[2] = 50;
                }
            }
        }
    }

    //通过计算每个障碍物对坦克的合力求出运行方向，如果不再避障范围，则作用力为零，距离越小作用力越大
    public Vector3 CacPosition(ManControl man, Transform transform, Vector3 target, float avoidAngle)
    {
        bool isEnmey = true;
        Vector3 MoveDir = Vector3.zero, MoveDir1 = Vector3.zero, TeamMateDir = Vector3.zero, OtherEnemyDir = Vector3.zero, EnemyDir = Vector3.zero;
        if (target == (tankSpawner.useTA ? tankSpawner.TAList[man.MinNum - 1].transform.position :
             tankSpawner.BlueAgentsList[man.MinNum - 1].transform.position))
            isEnmey = true;
        else
            isEnmey = false;

        man.target_dis = isEnmey ? Vector3.Distance(target, transform.position) : 3000.0f;

        

        if(trainingSetting.RedTeam.nums == 3 && trainingSetting.BlueTeam.nums == 5)
        {
            if ((GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[1] && man.gameManage.round % 4 == 1)
            || (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[2] && man.gameManage.round % 4 == 2))
            {
                
                switch (man.findEnemy2.judgeSelfPosToCenter(man))
                {
                    case 1:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, -45, 30, 0);
                        man.relativespeed = 1.0f;
                        break;
                    case 2:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 45, 30, 0);
                        man.relativespeed = 1.0f;
                        break;
                    default:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 0, 30, 0);
                        man.relativespeed = speed[0];
                        break;
                }
            }
            else if ((GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[0] && man.gameManage.round % 4 == 0)
                    || (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[3] && man.gameManage.round % 4 == 3))
            {
                switch (man.findEnemy2.judgeSelfPosToCenter(man))
                {
                    case 1:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, angle[0], 30, 0);
                        man.relativespeed = 1.0f;
                        break;
                    case 2:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, angle[1], 30, 0);
                        man.relativespeed = 1.0f;
                        break;
                    default:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 0, 30, 0);
                        man.relativespeed = speed[0];
                        break;
                }
            }
            else
                man.relativespeed = 1.0f;

        }
        else
        {
            if ((GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[0] && man.gameManage.round % 4 == 0)
    || (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[1] && man.gameManage.round % 4 == 1)
    || (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[2] && man.gameManage.round % 4 == 2)
    || (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[3] && man.gameManage.round % 4 == 3))
            {
                switch (man.findEnemy2.judgeSelfPosToCenter(man))
                {
                    case 1:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, angle[0], 30, 0);
                        break;
                    case 2:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, angle[1], 30, 0);
                        break;
                    default:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 0, 30, 0);
                        man.relativespeed = speed[0];
                        break;
                }
            }
            else
                man.relativespeed = 1.0f;
        }

        if ((target == (!tankSpawner.useTA ? tankSpawner.BlueAgentsList[man.MinNum - 1].transform.position : tankSpawner.TAList[man.MinNum - 1].transform.position)) && isEnmey)
        {
            if (((man.target_dis < man.BackDistance && man.target_dis > man.BackDistance - 50) || man.rotateFlag == 1 || (man.firetime < man.cooldowntime && man.enemyDisXOZ < dis[0] && man.target_dis > man.BackDistance)))
            {
                switch (man.findEnemy2.judgeSelfPosToCenter(man))
                {
                    case 1:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, -90, 30, 0);
                        break;
                    case 2:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 90, 30, 0);
                        break;
                    default:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 0, 30, 0);
                        man.relativespeed = speed[1];
                        break;
                }
            }
            else if ((man.firetime < man.cooldowntime && man.target_dis < man.BackDistance))
            {
                man.relativespeed = -1.0f;
                switch (man.findEnemy2.judgeSelfPosToCenter(man))
                {
                    case 1:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, angle[2], 30, 0);
                        break;
                    case 2:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, angle[3], 30, 0);
                        break;
                    default:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 0, 30, 0);
                        break;
                }

            }
        }

        if (target == (tankSpawner.useTA ? tankSpawner.TAList[man.MinNum - 1].transform.position :
     tankSpawner.BlueAgentsList[man.MinNum - 1].transform.position))
            isEnmey = true;
        else
            isEnmey = false;

        if (((man.target_dis < man.BackDistance - dis[1] && man.firetime < 300 && Mathf.Max(man.TeamMateRot) < 90 ) ||
        (man.speedControl == true && man.enemyDisXOZ < dis[2] && man.firetime > 300)) && man.rotateFlag != 1 && isEnmey)
            man.relativespeed = -1.0f;
        EnemyDir = (target - transform.position).normalized;

        //求队友对自身的合力
        for (int i = 0; i < teamnum; i++)
        {
            float TeamMateDis = i == man.TankNum - 1 ?  10000.0f : man.baseFunction2.CalculateDisX0Z(man.transform.position, tankSpawner.Biolist[i].transform.position);//man.TeamMateDis1[i];
            float angle = man.baseFunction2.DotCalculate5(man.transform, tankSpawner.Biolist[i].transform);
            float position_y = Mathf.Abs(transform.position.y - tankSpawner.Biolist[i].transform.position.y);
            //if (man.TankNum == 7) print(TeamMateDis);
            if ((TeamMateDis < man.TeammateAvoidDis) && TeamMateDis != 10000.0f && position_y < 4)
            {
                if (man.relativespeed == 1.0f)
                {
                    TeamMateDir = (transform.position - tankSpawner.Biolist[i].transform.position).normalized/
                        Vector3.Distance(transform.position, tankSpawner.Biolist[i].transform.position);
                    //if(angle > 120) man.relativespeed = 1.0f;
                }
                else
                    TeamMateDir = (tankSpawner.Biolist[i].transform.position - transform.position).normalized /
                        Vector3.Distance(transform.position, tankSpawner.Biolist[i].transform.position);

            }
            else
            {
                
                TeamMateDir = Vector3.zero;
            }
                
            MoveDir1 += TeamMateDir;
        }
  
        MoveDir = (MoveDir1 + EnemyDir / man.target_dis + ObstacleVector(man, 22.5f, 40)).normalized;

        return MoveDir;
    }

    public Vector3 ObstacleVector(ManControl man, float angle, float distance)
    {
        int count = (int)(360 / angle);
        Vector3 direction = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            Vector3 manPos = new Vector3(man.transform.position.x, man.transform.position.y + 2, man.transform.position.z); ;
            Physics.Raycast(manPos, Quaternion.Euler(0, angle * i, 0) * man.transform.forward, out hit, distance);

            if (hit.collider != null && hit.collider.tag == "wall")
            {
                //print("avoid");
                direction += (man.transform.position - hit.point);
            }

            //direction += (man.transform.position - positions[minValues[i]]).normalized / (wallDis1[minValues[i]] + 0.00001f);// / (0.001f + wallDis1[i]/10);
            //direction += (positions[minValues[i]] - man.transform.position);
        }
        //Debug.DrawRay(man.transform.position, direction, Color.green);
        return direction;
    }
}
