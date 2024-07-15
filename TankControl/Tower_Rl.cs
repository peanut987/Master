using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tower_Rl : MonoBehaviour
{
    public Transform tankTransform;
    public TankSpawner tankSpawner;
    public TankControl tankControl;
    public Vector3 TowerDir;

    public float angle;
    public float angle2;
    public int minnum;
    private RaycastHit hit;

    float JustAngle;
    // Start is called before the first frame update
    void Start()
    {
        tankTransform = transform.parent.parent;
        tankControl = tankTransform.GetComponent<TankControl>();
        tankSpawner = FindObjectOfType<TankSpawner>();

        JustAngle = 0.1f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (tankControl.trainingSetting.BlueTeam.HumanControl)
        {
            Vector3 target = tankControl.attackEnemy != 0 ? tankSpawner.Biolist[tankControl.attackEnemy - 1].transform.position : Vector3.zero;
            minnum = tankControl.attackEnemy - 1;
            TowerDir = (target - transform.position).normalized;
            Vector3 cannonDir = tankControl.cannon.forward;

            cannonDir = new Vector3(cannonDir.x, 0, cannonDir.z);
            TowerDir = new Vector3(TowerDir.x, 0, TowerDir.z);

            angle = Vector3.SignedAngle(TowerDir, cannonDir, Vector3.up);//左正右负

            //if (angle > 0.1) transform.Rotate(-Vector3.up, 0.6f);
            //else if (angle < -0.1f) transform.Rotate(Vector3.up, 0.6f);
        }
        else
        {
            if (tankControl.EnemyBiodir.Count != 0 && tankControl.Isdead == false)
            {
                ManControl enemY = tankControl.EnemyBiodir.First().Value;
                Vector3 target = enemY.transform.position;

                TowerDir = (target - transform.position).normalized;
                Vector3 cannonDir = tankControl.Tower.forward;

                Vector3 cannonDir1 = new Vector3(cannonDir.x, 0, cannonDir.z);
                Vector3 TowerDir1 = new Vector3(TowerDir.x, 0, TowerDir.z);

                angle = Vector3.SignedAngle(TowerDir1, cannonDir1, Vector3.up);//左正右负

                if (angle > 0.1) transform.Rotate(-Vector3.up, 0.6f);
                else if (angle < -0.1f) transform.Rotate(Vector3.up, 0.6f);


                if (Vector3.Angle(TowerDir1, cannonDir1) < 5 )//&& UnityEngine.Random.Range(0, 1) > JustAngle)
                {
                    float enemyAngle = Vector3.Angle(target, tankControl.ShellPos.forward);
                    float enemyAngle1 = target.normalized.y > tankControl.cannon.transform.forward.normalized.y ?
                               enemyAngle : -enemyAngle;

                    enemyAngle1 = enemyAngle1 < 25.0f ? enemyAngle1 : 25.0f;
                    enemyAngle1 = enemyAngle1 > -5.0f ? enemyAngle1 : -5.0f;

                    //炮筒旋转
                    if (-(tankControl.baseFunction.angleOffset(tankControl.cannon.localRotation.eulerAngles.x)) >= 0.0f)
                    {
                        if (enemyAngle1 < 0)
                        {
                            tankControl.cannon.Rotate(Vector3.right, 0.3f);
                        }
                        else if (enemyAngle1 < -2)
                        {
                            tankControl.cannon.Rotate(Vector3.right, 0.6f);
                        }
                    }
                    if (-(tankControl.baseFunction.angleOffset(tankControl.cannon.localRotation.eulerAngles.x)) <= 35.0f)
                    {
                        if (enemyAngle1 >= 2)
                        {
                            tankControl.cannon.Rotate(-Vector3.right, 0.6f);
                        }
                        else if (enemyAngle1 >= 0)
                        {
                            tankControl.cannon.Rotate(-Vector3.right, 0.3f);
                        }
                    }

                }

            }
        }

    }
}
