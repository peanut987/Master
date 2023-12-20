using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRules : MonoBehaviour
{
	int timer;
    // Start is called before the first frame update
    void Start()
    {
		timer = 0;
    }

    public void tankBebaviorControl(ManControl man, int timeScale = 100)
    {
		man.timeBetween += 1;//计时
		int moveTankNum = (man.TankNum - 1) % 4;//分成5个小队
		if (moveTankNum == 0 || man.team == 1) man.startFlag = 1;
		if (man.team > 2) timer = 300;
		else if (man.team == 1) timer = 50; 
		if (man.timeBetween > timer + (moveTankNum-1) * timeScale && man.timeBetween < timer + (moveTankNum) * timeScale) man.startFlag = 1;//开始移动
    }
    //
}
