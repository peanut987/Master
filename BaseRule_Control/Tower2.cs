using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower2 : MonoBehaviour
{
    public Transform tankTransform;
    public TankSpawner tankSpawner;
    public ManControl manControl2;
    public Vector3 TowerDir;
    public float angle;
    public int minnum;
    private RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        tankTransform = transform.parent.parent;
        manControl2 = tankTransform.GetComponent<ManControl>();
        tankSpawner = FindObjectOfType<TankSpawner>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!manControl2.trainingSetting.RedTeam.HumanControl)
        {
            Vector3 target1 = (manControl2.MinNum != 0 && manControl2.tankTeam != TankTeam.Tank_Blue) ? 
                (tankSpawner.useTA ? 
                tankSpawner.TAList[manControl2.MinNum - 1].transform.position : tankSpawner.BlueAgentsList[manControl2.MinNum - 1].transform.position) : Vector3.zero;
            Vector3 target2 = (manControl2.tankTeam == TankTeam.Tank_Blue &&  tankSpawner.Biolist.Count != 0 && manControl2.MinNum != 0 && manControl2.MinNum != -1) ? tankSpawner.Biolist[manControl2.MinNum - 1].transform.position : Vector3.zero;
            Vector3 target = manControl2.tankTeam == TankTeam.Tank_Red ? target1 : target2;
            minnum = manControl2.MinNum - 1;
            TowerDir = (target - transform.position).normalized;
            Vector3 cannonDir = manControl2.cannon.forward;

            cannonDir = new Vector3(cannonDir.x, 0, cannonDir.z);
            TowerDir = new Vector3(TowerDir.x, 0, TowerDir.z);

            angle = Vector3.SignedAngle(TowerDir, cannonDir, Vector3.up);//×óÕýÓÒ¸º

            if (angle > 0.1) transform.Rotate(-Vector3.up, 0.6f);
            else if (angle < -0.1f) transform.Rotate(Vector3.up, 0.6f);

        }
    }
}
