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
    private RaycastHit hit;
    //private Vector3 MoveDir, MoveDir1, TeamMateDir, OtherEnemyDir = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        tankSpawner = FindObjectOfType<TankSpawner>();
        trainingSetting = FindObjectOfType<TranningSetting>();
        teamnum = trainingSetting.RedTeam.nums;
        enemynum = trainingSetting.BlueTeam.nums;
        if(teamnum == 3)
            roundTime = new float[] { 20, 27, 25, 22 };
        else if(teamnum > 3)
            roundTime = new float[] { 5, 15, 5, 5 };


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

        //target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 90, 30, 0);
        if ((GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[0] && man.gameManage.round % 4 == 0)
            ||(GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[1] && man.gameManage.round % 4 == 1)
            || (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[2] && man.gameManage.round % 4 == 2)
            || (GameObject.Find("NVN").GetComponent<GameManage>().Righttime >= roundTime[3] && man.gameManage.round % 4 == 3))
        {
            switch (man.findEnemy2.judgeSelfPosToCenter(man))
            {
                case 1:
                    target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, -45, 30, 0);
                    break;
                case 2:
                    target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 45, 30, 0);
                    break;
                default:
                    target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 0, 30, 0);
                    man.relativespeed = 0.1f;
                    break;
            }
        }
        else
            man.relativespeed = 1.0f;

        if ((target == (!tankSpawner.useTA ? tankSpawner.BlueAgentsList[man.MinNum - 1].transform.position : tankSpawner.TAList[man.MinNum - 1].transform.position)) && isEnmey)
        {
            if (((man.target_dis < man.BackDistance && man.target_dis > man.BackDistance - 50) || man.rotateFlag == 1 || (man.firetime < man.cooldowntime && man.enemyDisXOZ < 800 && man.target_dis > man.BackDistance)))
            {
                //target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 90, 30, 0);
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
                        man.relativespeed = 0.2f;
                        break;
                }
            }
            else if ((man.firetime < man.cooldowntime && man.target_dis < man.BackDistance))
            {
                man.relativespeed = -1.0f;
                switch (man.findEnemy2.judgeSelfPosToCenter(man))
                {
                    case 1:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 45, 30, 0);
                        break;
                    case 2:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, -45, 30, 0);
                        break;
                    default:
                        target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 0, 30, 0);
                        break;
                }

            }
        }


        //if(Mathf.Max(man.TeamMateSingleRot) < 70)
        //{
        //    if (man.TankNum == 1 || man.TankNum == 2) target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, -90, 30);
        //    else if(man.TankNum == 3) man.relativespeed = 0.1f;
        //    else if (man.TankNum == 4 || man.TankNum == 5) target = man.baseFunction2.Set_point(man.transform, target - man.transform.position, 90, 30);

        //}
        //if((man.target_dis < man.BackDistance - 50 && man.firetime < 300) ||
        //        (man.speedControl == true && man.enemyDisXOZ < 200 && man.firetime > 300))
        //    EnemyDir = (transform.position - target).normalized;
        //else EnemyDir = (target - transform.position).normalized;

        if (target == (tankSpawner.useTA ? tankSpawner.TAList[man.MinNum - 1].transform.position :
     tankSpawner.BlueAgentsList[man.MinNum - 1].transform.position))
            isEnmey = true;
        else
            isEnmey = false;

        if (((man.target_dis < man.BackDistance - 100 && man.firetime < 300)||
        (man.speedControl == true && man.enemyDisXOZ < 200 && man.firetime > 300)) && man.rotateFlag != 1 && isEnmey)
            man.relativespeed = -1.0f;
        EnemyDir = (target - transform.position).normalized;
        // EnemyDir = EnemyDir / 5;

        //EnemyDir = new Vector3(EnemyDir.x, EnemyDir.y + 2, EnemyDir.z) ;
        //求队友对自身的合力
        for (int i = 0; i < teamnum; i++)
        {
            float TeamMateDis = i == man.TankNum - 1 ?  10000.0f : man.baseFunction2.CalculateDisX0Z(man.transform.position, tankSpawner.Biolist[i].transform.position);//man.TeamMateDis1[i];
            float angle = man.baseFunction2.DotCalculate5(man.transform, tankSpawner.Biolist[i].transform);
            float position_y = Mathf.Abs(transform.position.y - tankSpawner.Biolist[i].transform.position.y);
            //if (man.TankNum == 7) print(TeamMateDis);
            if ((TeamMateDis < man.TeammateAvoidDis) && TeamMateDis != 10000.0f && position_y < 4)
            {
                if ((man.MinNum == -1) && tankSpawner.Biolist[i].transform.position != target)
                {
                    //if(man.TankNum == 16)print("avoid");
                    //if (man.TankNum == 7) print(TeamMateDis);
                    TeamMateDir = (transform.position - tankSpawner.Biolist[i].transform.position).normalized/
                        Vector3.Distance(transform.position, tankSpawner.Biolist[i].transform.position);
                }
                else
                    TeamMateDir = Vector3.zero;
                
            }
            else
            {
                
                TeamMateDir = Vector3.zero;
            }
                
            MoveDir1 += TeamMateDir;
        }

        //if(MoveDir1 != Vector3.zero)
        //{
        //    man.relativespeed = man.relativespeed > 0 ? 0.8f : man.relativespeed;
        //}
        //else
        //{
        //    man.relativespeed = man.relativespeed > 0 ? 1.0f : man.relativespeed;
        //}

        //if(man.firetime > man.cooldowntime)
        //    MoveDir = (MoveDir1 + EnemyDir / man.target_dis).normalized;
        //else
            MoveDir = (MoveDir1 + EnemyDir / man.target_dis + ObstacleVector(man, 22.5f, 40)).normalized;

        //MoveDir = new Vector3(MoveDir.x, MoveDir.y + 2, MoveDir.z);
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
