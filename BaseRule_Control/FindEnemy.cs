using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;
using System.Linq;

public class FindEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public TankSpawner tankSpawner;
    public BaseFunction baseFunction2;
    public ObstacleAvoid obstacleAvoid2;
    public PathOptimize pathOptimize2;
    public TranningSetting trainingSetting;
    public float BackDistance1 = 50;//后撤距离
    public float BackDistance2 = 50;//后撤距离
    public float AmbushDis = 600;//埋伏距离
    private RaycastHit hit;

    void Start()
    {
        tankSpawner = FindObjectOfType<TankSpawner>();
        baseFunction2 = FindObjectOfType<BaseFunction>();
        obstacleAvoid2 = FindObjectOfType<ObstacleAvoid>();
        pathOptimize2 = FindObjectOfType<PathOptimize>();
        trainingSetting = FindObjectOfType<TranningSetting>();
        setDis(trainingSetting.RedTeam.nums, trainingSetting.BlueTeam.nums, tankSpawner.useTA, trainingSetting.algorithmSelect.BioOptimized);


    }

    //后撤距离设置函数，针对不同情况设置不同的后撤距离
    float KeepDis(ManControl man)
    {
        float dis = 24.0f;
        man.EnemyAttackNum = 0;
        if (man.EnemyAttackNum >= 2)
            dis = 24;
        if (man.Enemyinf[2] > 50 && man.EnemyDis[man.MinNum - 1] < 40)
            dis = 20;
        return dis;
    }

    public float radius = 2000.0f; // 圆的半径
                                   //寻找并攻击敌军
                                   
    public void setDis(int redNum, int blueNum, bool isNR, bool isOptimized)
    {
    	if(isNR)
    	{
            BackDistance1 = 700;
            if (redNum == 3 && blueNum == 3)
            {
                if (isOptimized)
                {
                   BackDistance2 = 600;
                    AmbushDis = 100;
                }
                else
                {
                   BackDistance2 = 500;
                    AmbushDis = 300;
                }
            }
            else if (redNum == 3 && blueNum == 4)
            {
                if (isOptimized)
                {
                   BackDistance2 = 800;
                    AmbushDis = 0;
                }
                else
                {
                   BackDistance2 = 600;
                    AmbushDis = 500;
                }
            }
            else if (redNum == 3 && blueNum == 5)
            {
                if (isOptimized)
                {
                   BackDistance2 = 500;
                    AmbushDis = 100;
                }
                else
                {
                   BackDistance2 = 500;
                    AmbushDis = 300;
                }
            }
            else if (redNum == 4 && blueNum == 3)
            {
                if (isOptimized)
                {
                   BackDistance2 = 700;
                    AmbushDis = 0;
                }
                else
                {
                   BackDistance2 = 400;
                    AmbushDis = 0;
                }
            }
            else if (redNum == 5 && blueNum == 3)
            {
                if (isOptimized)
                {
                   BackDistance2 = 500;
                    AmbushDis = 0;
                }
                else
                {
                   BackDistance2 = 400;
                    AmbushDis = 350;
                }
            }
            else if (redNum == 5 && blueNum == 5)
            {
                if (isOptimized)
                {
                    BackDistance1 = 800;
                    BackDistance2 = 600;
                    AmbushDis = 0;
                }
                else
                {
                   BackDistance2 = 700;
                    AmbushDis = 450;
                }
            }
        }
    }
    
    public void Scout(ManControl man, Transform transform)
    {
        float t = 0.01f;
        Vector3 target;
        man.OpenFireDis = 2000.0f;//设置开火范围

        //if (Mathf.Max(man.TeamMateRot) > 90)
        //    man.BackDistance = 400;

        if (man.enemylive > 3)
            man.BackDistance = BackDistance1;//5v5优化版本胜率挺高，其他版本为700
        else if (man.enemylive > 1 && man.enemylive <= 3)
            man.BackDistance =BackDistance2;
        else
            man.BackDistance =BackDistance2;

        if (!tankSpawner.useTA)
        {
            //计算对手的方位
            var attackedEnemy = tankSpawner.BlueAgentsList[man.MinNum - 1].GetComponent<TankControl>();
            float[] TempRot = baseFunction2.DotCalculate(man.cannon, attackedEnemy.transform);
            man.Enemyinf[0] = TempRot[0];
            man.Enemyinf[1] = baseFunction2.DotCalculate5(man.cannon, attackedEnemy.transform); //baseFunction.DotCalculate3(transform, tankSpawner.BlueAgentsList[man.MinNum - 1].transform); ;
            TempRot[1] = baseFunction2.CrossCalculate1(transform, attackedEnemy.transform);
            man.Enemyinf[2] = TempRot[1];//右边为正左边为负值


            
            man.enemyDisXOZ = baseFunction2.CalculateDisX0Z(man.transform.position, attackedEnemy.transform.position);
            if (!attackedEnemy.Isdead)
            {
                if (man.enemyDisXOZ <= man.BackDistance && man.Enemyinf[1] < 30)
                {
                    man.relativespeed = -1.0f;
                    man.avoidAngle = 70.0f;
                }
            }
            else
            {
                man.relativespeed = 1.0f;
                man.avoidAngle = 120.0f;
            }
            target = attackedEnemy.transform.position;
        }
        else
        {
            //计算对手的方位
            var attackedEnemy = tankSpawner.TAList[man.MinNum - 1].GetComponent<ManControl>();
            float[] TempRot = baseFunction2.DotCalculate(man.cannon, attackedEnemy.transform);
            man.Enemyinf[0] = TempRot[0];
            man.Enemyinf[1] = baseFunction2.DotCalculate5(man.cannon, attackedEnemy.transform); //baseFunction.DotCalculate3(transform, tankSpawner.BlueAgentsList[man.MinNum - 1].transform); ;
            TempRot[1] = baseFunction2.CrossCalculate1(transform, attackedEnemy.transform);
            man.Enemyinf[2] = TempRot[1];//右边为正左边为负值


            //速度确定代码，当对手距离大于后撤距离则全速前进，小于后撤距离则以0.8倍最大速度后撤
            man.enemyDisXOZ = baseFunction2.CalculateDisX0Z(man.transform.position, attackedEnemy.transform.position);
            if (!attackedEnemy.Isdead)
            {
                if (man.enemyDisXOZ <= man.BackDistance && man.Enemyinf[1] < 30)
                {
                    man.relativespeed = 1.0f;
                    man.avoidAngle = 70.0f;
                }
            }
            else
            {
                man.relativespeed = 1.0f;
                man.avoidAngle = 120.0f;
            }
            target = attackedEnemy.transform.position;
        }


        // 设置物体的位置
        Vector3 target2 = target;

        //if (Mathf.Max(man.TeamMateRot) < 70 && man.enemyDisXOZ > man.BackDistance)
        //{
        //    //print(man.TankNum + ":  " + man.BioSameEnemyDir.First().Value.TankNum + ":  " + man.BioSameEnemyDir.First().Key);
        //    man.HillIndex = judgeSelfPos(man);
        //    switch (judgeSelfPos(man))
        //    {
        //        case -1:
        //            target2 = target;
        //            man.relativespeed = 1.0f;
        //            break;
        //        case 1:
        //            target2 = baseFunction2.Set_point(man.transform, target - man.transform.position, 45, 30);
        //            break;
        //        case 2:
        //            target2 = baseFunction2.Set_point(man.transform, target - man.transform.position, -45, 30);
        //            break;
        //    }
        //}

        if (man.cannon_script2.ColliderFlag1)
        {
            target2 = baseFunction2.Set_point(man.transform, target - man.transform.position, 90, 30);
        }

        int Rmask = LayerMask.GetMask("GroundLayer");
        man.target1 = target2;//通过target1能够在inspector直观显示锁定对手的位置信息

        man.offset = obstacleAvoid2.CacPosition(man, transform, target2, man.avoidAngle);//通过避障函数求出运行方向
        man.offset[1] = 0;//令y方向为0

        man.TowerDir = target - man.Tower.position;
        man.TowerDir[1] = 0;

        man.CannonDir = target - man.ShellPos.position;
        man.CannonDir[0] = 0;
        man.CannonDir[2] = 0;
        man.ControlFlag = true;

        //t值可以保证坦克在旋转过程中比较平稳
        t = 0.0f;
        t = Mathf.Clamp01(t + (Time.deltaTime * 1.0f));

        if (((man.cannon_script2.ColliderFlag || (judgeCannonCast(man.BioEnemydir, man.BioEnemydirTA, man) && man.enemyDisXOZ < AmbushDis)) && man.firetime >= man.cooldowntime))
        {
            man.speedControl = false;
            man.ControlFlag = false;
        }
        else
        {
            man.ControlFlag = true;
            man.speedControl = true;
        }

        //if (man.speedControl == false || man.rotateFlag == 1)
        //{
        //    man.stopTime++;
        //}

        //if (man.stopTime > 400 && man.stopTime < 1000)
        //{
        //    if (man.tower_script.angle < 5.0f)
        //    {
        //        man.rotateFlag = 1;//开始绕
        //        man.speedControl = true;
        //    }
        //}
        //else if (man.stopTime >= 1000)
        //{
        //    man.stopTime = 0;
        //    man.rotateFlag = 0;
        //}

        //if (man.cannon_script2.ColliderFlag && man.firetime > man.cooldowntime)
        //{
        //    man.speedControl = false;
        //    man.ControlFlag = false;
        //}
        //else
        //{
        //    man.ControlFlag = true;
        //    man.speedControl = true;
        //}

        if (man.RegularFlag == 0)
        {
            float t1 = 0.0f;
            t1 = Mathf.Clamp01(t + (Time.deltaTime * 1.0f));
            //坦克动作执行代码，转向由Slerp函数执行，前进由man.move函数执行
            //Quaternion targetRotation1 = Quaternion.LookRotation(man.TowerDir);
            //man.Tower.transform.rotation = Quaternion.Lerp(man.Tower.rotation, targetRotation1, t1);

            Quaternion targetRotation = Quaternion.LookRotation(man.offset);
            man.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);

            if (man.speedControl)
            {
                man.move(man.relativespeed * man.MaxSpeed, 0);
            }
        }

        if (man.ControlFlag)
        {
            //man.enemyAngle = baseFunction2.DotCalculate3_1(target, man.Tower.position);            
        }
        Vector3 newTarget = Vector3.zero;
        if(tankSpawner.useTA && man.tankSpawner.TAList[man.MinNum - 1].transform.position.y - man.transform.position.y > 20.0f)
            newTarget =  new Vector3(target.x, target.y + 4.5f, target.z);
        else newTarget = new Vector3(target.x, target.y + 2.5f, target.z);
        newTarget = newTarget - man.ShellPos.position;
        if (man.Enemyinf[1] < 25)
        {
            man.enemyAngle = Vector3.Angle(newTarget, man.ShellPos.forward);
            man.enemyAngle1 = newTarget.normalized.y > man.cannon.transform.forward.normalized.y ?
                       man.enemyAngle : -man.enemyAngle;
            man.OpenFire2(1, man.enemyAngle1, 1);
        }

    }

    public void TAScout(ManControl TATank, Transform transform)
    {
        TATank.Enemy_len = TATank.TAEnemydir.Count;
        TATank.MinNum = TATank.Enemy_len != 0 ? TATank.enemyJudge2.enemyList[TATank.TankNum] : 1;
        TATank.relativespeed = 1;

        //计算对手的方位
        var attackedEnemy = tankSpawner.Biolist[TATank.MinNum - 1].GetComponent<ManControl>();
        float[] TempRot = baseFunction2.DotCalculate(TATank.cannon, attackedEnemy.transform);
        TATank.Enemyinf[0] = TempRot[0];
        TATank.Enemyinf[1] = baseFunction2.DotCalculate5(TATank.cannon, attackedEnemy.transform); //baseFunction.DotCalculate3(transform, tankSpawner.BlueAgentsList[man.MinNum - 1].transform); ;
        TempRot[1] = baseFunction2.CrossCalculate1(transform, attackedEnemy.transform);
        TATank.Enemyinf[2] = TempRot[1];//右边为正左边为负值

        TATank.target1 = tankSpawner.Biolist[TATank.MinNum - 1].transform.position;
        if (TATank.baseFunction2.CalculateDisX0Z(TATank.target1, TATank.transform.position) > 30)
            TATank.offset = ((TATank.target1 - TATank.transform.position).normalized + obstacleAvoid2.ObstacleVector(TATank, 45, 30).normalized).normalized;//obstacleAvoid2.CacPosition(TATank, transform, TATank.target1, 180);//通过避障函数求出运行方向
        else
            TATank.offset = ((TATank.transform.position - TATank.target1).normalized + obstacleAvoid2.ObstacleVector(TATank, 45, 30).normalized).normalized;//obstacleAvoid2.CacPosition(TATank, transform, TATank.target1, 180);//通过避障函数求出运行方向

        TATank.offset[1] = 0;//令y方向为0

        float t = 0.0f;
        t = Mathf.Clamp01(t + (Time.deltaTime * 1.0f));
        //坦克动作执行代码，转向由Slerp函数执行，前进由man.move函数执行
        //Quaternion targetRotation1 = Quaternion.LookRotation(man.TowerDir);
        //man.Tower.transform.rotation = Quaternion.Lerp(man.Tower.rotation, targetRotation1, t1);

        Quaternion targetRotation = Quaternion.LookRotation(TATank.offset);
        TATank.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);

        if (TATank.speedControl)
        {
            TATank.move(TATank.relativespeed * TATank.MaxSpeed, 0);
        }

        Vector3 newTarget = new Vector3(TATank.target1.x, TATank.target1.y + 1.5f, TATank.target1.z);
        newTarget = newTarget - TATank.ShellPos.position;
        if (TATank.Enemyinf[1] < 5)
        {
            TATank.enemyAngle = Vector3.Angle(newTarget, TATank.ShellPos.forward);
            TATank.enemyAngle1 = newTarget.normalized.y > TATank.cannon.transform.forward.normalized.y ?
                       TATank.enemyAngle : -TATank.enemyAngle;
            TATank.OpenFire3(1, TATank.enemyAngle1, 1);
        }
    }
    //public int judgeSelfPos(ManControl man)
    //{
    //    int result = -1;   
    //    if(man.BioSameEnemyDir.Count !=0)
    //    {
    //        if (man.EnemyDis[man.MinNum - 1] > man.BioSameEnemyDir.First().Key)
    //        {
    //            if (man.TeamMateSingleRot[man.BioSameEnemyDir.First().Value.TankNum - 1] > 0)
    //                result = 1;
    //            else
    //                result = 2;
    //        }
    //    }

    //    return result;

    //}
    public int judgeSelfPos(ManControl man)
    {
        int result = -1;
        if (man.BioSameEnemyDir.Count != 0)
        {
            //int SameEnemyDirLen = man.BioSameEnemyDir.Count;
            if (man.EnemyDis[man.MinNum - 1] < man.BioSameEnemyDir.Last().Key)
            {
                if (man.TeamMateSingleRot[man.BioSameEnemyDir.Last().Value.TankNum - 1] > 0)
                    result = 1;
                else if (man.TeamMateSingleRot[man.BioSameEnemyDir.Last().Value.TankNum - 1] < 0)
                    result = 2;//左侧小于零
            }
        }

        return result;
    }

    public int judgeSelfPosToCenter(ManControl man)
    {
        int right = 0, left = 0, result = -1;
        for(int i = 0; i < man.TeamMateSingleRot.Length; i ++)
        {
            if (man.TeamMateSingleRot[i] < 0) right++;//右侧队友数量加一
            else if (man.TeamMateSingleRot[i] > 0) left++;//左侧队友数量加一
        }
        if (((right >= 1 && right <= 4) && left == 0) || ((right == 2  || right == 3) && left == 1)) result = 1;//位于队伍左侧
        else if(((left >= 1 && left <= 4) && right == 0) || ((left == 2 || left == 3) && right == 1)) result = 2;
        else if ((right == 2 && left == 2) && (right == 1 && left == 1)) result = 0;

        return result;
    }

    private bool judgeCannonCast(SortedDictionary<float, TankControl> BioEnemydir, SortedDictionary<float, ManControl> BioEnemydirTA, ManControl man)
    {
        if (!man.tankSpawner.useTA)
        {
            foreach (var enemy in BioEnemydir)
            {
                RaycastHit Enemyhit;
                bool JudgeEnemy = false;

                Vector3 startPoint = enemy.Value.ShellPos.position;
                Vector3 endPoint = new Vector3(man.transform.position.x, man.transform.position.y + 1.5f, man.transform.position.z);
                Vector3 direction = endPoint - startPoint;
                float distance = direction.magnitude;
                if (Physics.Raycast(startPoint, direction.normalized, out Enemyhit, distance)) // 射线检测是否有障碍物
                {
                    if (Enemyhit.collider != null && Enemyhit.collider.tag == "Tank_Red") // 判断射线撞击的是否是红色坦克
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            foreach (var enemy in BioEnemydirTA)
            {
                RaycastHit Enemyhit;
                bool JudgeEnemy = false;

                Vector3 startPoint = enemy.Value.ShellPos.position;
                Vector3 endPoint = new Vector3(man.transform.position.x, man.transform.position.y + 1.5f, man.transform.position.z);
                Vector3 direction = endPoint - startPoint;
                float distance = direction.magnitude;
                if (Physics.Raycast(startPoint, direction.normalized, out Enemyhit, distance)) // 射线检测是否有障碍物
                {
                    if (Enemyhit.collider != null && Enemyhit.collider.tag == "Tank_Red") // 判断射线撞击的是否是红色坦克
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    int restartFindEnemy(ManControl man, SortedDictionary<float, ManControl> teamMateMinNum, int index = 0)
    {
        if (teamMateMinNum.Values.ElementAt(index).MinNum != -1) return teamMateMinNum.Values.ElementAt(index).MinNum;
        else return restartFindEnemy(man, teamMateMinNum, index + 1);
    }


    Vector3 move_target1(ManControl man, int leaderNum, float angle, float distance)
    {
        Vector3 target = Vector3.zero;
        if (man.EnemyDis[man.MinNum - 1] > man.BackDistance + 10)
            target = baseFunction2.Set_point(tankSpawner.Biolist[leaderNum - 1].transform,
                tankSpawner.Biolist[leaderNum - 1].transform.forward, angle, distance);
        else
            target = tankSpawner.BlueAgentsList[man.MinNum - 1].transform.position;
        return target;
    }

    Vector3 assign_target(ManControl man)
    {
        Vector3 target = Vector3.zero;
        switch (man.TankNum)
        {
            case 1:
                target = move_target1(man, 3, -(man.TankNum - 3) / (man.TankNum - 3) * 90, 100);
                break;
            case 2:
                target = move_target1(man, 3, -(man.TankNum - 3) / (man.TankNum - 3) * 90, 50);
                break;
            case 3:
                target = tankSpawner.BlueAgentsList[man.MinNum - 1].transform.position;
                break;
            case 4:
                target = move_target1(man, 3, (man.TankNum - 3) / (man.TankNum - 3) * 90, 50);
                break;
            case 5:
                target = move_target1(man, 3, 
                    (man.TankNum - 3) / (man.TankNum - 3) * 90, 100);
                break;
                //default:


        }
        return target;
    }


    public void attack_point(ManControl man, Vector3 target_point)
    {
        float dis = Vector3.Distance(man.transform.position, target_point);
        man.Enemyinf[0] = dis;
        float angle = baseFunction2.DotCalculate4(man.transform, target_point);
        man.Enemyinf[1] = angle;
        if (dis < man.OpenFireDis && angle < 5.0f)
        {
            //RealShoot(man, tankSpawner.BlueAgentsList[man.MinNum - 1].transform.position);
            //计算坦克炮筒与对手在XOZ平面的夹角
            man.enemyAngle = baseFunction2.DotCalculate3_1(target_point, man.transform.position);
            Vector3 enemyPos = target_point;
            //float[] optimize_result = new float[2];
            //man.optimize_result = optimitize_openfire(man, enemyPos, man.shell_start_pos, man.shell_collider_pos, man.enemyAngle1);
            Vector3 vector3 = new Vector3(enemyPos.x, enemyPos.y + 2 + man.optimize_result[1], enemyPos.z) - man.cannon.transform.position;
            //man.enemyAngle = Vector3.Angle(new Vector3(0f, man.cannon.transform.forward.y, 0f), new Vector3(0f, vector3.y, 0f));
            man.enemyAngle = Vector3.Angle(man.cannon.transform.forward, vector3);// + man.setEnemyAngle1;
                                                                                  //如果setEnemyAngle1 = -1，表示此时角度由计算得出，否则炮筒角度由setEnemyAngle1直接赋值
                                                                                  //if (man.setEnemyAngle1 == -1) man.enemyAngle1 =
                                                                                  //        vector3.normalized.y > man.cannon.transform.forward.normalized.y ?
                                                                                  //        man.enemyAngle : -man.enemyAngle;
                                                                                  //else man.enemyAngle1 = man.setEnemyAngle1 < 0 ? 0 : man.setEnemyAngle1;
                                                                                  //man.optimize_result = optimitize_openfire(man, enemyPos, man.shell_start_pos, man.shell_collider_pos, man.enemyAngle1);
            man.enemyAngle1 = vector3.normalized.y > man.cannon.transform.forward.normalized.y ?
                   man.enemyAngle : -man.enemyAngle;
            //man.enemyAngle1 = man.enemyAngle;

            //炮弹速度补偿，根据自身高度调整射速
            if (man.speedControl == true
                || baseFunction2.CalculateDisX0Z(man.transform.position, target_point) <= man.BackDistance)
            {
                float fireSpeedOffset = man.transform.position.y > target_point.y ?
                man.transform.position.y - target_point.y : 0;
                fireSpeedOffset = fireSpeedOffset / 30.0f * 2.5f;
                man.testFireSpeeed = 1 - fireSpeedOffset;
            }
            else
            {
                if (baseFunction2.CalculateDisX0Z(man.transform.position, target_point) > man.BackDistance)
                {
                    man.testFireSpeeed = 1.0f;
                }
            }

            man.testFireSpeeed = man.optimize_result[0];
            if (man.SetFireSpeeed != -1)
            {
                man.testFireSpeeed = man.SetFireSpeeed;
            }

            if (dis > 10.0f)
            {

                man.OpenFire2(1, man.enemyAngle1, 1);
            }
            else if (dis <= 10.0f)
                man.OpenFire2(-0.5f, man.enemyAngle1, 1);
        }
    }

    /*
     分情况讨论：
    1、低处打高处，如果短了就角度补偿，长了就速度补偿
    2、高处打低处，即坦克角度为0，速度补偿
     */
    //public float[] result = new float[2];

    float[] optimitize_openfire(ManControl man, Vector3 enemy_pos, Vector3 shell_start, Vector3 shell_end, float enemy_angle)
    {

        if (enemy_angle < -1.0f)//高打低、低打高但是炮筒斜着朝上
        {
            //man.cooldowntime = 100;
            if (man.SameFireCount < 3)//同一对手坦克发炮次数小于5
            {
                man.optimize_result[0] = 1 + enemy_angle / 18.0f;
            }
            else
            {
                if (man.left_edge == 1)
                {
                    man.left_edge = -1;
                    man.right_edge = 1;
                    man.optimize_result[0] = 1 + enemy_angle / 16.0f;
                }
                if (Mathf.Abs(man.left_edge - man.right_edge) < 0.01)
                {
                    man.left_edge = -1;
                    man.right_edge = 1;
                    man.optimize_result[0] = 1 + enemy_angle / 16.0f;
                }
                //if(man.firetime < 50)
                //{
                if (man.shell_collider_pos != Vector3.zero)
                {

                    man.shell_dis = baseFunction2.CalculateDisX0Z(shell_start, shell_end);
                }
                else
                {

                    man.shell_dis = 1000;
                }
                man.bio_enemy_dis = baseFunction2.CalculateDisX0Z(man.transform.position, enemy_pos);


                //if (man.CalFireCount == 0) print("111");
                if (man.bio_enemy_dis >= man.shell_dis && man.CalFireCount == 0)
                {
                    //print("112");
                    man.CalFireCount++;
                    man.left_edge = man.optimize_result[0];
                    man.optimize_result[0] = (man.left_edge + man.right_edge) / 2;
                    //right_edge = 
                }
                else if (man.firetime > 70 && man.CalFireCount == 0)
                {
                    man.CalFireCount++;
                    //left_edge = man.fire_speed_buffer;
                    man.right_edge = man.optimize_result[0];
                    man.optimize_result[0] = (man.left_edge + man.right_edge) / 2;
                }
                //man.fire_speed_buffer = man.optimize_result[0];

                //}
            }

        }
        else
        {
            //man.cooldowntime = 80;
            man.optimize_result[0] = 1;
        }

        if (man.firetime < 50)
        {
            man.bio_enemy_dis = baseFunction2.CalculateDisX0Z(man.transform.position, enemy_pos);
            man.shell_dis = baseFunction2.CalculateDisX0Z(shell_start, shell_end);

            if (man.bio_enemy_dis > man.shell_dis && man.CalFireCount1 == 0)//表示对手在山顶，但自己只能打到山坡
            {
                man.CalFireCount1++;
                if (man.SameFireCount <= 2)
                {
                    man.fire_speed_buffer = man.optimize_result[1];
                    //man.optimize_result[1] = 1.0f + enemy_pos.y - shell_end.y;
                    if (man.bio_enemy_dis - man.shell_dis > 20)
                    {
                        man.optimize_result[1] = 1.5f + 3.0f * Mathf.Sqrt(man.SameFireCount) + enemy_pos.y - shell_end.y;
                    }
                    else
                        man.optimize_result[1] = 1.5f + 1.0f * Mathf.Sqrt(man.SameFireCount) + enemy_pos.y - shell_end.y;
                }
                else
                {
                    //man.optimize_result[1] = enemy_pos.y - shell_end.y + 1.0f * (man.SameFireCount - 2);
                    if (man.bio_enemy_dis - man.shell_dis > 20)
                    {
                        man.optimize_result[1] = 1.5f + 3.0f * Mathf.Sqrt(man.SameFireCount) + enemy_pos.y - shell_end.y;
                    }
                    else
                        man.optimize_result[1] = enemy_pos.y - shell_end.y + 1.5f + 1.2f * Mathf.Sqrt(man.SameFireCount);
                }
                //man.optimize_result[1] = man.optimize_result[1] + enemy_pos.y - shell_end.y;
            }

        }
        else if (man.CalFireCount1 == 0)
        {
            man.CalFireCount1++;
            //if (man.TankNum == 3)print("--");
            if (man.optimize_result[1] > 5) man.optimize_result[1] -= 1.5f;
            else man.optimize_result[1] -= 0.5f;
            if (man.optimize_result[1] <= 0) man.optimize_result[1] = 0;
        }

        if (man.optimize_result[1] < 0)
        {
            //if (man.TankNum == 3) print("-1");
            man.optimize_result[1] = 2;
        }
        return man.optimize_result;
    }

    void behavior_control(ManControl man, Transform transform)
    {
        Vector3 target = new Vector3(0, 0, 0);//这个坐标就是x基准点，需要自己定义
        if (man.TankNum == 0)
        {
            //man.offset = obstacleAvoid.CacPosition(man, transform, target);//求合力，即运行方向
            man.offset = target - transform.position;
            man.offset[1] = 0;//y轴置零
            //t值可以保证坦克在旋转过程中比较平稳
            float t = 0.0f;
            t = Mathf.Clamp01(t + (Time.deltaTime * 3.0f));

            Quaternion targetRotation = Quaternion.LookRotation(man.offset);
            man.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
            man.move(man.MaxSpeed, 0);//这里速度直接给了最大值，具体使用时需要自己求

        }
    }

}
