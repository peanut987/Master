//////
//该函数主要是控制炮弹爆炸伤害，以及tank伤害传递。
//////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellControl : MonoBehaviour
{
    //定义基础参数
    public ParticleSystem shellExplosion;
    public float explosionRadius = 2;
    public LayerMask tankMask;

    public float MaxDamage = 15;
    public int offet = 5;
    public GameObject father;
    private float MaxPH;
    private Vector3 collisionPosition; // 记录炮弹碰撞时的坐标
    public BaseFunction baseFunction;

    //写入配置文件
    private TranningSetting TranningSetting;
    public int shell_num;
    private float BioDemage;
    private float RLDemage;

    private void Start()
    {
        ManControl BioFireTank;
        //初始化参数
        TranningSetting = FindObjectOfType<TranningSetting>();
        baseFunction = FindObjectOfType<BaseFunction>();
        //如果找到配置文件
        if (TranningSetting)
        {
            MaxDamage = TranningSetting.EnvInfo.MaxDamage;
            explosionRadius = TranningSetting.EnvInfo.explosionRadius;
            offet = TranningSetting.EnvInfo.offet;
            MaxPH = TranningSetting.BlueTeam.FULLPH;
        }
        else
        {
            Debug.LogError("配置失败");
        }

        if (GameObject.Find("TankSpawner").GetComponent<TankSpawner>().useBio)
        {
            if (father.CompareTag("Tank_Red"))
            {
                BioFireTank = father.GetComponent<ManControl>();
                shell_num = BioFireTank.fire;
            }
            else
            {
                shell_num = -1;
            }
        }

        if (TranningSetting.RedTeam.nums == 3 && TranningSetting.BlueTeam.nums == 5)
        {
            BioDemage = MaxPH / 2;
            RLDemage = MaxPH;
        }
        else if (TranningSetting.RedTeam.nums == 5 && TranningSetting.BlueTeam.nums == 3)
        {
            BioDemage = MaxPH;
            RLDemage = MaxPH / 2;
        }
        else
        {
            BioDemage = MaxPH;
            RLDemage = MaxPH;
        }
    }

    //设置某个tank的发射方
    public void Set_father(GameObject fire)
    {
        father = fire;
    }

    //获取爆炸点坐标
    public Vector3 GetCollisionPosition()
    {
        return collisionPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //设置列表储存碰撞体（只储存tankmask层，即只检测tank）
        Collider[] tankColliders = Physics.OverlapSphere(this.transform.position, explosionRadius, tankMask);
        TankControl FireTank;
        TankControl BlueTank;
        ManControl BioFireTank;
        TankControl RlFireTank;

        collisionPosition = transform.position;
        if (GameObject.Find("TankSpawner").GetComponent<TankSpawner>().useBio)
        {
            if (father.CompareTag("Tank_Red"))
            {
                BioFireTank = father.GetComponent<ManControl>();
                if (shell_num == BioFireTank.fire) BioFireTank.shell_collider_pos = collisionPosition;
            }

            //if (father.CompareTag("Tank_Blue"))
            //{
            //    RlFireTank = father.GetComponent<TankControl>();
            //    RlFireTank.shell_pos = collisionPosition;
            //    FireFeedback(RlFireTank);
            //}
        }

        for (int i = 0; i < tankColliders.Length; i++)
        {
            var tankRigidbody = tankColliders[i].gameObject.GetComponent<Rigidbody>();

            if (tankRigidbody != null)
            {
                if (tankColliders[i].gameObject.TryGetComponent<TankControl>(out var tankControl))
                {
                    //tankControl.ShellDamage(MaxPH + 1);


                    //这里是原来的计算代码，根据与炮弹距离设置炮弹伤害
                    //float distance = (this.transform.position - (tankRigidbody.position + new(0, 0.4f, 0))).magnitude;
                    //float currentDamage = ((offet + explosionRadius - distance) / (offet + explosionRadius)) * MaxDamage;

                    //第二次更改，这里判断是否在背后攻击，如果是该方式，则增加伤害。
                    //float angle = Vector3.Angle(FireTank.transform.forward, tankControl.transform.forward);
                    //is_back = angle < 90.0f  && (FireTank!= tankControl)? ((90f - angle) / 90f) * 0.5f + 1f : 1f;
                    //float currentDamage = Mathf.Min(MaxDamage, Mathf.Max(0, 10*is_back));
                    //tankControl.ShellDamage(currentDamage);

                    //第三次更改，设置为坦克受到炮弹攻击就会死亡
                    if (!father.CompareTag(tankRigidbody.tag))
                    {
                        //if(!hasCollided)
                        //{
                        //    hasCollided = true;
                        //    tankControl.ShellDamage(RLDemage + 1);  //MaxPH + 1
                        //}
                        tankControl.ShellDamage(RLDemage + 1);  //MaxPH + 1
                        if (!GameObject.Find("TankSpawner").GetComponent<TankSpawner>().useBio)
                        {
                            FireTank = father.GetComponent<TankControl>();
                            FireTank.kill++;

                            if (!FireTank.Isdead)
                            {
                                FireTank.AddReward(1.8f);
                            }
                            else
                            {
                                //father.SetActive(true);
                                FireTank.AddReward(1.6f);
                                //father.SetActive(false);
                            }
                        }
                        else
                        {

                            BioFireTank = father.GetComponent<ManControl>();
                            BioFireTank.kill++;
                            BioFireTank.FireSuccessNum++;
                        }


                    }

                }
                else if (tankColliders[i].gameObject.TryGetComponent<ManControl>(out var ManControl))
                {


                    if (father.CompareTag(tankRigidbody.tag))
                    {
                        //Hurtteammate = true;

                        if (!tankColliders[i].gameObject.activeSelf)
                        {
                            if (father == tankColliders[i].gameObject)
                                ++ManControl.KillSelf;
                            else
                                ++ManControl.KillByMate;
                        }
                    }
                    else
                    {
                        //if (!hasCollided)
                        //{
                        //    hasCollided = true;
                        //    ManControl.ShellDamage(BioDemage + 1); //MaxPH + 1
                        //}
                        ManControl.ShellDamage(15); //MaxPH + 1
                                                    //FireSuccess = true;
                        if (!ManControl.tankSpawner.useTA)
                        {
                            BlueTank = father.GetComponent<TankControl>();
                            if (ManControl.Isdead)
                            {
                                BlueTank.kill++;
                                if (!BlueTank.Isdead)
                                {
                                    BlueTank.AddReward(1.9f);
                                }
                                else
                                {
                                    //father.SetActive(true);
                                    BlueTank.AddReward(1.7f);
                                    //father.SetActive(false);
                                }
                            }
                        }

                    }

                }

            }
        }

        //对于shell进行销毁
        shellExplosion.transform.parent = null;
        if (shellExplosion != null)
        {
            shellExplosion.Play();
            Destroy(shellExplosion.gameObject, shellExplosion.main.duration);
        }
        Destroy(this.gameObject);

    }

    public void FireFeedback(TankControl FireBlueTank)
    {
        int i = 0;
        int Errortimes = 0;
        foreach (var enemy in FireBlueTank.EnemyBiodir)
        {
            if (i >= FireBlueTank.EnemyCount) { break; };

            ManControl Enemy = enemy.Value;

            float shell_distance = Vector3.Distance(Enemy.transform.position, FireBlueTank.shell_pos);

            if (shell_distance < 40)
            {
                FireBlueTank.AddReward((40 - shell_distance) * 0.015f);

            }
            if (shell_distance > 60)
            {
                Errortimes += 1;
            }

            i++;
        }
        if (Errortimes == FireBlueTank.EnemyCount)
        {
            FireBlueTank.AddReward(-0.1f);
        }
    }
}
