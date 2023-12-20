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



	//���������Ϣ
	public void Calculate(ManControl man, Transform transform)
	{
		//������־�����Ϣ
		//CaculateEnmeyDistance(man);
		//ͳ�ƴ����Ѿ��͵о���
		man.enemylive = 0;
		man.teammatelive = 0;
		man.Enemy_len = tankSpawner.useTA ? man.BioEnemydirTA.Count : man.BioEnemydir.Count;
		for (int i = 0; i < EnemyNum; i++)
		{
			if (!tankSpawner.useTA)
			{
				//�������������ļн�
				float[] TempRot = baseFunction.DotCalculate2(transform, tankSpawner.BlueAgentsList[i].transform);
				man.EnemyRot[i] = TempRot[1];
				man.EnemyDis[i] = 10000;

				//��������Ѽ�������
				if (tankSpawner.BlueAgentsList[i].Isdead == false)//&& man.EnemyDis[i] < 60)
					man.enemylive++;
			}
			else
			{
				//�������������ļн�
				float[] TempRot = baseFunction.DotCalculate2(transform, tankSpawner.TAList[i].transform);
				man.EnemyRot[i] = TempRot[1];
				man.EnemyDis[i] = 10000;

				//��������Ѽ�������
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

		//������Ѿ�����Ϣ
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
