using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathOptimize : MonoBehaviour
{

    public TankSpawner tankSpawner;
    //public GameManage gameManage;
    // Start is called before the first frame update
    void Start()
    {
        tankSpawner = FindObjectOfType<TankSpawner>();
    }

}
