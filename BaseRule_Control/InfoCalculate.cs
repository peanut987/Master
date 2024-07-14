using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoCalculate : MonoBehaviour
{
	public TankSpawner tankSpawner;
	private int EnemyNum, TeamNum;
	public TranningSetting trainingSetting;
	public BaseFunction baseFunction;
	// Start is called before the first frame update
	void Start()
	{
		trainingSetting = FindObjectOfType<TranningSetting>();
		baseFunction = FindObjectOfType<BaseFunction>();
		tankSpawner = FindObjectOfType<TankSpawner>();
		EnemyNum = trainingSetting.BlueTeam.nums;
		TeamNum = trainingSetting.RedTeam.nums;
	}

	private void judgeEnemyPos(ManControl red)
    {
		float distance = 0.0f, Matedistance = 0.0f, viewdistance = 1200.0f, TeammateDistance = 600.0f;
		foreach (var blue in tankSpawner.TAList)
		{
			if (blue.isActiveAndEnabled)
			{
				if (red.isActiveAndEnabled)
				{
					RaycastHit Enemyhit;
					bool JudgeEnemy = false;
					Vector3 direction = new Vector3(blue.transform.position.x, blue.transform.position.y + 2, blue.transform.position.z) -
						new Vector3(red.transform.position.x, red.transform.position.y + 2, red.transform.position.z);
					distance = direction.magnitude;

					if (distance <= viewdistance) // 判断距离是否在150以内
					{
						if (Physics.Raycast(new Vector3(red.transform.position.x, red.transform.position.y + 2, red.transform.position.z), direction.normalized, out Enemyhit, distance)) // 射线检测是否有障碍物
						{
							if (Enemyhit.collider != null && Enemyhit.collider.tag == "Tank_Blue") // 判断射线撞击的是否是蓝色坦克
							{
								JudgeEnemy = true;
							}
						}
					}

					if (JudgeEnemy)
					{
						red.TAEnemydir.TryAdd(distance, blue);
					}

				}
			}
		}

		foreach (var Red in tankSpawner.Biolist)
		{
			if (Red.isActiveAndEnabled)
			{
				Red.TAEnemydir1 = new SortedDictionary<float, ManControl>(Red.TAEnemydir);
			}

		}


		//foreach (var red1 in tankSpawner.Biolist)
		//{
		//	if (red1.isActiveAndEnabled)
		//	{
		//		foreach (var red2 in tankSpawner.Biolist)
		//		{
		//			if (red2.isActiveAndEnabled)
		//			{
		//				Matedistance = Vector3.Distance(red1.transform.position, red2.transform.position);
		//				if (Matedistance <= TeammateDistance && red1.TankNum != red2.TankNum)
		//				{
		//					foreach (var ENemy in red2.BioEnemydir1.Values)
		//					{
		//						float ENenydis = Vector3.Distance(red1.transform.position, ENemy.transform.position);
		//						if (!red1.BioEnemydir.ContainsValue(ENemy))
		//						{
		//							red1.BioEnemydir.TryAdd(ENenydis, ENemy);
		//						}

		//					}
		//				}
		//			}

		//		}
		//	}
		//}
	}

	public void judgeEnemyPos1(ManControl blue)
	{
		float distance = 0.0f, Matedistance = 0.0f, viewdistance = 1200.0f, TeammateDistance = 600.0f;
		foreach (var red in tankSpawner.Biolist)
		{
			if (red.isActiveAndEnabled)
			{
				if (blue.isActiveAndEnabled)
				{
					RaycastHit Enemyhit;
					bool JudgeEnemy = false;
					Vector3 direction = new Vector3(red.transform.position.x, red.transform.position.y + 2, red.transform.position.z) -
						new Vector3(blue.transform.position.x, blue.transform.position.y + 2, blue.transform.position.z);
					distance = direction.magnitude;

					if (distance <= viewdistance) // 判断距离是否在150以内
					{
						if (Physics.Raycast(new Vector3(blue.transform.position.x, blue.transform.position.y + 2, blue.transform.position.z), direction.normalized, out Enemyhit, distance)) // 射线检测是否有障碍物
						{
							if (Enemyhit.collider != null && Enemyhit.collider.tag == "Tank_Red") // 判断射线撞击的是否是蓝色坦克
							{
								JudgeEnemy = true;
							}
						}
					}

					if (JudgeEnemy)
					{
						blue.TAEnemydir.TryAdd(distance, blue);
					}

				}
			}
		}

		foreach (var Blue in tankSpawner.Biolist)
		{
			if (Blue.isActiveAndEnabled)
			{
				Blue.TAEnemydir1 = new SortedDictionary<float, ManControl>(Blue.TAEnemydir);
			}

		}


		//foreach (var blue1 in tankSpawner.TAList)
		//{
		//	if (blue1.isActiveAndEnabled)
		//	{
		//		foreach (var red2 in tankSpawner.Biolist)
		//		{
		//			if (red2.isActiveAndEnabled)
		//			{
		//				Matedistance = Vector3.Distance(blue1.transform.position, red2.transform.position);
		//				if (Matedistance <= TeammateDistance && blue1.TankNum != red2.TankNum)
		//				{
		//					foreach (var ENemy in red2.BioEnemydir1.Values)
		//					{
		//						float ENenydis = Vector3.Distance(blue1.transform.position, ENemy.transform.position);
		//						if (!blue1.BioEnemydir.ContainsValue(ENemy))
		//						{
		//							blue1.BioEnemydir.TryAdd(ENenydis, ENemy);
		//						}

		//					}
		//				}
		//			}

		//		}
		//	}
		//}
	}


	//计算队伍存活个体的中心位置
	public Vector3 CalculateCnter(List<ManControl> tank)
    {
		Vector3 teamCenterPos = Vector3.zero;
		int aliveCount = 0;
		foreach (var it in tank)
        {
            if (!it.Isdead)
            {
				teamCenterPos += it.transform.position;
				aliveCount++;

			}
        }
		return teamCenterPos/aliveCount;
    }

	//计算基本信息
	public void Calculate(ManControl man, Transform transform)
	{
		//计算对手距离信息
		//CaculateEnmeyDistance(man);
		//统计存活的友军和敌军数
		man.enemylive = 0;
		man.teammatelive = 0;
		man.Enemy_len = tankSpawner.useTA ? man.BioEnemydirTA.Count : man.BioEnemydir.Count;
		man.pos = man.findEnemy2.judgeSelfPosToCenter(man);
		//judgeEnemyPos(man);
		for (int i = 0; i < EnemyNum; i++)
		{
			if (!tankSpawner.useTA)
			{
				//计算与对手正面的夹角
				float[] TempRot = baseFunction.DotCalculate2(transform, tankSpawner.BlueAgentsList[i].transform);
				man.EnemyRot[i] = TempRot[1];
				man.EnemyDis[i] = 10000;

				//计算存活队友及敌人数
				if (tankSpawner.BlueAgentsList[i].Isdead == false)//&& man.EnemyDis[i] < 60)
					man.enemylive++;
			}
			else
			{
				//计算与对手正面的夹角
				float[] TempRot = baseFunction.DotCalculate2(transform, tankSpawner.TAList[i].transform);
				man.EnemyRot[i] = TempRot[1];
				man.EnemyDis[i] = 10000;

				//计算存活队友及敌人数
				if (tankSpawner.TAList[i].Isdead == false)//&& man.EnemyDis[i] < 60)
					man.enemylive++;
			}
		}

		if (man.BioEnemydir.Count != 0 || man.BioEnemydirTA.Count != 0)
		{
			if (!tankSpawner.useTA)
			{
				foreach (var RlTank in man.BioEnemydir)
				{

					man.EnemyDis[RlTank.Value.TankNum] = RlTank.Key;
				}
			}
			else
			{
				foreach (var TATank in man.BioEnemydirTA)
				{

					man.EnemyDis[TATank.Value.TankNum] = TATank.Key;
				}
			}
		}

		//计算队友距离信息
		for (int i = 0; i < TeamNum; i++)
		{
			if (i != man.TankNum - 1 && tankSpawner.Biolist[i].Isdead == false)
				man.TeamMateDis[i] = (tankSpawner.Biolist[i].transform.position - transform.position).magnitude;
			else
				man.TeamMateDis[i] = 10000.0f;
		}

		for (int i = 0; i < TeamNum; i++)
		{
			if (tankSpawner.Biolist[i].Isdead == false)
				man.teammatelive++;
		}

		for (int i = 0; i < EnemyNum; i++)
		{
			if (man.EnemyRot[i] < 15 && man.EnemyDis[i] < 45)
				man.EnemyAttackNum++;
		}

	}
}
