using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class EnemyJudge : MonoBehaviour
{
	public TankSpawner tankSpawner;
	public TranningSetting trainingSetting;
	public GameManage gameManage;
	public BaseFunction baseFunction;
	private int EnemyNum, TeamNum;
	private int MaxLockNum;
	private Dictionary<int, int> enemyLockCount = new Dictionary<int, int>();//���ڼ��ÿ���Ѵ����˷ֱ𱻶�Ӧ���ٸ��ѷ���������
	public int numberOfTeams = 5;
	public int tanksPerTeam = 4;
	private Dictionary<int, int> teamLeaders = new Dictionary<int, int>();
	public List<Transform> tanks = new List<Transform>();

	//���Ķ����㷨ʹ�õ��ı���
	public int[] enemyList;
	public Vector3 TATankCenter;
	public SortedDictionary<float, ManControl> SortEnemyDis = new();
	public SortedDictionary<float, ManControl> BioSameEnemyDir = new();

	void Start()
	{
		trainingSetting = FindObjectOfType<TranningSetting>();
		tankSpawner = FindObjectOfType<TankSpawner>();
		gameManage = FindObjectOfType<GameManage>();
		baseFunction = FindObjectOfType<BaseFunction>();
		EnemyNum = trainingSetting.BlueTeam.nums;
		TeamNum = trainingSetting.RedTeam.nums;
		enemyList = new int[trainingSetting.BlueTeam.nums];
		MaxLockNum = 20;
		//GetAllTanks();
		//InitializeTeams();
	}

    private void FixedUpdate()
    {
		AN_Enemy();

	}
    //�о��ж�
    public void Judge(ManControl man)
	{
		int index = 0;
		float bufferdis = 10000;
		for (int i = 0; i < EnemyNum; i++)
		{
			if (man.EnemyDis[i] < bufferdis)
			{
				bufferdis = man.EnemyDis[i];
				index = i;
			}
		}
		man.MinNum = index + 1;

		if (man.BioEnemydir.Count != 0)// && man.BioEnemydir.Count > 1)
			man.MinNum = findCloseEnemy(man) + 1;


		man.MinNumBuffer[1] = man.MinNumBuffer[0];
		man.MinNumBuffer[0] = man.MinNum;

		if (man.MinNumBuffer[0] != man.MinNumBuffer[1] && (man.MinNumBuffer[0] != -1 && man.MinNumBuffer[1] != -1))
		{
			if (!tankSpawner.useTA)
			{
				TankControl item0 = tankSpawner.BlueAgentsList[man.MinNumBuffer[0] - 1];
				TankControl item1 = tankSpawner.BlueAgentsList[man.MinNumBuffer[1] - 1];
				float dis0 = man.MinNumBuffer[0] != -1 ? Vector3.Distance(man.transform.position, item0.transform.position) : 0.0f;
				float dis1 = man.MinNumBuffer[1] != -1 ? Vector3.Distance(man.transform.position, item1.transform.position) : 0.0f;

				if (Mathf.Abs(dis0 - dis1) < 20 && dis0 > 200 && !item1.Isdead)
				{
					man.MinNum = man.MinNumBuffer[1];
					man.MinNumBuffer[0] = man.MinNumBuffer[1];
				}
				else
				{
					man.SameFireCount = 0;
					man.left_edge = -1;
					man.right_edge = 1;
				}
			}
			else
			{
				ManControl item0 = tankSpawner.TAList[man.MinNumBuffer[0] - 1];
				ManControl item1 = tankSpawner.TAList[man.MinNumBuffer[1] - 1];
				float dis0 = man.MinNumBuffer[0] != -1 ? Vector3.Distance(man.transform.position, item0.transform.position) : 0.0f;
				float dis1 = man.MinNumBuffer[1] != -1 ? Vector3.Distance(man.transform.position, item1.transform.position) : 0.0f;

				if (Mathf.Abs(dis0 - dis1) < 20 && dis0 > 200 && !item1.Isdead)
				{
					man.MinNum = man.MinNumBuffer[1];
					man.MinNumBuffer[0] = man.MinNumBuffer[1];
				}
				else
				{
					man.SameFireCount = 0;
					man.left_edge = -1;
					man.right_edge = 1;
				}
			}
		}

		man.BioAllTeammateDir.Clear();
		//������ж��Ѿ�����Ϣ
		for (int i = 0; i < man.TeamNum; i++)
		{
			ManControl item = tankSpawner.Biolist[i].GetComponent<ManControl>();
			if (item.Isdead == true)
			{
				man.BioAllTeammateDir.TryAdd(man.TeamMateDis[i], item);
			}
		}


		//���minNumΪ-1����ʾ��ʱ��������ԶС�ڼ�������ȡ������������Ķ�����Ϊ����
		if (man.MinNum == -1)
		{
			for (int i = 0; i < man.TeamNum; i++)
			{
				if (tankSpawner.Biolist[i].MinNum != -1 && !tankSpawner.Biolist[i].Isdead)
				{
					man.BioTeanMateDir.TryAdd(man.TeamMateDis[i], tankSpawner.Biolist[i]);
				}

			}
			//print(man.BioTeanMateDir.First().Value.MinNum);
			man.MinNum = man.BioTeanMateDir.First().Value.MinNum;//restartFindEnemy(man, man.BioTeanMateDir);
		}
		man.BioTeanMateDir.Clear();

		man.BioSameEnemyDir.Clear();
		for (int i = 0; i < TeamNum; i++)
		{
			if ((man.MinNum == tankSpawner.Biolist[i].MinNum || i == man.TankNum - 1 || tankSpawner.useTA) && tankSpawner.Biolist[i].Isdead == false)// && i != man.TankNum - 1)
			{
				Vector3 manXOZ = new Vector3(man.transform.position.x, 0, man.transform.position.z);
				Vector3 teamXOZ = tankSpawner.Biolist[i].transform.position;
				teamXOZ = new Vector3(teamXOZ.x, 0, teamXOZ.z);
				Vector3 enemyPos = tankSpawner.useTA ? TATankCenter : tankSpawner.BlueAgentsList[man.MinNum - 1].transform.position;
				enemyPos = new Vector3(enemyPos.x, 0, enemyPos.z);
				man.TeamMateSingleRot[i] = Vector3.SignedAngle(manXOZ - enemyPos, teamXOZ - enemyPos, Vector3.up);//�����Ҹ�
				man.TeamMateRot[i] = Vector3.Angle(manXOZ - enemyPos, teamXOZ - enemyPos);//�����Ҹ�
				if (tankSpawner.useTA)
					man.BioSameEnemyDir.TryAdd(Vector3.Distance(tankSpawner.Biolist[i].transform.position, enemyPos), tankSpawner.Biolist[i]);
				else
					man.BioSameEnemyDir.TryAdd(tankSpawner.Biolist[i].EnemyDis[tankSpawner.Biolist[i].MinNum - 1], tankSpawner.Biolist[i]);

			}
			else
			{
				man.TeamMateSingleRot[i] = 0;
				man.TeamMateRot[i] = 0;

			}

		}

	}


	public void AN_Enemy()
	{
		//�󼺷����ĵ�
		TATankCenter = Vector3.zero;
		int aliveNum = 0;
		foreach (var tank in tankSpawner.TAList)
		{
			tank.MinNum = -1;
			tank.assignFlag = false;
			if (!tank.Isdead)
			{
				aliveNum++;
				TATankCenter += tank.transform.position;
			}

        }
		for (int i = 0; i < tankSpawner.TAList.Count; i++)
		{
			enemyList[i] = 0;

		}
		TATankCenter = TATankCenter / aliveNum;

		//�����־��뼺�����ĵ�����
		SortEnemyDis.Clear();
		foreach (var enemy in tankSpawner.Biolist)
		{
			if(!enemy.Isdead)
            {
				float dis = baseFunction.CalculateDisX0Z(enemy.transform.position, TATankCenter);
				SortEnemyDis.TryAdd(dis, enemy);
			}

		}

		//����������ĵ�����Ķ���
		for (int i = 0; i < SortEnemyDis.Count; i++)
		{
			BioSameEnemyDir.Clear();
			foreach (var tank in tankSpawner.TAList)
			{
				if (!tank.assignFlag && !tank.Isdead)
				{
					float dis = baseFunction.CalculateDisX0Z(tank.transform.position, SortEnemyDis.ElementAt(i).Value.transform.position);
					BioSameEnemyDir.TryAdd(dis,tank);					
				}

			}
			foreach (var tank in tankSpawner.TAList)
			{
				if (!tank.assignFlag && !tank.Isdead && tank.TankNum == BioSameEnemyDir.First().Value.TankNum)
                {
				    enemyList[tank.TankNum] = SortEnemyDis.ElementAt(i).Value.TankNum;
					tank.assignFlag = true;
				}
			}
		}

		foreach (var tank in tankSpawner.TAList)
        {
			if (enemyList[tank.TankNum] == 0 && !tank.Isdead && tank.TAEnemydir.Count != 0) enemyList[tank.TankNum] = tank.TAEnemydir.First().Value.TankNum;
		}
		//foreach (var tank in tankSpawner.TAList)
		//{
		//	tank.MinNum = enemyList[tank.TankNum];
		//}
		//for(int i = 0; i < tankSpawner.TAList.Count; i++)
  //      {
		//	tankSpawner.TAList[i].MinNum = enemyList[i];

		//}
		//if (TATank.MinNum == -1 && tankSpawner.TAList.Count != 0) TATank.MinNum = TATank.TAEnemydir.First().Value.TankNum;
	}

	//�ж�ĳ�������Ƿ�����ڵ����б�
	bool judgeEnemyExist(SortedDictionary<float, TankControl> enemyDir, TankControl judgeEnemy)
	{
		foreach (var enemy in enemyDir.Values)
		{
			if (enemy == judgeEnemy)
				return true;
		}
		return false;
	}

	int findCloseEnemy(ManControl man, int index = 0)
	{
		int enemyCount = 0;
		if ((tankSpawner.useTA ? man.BioEnemydirTA.Count : man.BioEnemydir.Count) < index + 1)
		{
			//(man.TankNum + " : -2");
			man.ResultMinNum = -1;
			return -2;
		}

		man.ResultMinNum = tankSpawner.useTA ? man.BioEnemydirTA.ElementAt(index).Value.TankNum : man.BioEnemydir.ElementAt(index).Value.TankNum;
		man.BioSameEnemyDir.Clear();
		for (int i = 0; i < TeamNum; i++)
		{
			if ((man.ResultMinNum == tankSpawner.Biolist[i].MinNum - 1 || i == man.TankNum - 1) && tankSpawner.Biolist[i].Isdead == false)// && i != man.TankNum - 1)
			{
				man.BioSameEnemyDir.TryAdd(tankSpawner.Biolist[i].EnemyDis[man.ResultMinNum], tankSpawner.Biolist[i]);
				//enemyCount++;
			}
			else
				man.TeamMateRot[i] = 0;
		}
		//print(man.TankNum + ": " + man.BioSameEnemyDir.Count);

		for (int i = 0; i < man.BioSameEnemyDir.Count; i++)
		{
			if (man.BioSameEnemyDir.ElementAt(i).Value.TankNum == man.TankNum)
			{
				enemyCount = i;
				break;
			}
		}
		if (enemyCount < 1)
		{
			return man.ResultMinNum;
		}
		else
		{
			//man.EnemyDis[man.ResultMinNum - 1] = 10001;
			return findCloseEnemy(man, index + 1);

		}

	}



	//�жϹ�����������Ķ�����֮�����������Ķ����Ƿ�������������������Ƚ�˫����Ѫ����������������Ķ���Ѫ�����ͣ��򽫸ö���ȷ��Ϊ����Ŀ�ꡣ
	private void EnemyChoose(ManControl man)
	{

		if (man.LastMinNum != man.ResultMinNum && tankSpawner.BlueAgentsList[man.LastMinNum - 1].Isdead && man.EnemyDis[man.MinNum - 1] < 30 && Mathf.Min(man.EnemyDis) >= 20)
		{
			if ((man.EnemyDis[man.LastMinNum - 1] > man.OpenFireDis + 5) && (man.EnemyRot[man.ResultMinNum - 1] < 15))
			{
				man.LastMinNum = man.ResultMinNum;
				man.MinNum = man.ResultMinNum;
			}

			if (man.EnemyAttackNum >= 2)
			{
				man.LastMinNum = man.ResultMinNum;
				man.MinNum = man.ResultMinNum;
			}
			if ((tankSpawner.BlueAgentsList[man.LastMinNum - 1].PH - tankSpawner.BlueAgentsList[man.ResultMinNum - 1].PH) > 15)
			{
				man.LastMinNum = man.ResultMinNum;
				man.MinNum = man.ResultMinNum;
			}
			else
				man.MinNum = man.LastMinNum;
		}
		else
		{
			man.MinNum = man.ResultMinNum;
			man.LastMinNum = man.ResultMinNum;
		}
	}

	private bool isBeyondMaxNum(int enemyID)
	{
		if (tankSpawner.BlueAgentsList[enemyID - 1].Isdead == false && enemyLockCount.ContainsKey(enemyID))
		{
			if (enemyLockCount[enemyID] >= MaxLockNum)
				return true;
		}
		return false;
	}

	//��������Ŀ��
	private void LockEnemy(int enemyID)
	{
		if (enemyLockCount.ContainsKey(enemyID))
		{
			enemyLockCount[enemyID]++;
		}
		else
		{
			enemyLockCount[enemyID] = 1;
		}
	}

	//��������Ŀ��
	private void UnlockEnemy(int enemyID)
	{
		if (enemyLockCount.ContainsKey(enemyID))
		{
			enemyLockCount[enemyID]--;

			if (tankSpawner.BlueAgentsList[enemyID - 1].Isdead) //����Ŀ�꽫���ֵ�ȥ��
			{
				enemyLockCount.Remove(enemyID);
			}
		}
	}

	public void GetAllTanks()
	{
		for (int i = 0; i < TeamNum; i++)
		{
			Transform tank = tankSpawner.Biolist[i].transform;
			tanks.Add(tank);
		}
	}

	public void InitializeTeams()
	{
		// ��̹�˷�������鲢���öӳ�
		for (int i = 0; i < numberOfTeams; i++)
		{
			for (int j = 0; j < tanksPerTeam; j++)
			{
				int tankIndex = i * tanksPerTeam + j;
				int teamIndex = i + 1;
				tanks[tankIndex].GetComponent<ManControl>().SetTeam(teamIndex);

				if (j == 0)
				{
					// ��һ��̹����Ϊ�ӳ�
					teamLeaders.Add(teamIndex, tankIndex);
					tankSpawner.Biolist[tankIndex].isLeader = true;
				}
			}
		}
	}

	public void TankDestroyed(int teamIndex)
	{
		// ���ӳ����ݻ�ʱ�����ݶӳ�ְλ�������̹��
		if (teamLeaders.ContainsKey(teamIndex) && tankSpawner.Biolist[teamLeaders[teamIndex]].Isdead == true)
		{
			float closestDistance = float.MaxValue;
			int newLeaderIndex = -1;
			tankSpawner.Biolist[teamLeaders[teamIndex]].isLeader = false;
			// Ѱ������Ķ���̹��
			for (int i = 0; i < tanks.Count; i++)
			{
				if (tanks[i].GetComponent<ManControl>().GetTeam() == teamIndex && tankSpawner.Biolist[i].Isdead == false)
				{
					float distance = Vector3.Distance(tanks[i].position, tanks[teamLeaders[teamIndex]].position);
					if (distance < closestDistance)
					{
						closestDistance = distance;
						newLeaderIndex = i;
					}
				}
			}

			if (newLeaderIndex != -1)
			{
				// ���ӳ�ְλ���ݸ������̹��
				teamLeaders[teamIndex] = newLeaderIndex;
			}
		}
	}
}
