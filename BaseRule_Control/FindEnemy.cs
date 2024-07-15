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
                    //BackDistance2 = 500;
                    // AmbushDis = 250;
                    BackDistance1 = 0;
                    BackDistance2 = 0;
                    AmbushDis = 50.0F;
                }
            }
            else if (redNum == 3 && blueNum == 4)
            {
                if (isOptimized)
                {
                    //BackDistance2 = 500;
                    // AmbushDis = 0;
                    BackDistance2 = 500;
                    AmbushDis = 100;
                }
                else
                {
                    BackDistance1 = 500;
                    BackDistance2 = 500;
                    AmbushDis = 400;
                }
            }
            else if (redNum == 3 && blueNum == 5)
            {
                if (isOptimized)
                {
                    BackDistance1 = 500;
                    BackDistance2 = 500;
                    AmbushDis = 100;
                }
                else
                {
                    BackDistance1 = 500;
                    BackDistance2 = 500;
                    AmbushDis = 400;
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
            else// if (redNum == 5 && blueNum == 5)
            {
                if (isOptimized)
                {
                    //BackDistance1 = 800;
                    //BackDistance2 = 600;
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
        else
        {
            AmbushDis = 100;
            if (isOptimized)
            {
                BackDistance1 = 300;
                BackDistance2 = 300;
            }
            else
            {
                BackDistance1 = 100;
                BackDistance2 = 100;
            }
        }
    }
    
    public void Scout(ManControl man, Transform transform)
    {
        //man.AmbushDis = AmbushDis;
        //if (man.trainingSetting.RedTeam.nums == 3 && man.trainingSetting.BlueTeam.nums == 4)
        //{
        //    if (!man.trainingSetting.algorithmSelect.BioOptimized)
        //    {
        //        if (judgeSelfPosToCenter(man) == 0 || judgeSelfPosToCenter(man) == -1) man.AmbushDis = 50;
        //        //else man.AmbushDis = AmbushDis;
        //    }
        //}
       float t = 0.01f;
        Vector3 target;
        man.OpenFireDis = 2000.0f;//设置开火范围

        //if (Mathf.Max(man.TeamMateRot) > 90)
        //    man.BackDistance = 400;

        if (man.enemylive > 3)
            man.BackDistance = BackDistance1;//5v5优化版本胜率挺高，其他版本为700
        else if (man.enemylive > 1 && man.enemylive <= 3)
            man.BackDistance = BackDistance2;
        else
            man.BackDistance = BackDistance2;

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
            target = man.Enemy_len != 0 ? attackedEnemy.transform.position : new Vector3(139.653137f, 5.54664707f, 606.299988f);
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
            target = man.Enemy_len != 0 ? attackedEnemy.transform.position : new Vector3(139.653137f, 5.54664707f, 606.299988f);
        }


        // 设置物体的位置
        Vector3 target2 = target;

        if (Mathf.Max(man.TeamMateRot) < 70 && man.enemyDisXOZ > man.BackDistance)
        {
            //print(man.TankNum + ":  " + man.BioSameEnemyDir.First().Value.TankNum + ":  " + man.BioSameEnemyDir.First().Key);
            man.HillIndex = judgeSelfPos(man, target);
            switch (judgeSelfPos(man, target))
            {
                case -1:
                    target2 = target;
                    man.relativespeed = 1.0f;
                    break;
                case 1:
                    target2 = baseFunction2.Set_point(man.transform, target - man.transform.position, 45, 30);
                    break;
                case 2:
                    target2 = baseFunction2.Set_point(man.transform, target - man.transform.position, -45, 30);
                    break;
            }
        }

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
            man.OpenFire(1, man.enemyAngle1, 1);
        }

    }

    public void TAScout(ManControl TATank, Transform transform)
    {
        TATank.Enemy_len = TATank.TAEnemydir.Count;
        //TATank.enemyJudge2.AN_Enemy_View(TATank);
        //TATank.infoCalculate2.judgeEnemyPos1(TATank);
        TATank.MinNum = TATank.Enemy_len != 0 ?
            (TATank.TAEnemydir.Values.Contains(tankSpawner.Biolist[TATank.enemyJudge2.enemyList[TATank.TankNum] - 1]) ? TATank.enemyJudge2.enemyList[TATank.TankNum] : TATank.TAEnemydir.First().Value.TankNum) : 1;
        TATank.relativespeed = 1;
        //TATank.NUM_Text.text = (TATank.TankNum + 1).ToString() + "-" + (TATank.MinNum).ToString();

        //计算对手的方位
        var attackedEnemy = tankSpawner.Biolist[TATank.MinNum - 1].GetComponent<ManControl>();
        float[] TempRot = baseFunction2.DotCalculate(TATank.cannon, attackedEnemy.transform);
        TATank.Enemyinf[0] = TempRot[0];
        TATank.Enemyinf[1] = baseFunction2.DotCalculate5(TATank.cannon, attackedEnemy.transform); //baseFunction.DotCalculate3(transform, tankSpawner.BlueAgentsList[man.MinNum - 1].transform); ;
        TempRot[1] = baseFunction2.CrossCalculate1(transform, attackedEnemy.transform);
        TATank.Enemyinf[2] = TempRot[1];//右边为正左边为负值

        TATank.target1 = TATank.Enemy_len != 0 ? tankSpawner.Biolist[TATank.MinNum - 1].transform.position : new Vector3(139.653137f, 5.54664707f, 606.299988f);
        if (TATank.baseFunction2.CalculateDisX0Z(TATank.target1, TATank.transform.position) > 30)
            TATank.offset = ((TATank.target1 - TATank.transform.position).normalized + obstacleAvoid2.ObstacleVector(TATank, 45, 30).normalized + NRTeamMateAvoid(TATank, trainingSetting.BlueTeam.nums, 50)).normalized;//obstacleAvoid2.CacPosition(TATank, transform, TATank.target1, 180);//通过避障函数求出运行方向
        else
            TATank.offset = ((TATank.transform.position - TATank.target1).normalized + obstacleAvoid2.ObstacleVector(TATank, 45, 30).normalized + NRTeamMateAvoid(TATank, trainingSetting.BlueTeam.nums, 50)).normalized;//obstacleAvoid2.CacPosition(TATank, transform, TATank.target1, 180);//通过避障函数求出运行方向

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
            TATank.OpenFire(1, TATank.enemyAngle1, 1);
        }
    }

    private Vector3 NRTeamMateAvoid(ManControl man, int teamnum, float avoidDis)
    {
        Vector3 TeamMateDir = Vector3.zero ,AllTeamMateDir = Vector3.zero;
        for (int i = 0; i < teamnum; i++)
        {
            float TeamMateDis = i == man.TankNum ? 10000.0f : man.baseFunction2.CalculateDisX0Z(man.transform.position, tankSpawner.TAList[i].transform.position);//man.TeamMateDis1[i];
            float position_y = Mathf.Abs(man.transform.position.y - tankSpawner.TAList[i].transform.position.y);
            //if (man.TankNum == 7) print(TeamMateDis);
            if ((TeamMateDis < avoidDis) && TeamMateDis != 10000.0f && position_y < 4)
            {
                //print("avoid");
                TeamMateDir = (man.transform.position - tankSpawner.TAList[i].transform.position).normalized;
            }
            else
            {

                TeamMateDir = Vector3.zero;
            }
            AllTeamMateDir += TeamMateDir;
        }
        //if (AllTeamMateDir != Vector3.zero) UnityEngine.Debug.Log(AllTeamMateDir);
        return AllTeamMateDir;
    }

    //consensus-based auction algorithm对照算法
    public void CBAAScout(ManControl CBAATank, Transform transform)
    {
        CBAATank.relativespeed = 1;
        CBAATank.superiority_factor = new float[tankSpawner.Biolist.Count];
        for (int i = 0; i < tankSpawner.Biolist.Count; i++)
        {
            ManControl biotank = tankSpawner.Biolist[i];
            if (!biotank.Isdead)
                CBAATank.superiority_factor[i] = calSuperiorty(biotank, CBAATank);
            else
            {
                CBAATank.superiority_factor[i] = 0;
            }
            //if (CBAATank.name == "BlueTeanm4")
            //{
            //    Debug.Log(biotank.name + " " + CBAATank.superiority_factor[i]);
            //}
        }

        //计算对手的方位
        var attackedEnemy = tankSpawner.Biolist[CBAATank.finalNum].GetComponent<ManControl>();
        float[] TempRot = baseFunction2.DotCalculate(CBAATank.cannon, attackedEnemy.transform);
        CBAATank.Enemyinf[0] = TempRot[0];
        CBAATank.Enemyinf[1] = baseFunction2.DotCalculate5(CBAATank.cannon, attackedEnemy.transform); //baseFunction.DotCalculate3(transform, tankSpawner.BlueAgentsList[man.MinNum - 1].transform); ;
        TempRot[1] = baseFunction2.CrossCalculate1(transform, attackedEnemy.transform);
        CBAATank.Enemyinf[2] = TempRot[1];//右边为正左边为负值

        CBAATank.target1 = tankSpawner.Biolist[CBAATank.finalNum].transform.position;
        if (CBAATank.baseFunction2.CalculateDisX0Z(CBAATank.target1, CBAATank.transform.position) > 30)
            CBAATank.offset = ((CBAATank.target1 - CBAATank.transform.position).normalized + obstacleAvoid2.ObstacleVector(CBAATank, 45, 30).normalized + NRTeamMateAvoid(CBAATank, trainingSetting.BlueTeam.nums, 50)).normalized;//obstacleAvoid2.CacPosition(TATank, transform, TATank.target1, 180);//通过避障函数求出运行方向
        else
            CBAATank.offset = ((CBAATank.transform.position - CBAATank.target1).normalized + obstacleAvoid2.ObstacleVector(CBAATank, 45, 30).normalized + NRTeamMateAvoid(CBAATank, trainingSetting.BlueTeam.nums, 50)).normalized;//obstacleAvoid2.CacPosition(TATank, transform, TATank.target1, 180);//通过避障函数求出运行方向
        CBAATank.offset[1] = 0;//令y方向为0


        float t = 0.0f;
        t = Mathf.Clamp01(t + (Time.deltaTime * 1.0f));
        //坦克动作执行代码，转向由Slerp函数执行，前进由man.move函数执行
        //Quaternion targetRotation1 = Quaternion.LookRotation(man.TowerDir);
        //man.Tower.transform.rotation = Quaternion.Lerp(man.Tower.rotation, targetRotation1, t1);

        Quaternion targetRotation = Quaternion.LookRotation(CBAATank.offset);
        CBAATank.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);

        if (CBAATank.speedControl)
        {
            CBAATank.move(CBAATank.relativespeed * CBAATank.MaxSpeed, 0);
        }

        Vector3 newTarget = new Vector3(CBAATank.target1.x, CBAATank.target1.y + 1.5f, CBAATank.target1.z);
        newTarget = newTarget - CBAATank.ShellPos.position;
        if (CBAATank.Enemyinf[1] < 5)
        {
            CBAATank.enemyAngle = Vector3.Angle(newTarget, CBAATank.ShellPos.forward);
            CBAATank.enemyAngle1 = newTarget.normalized.y > CBAATank.cannon.transform.forward.normalized.y ?
                       CBAATank.enemyAngle : -CBAATank.enemyAngle;
            CBAATank.OpenFire(1, CBAATank.enemyAngle1, 1);
        }
    }

    public float calSuperiorty(ManControl biotank, ManControl tank)
    {
        float s1;//夹角信息
        float s2;//位置信息
        s1 = 1 - (Vector3.Angle(tank.transform.forward, biotank.transform.position - tank.transform.position)) / 180;
        float dis = Vector3.Distance(tank.transform.position, biotank.transform.position);
        if (dis > tank.OpenFireDis) s2 = tank.OpenFireDis / dis;
        else s2 = 1;
        return (float)(s1 * 0.3 + s2 * 0.7);
    }


    public int judgeSelfPos(ManControl man, Vector3 target)
    {
        int result = -1;
        Vector3 Dir1 = target - man.infoCalculate2.CalculateCnter(tankSpawner.Biolist);//我方中心到的敌方目标个体的向量
        Dir1 = new Vector3(Dir1.x, 0, Dir1.z);
        Vector3 Dir2 = target - man.transform.position;
        Dir2 = new Vector3(Dir2.x, 0, Dir2.z);
        float angle = Vector3.SignedAngle(Dir1, Dir2, Vector3.up);
        if (angle > 15.0f) result = 1;
        else if (angle < -15.0f) result = 2;
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
        if (right > left && right - left >= 2 * trainingSetting.RedTeam.nums / 5) result = 1;
        else if (left > right && left - right >= 2 * trainingSetting.RedTeam.nums / 5) result = 2;
        else if (Mathf.Abs(right - left) < 2 * trainingSetting.RedTeam.nums / 5) result = -1;
        //if (((right >= 1 && right <= 4) && left == 0) || ((right == 2  || right == 3) && left == 1)) result = 1;//位于队伍左侧
        //else if(((left >= 1 && left <= 4) && right == 0) || ((left == 2 || left == 3) && right == 1)) result = 2;
        //else if ((right == 2 && left == 2) && (right == 1 && left == 1)) result = 0;

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
