using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
	public GameObject[] camerasred;
	public GameObject[] camerasblue;
	public string[] shotcuts;
	public int index = 0;
	public int index1 = 0;

	void Update()
	{
		if(index > camerasred.Length)
        {
			index = 0;
		}

		if (Input.GetKeyDown(KeyCode.Q))
		{
			SwitchCamerared(index);
			index++;
		}

		if (index1 > camerasblue.Length)
		{
			index1 = 0;
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			SwitchCamerablue(index1);
			index1++;
		}

		/*int i = 0;
		for (i = 0; i < cameras.Length; i++)
		{
			if (Input.GetKeyUp(shotcuts[i]))
				SwitchCamera(i);
		}*/
	}

	void SwitchCamerared(int index)
	{
		int i = 0;
		for (i = 0; i < camerasred.Length; i++)
		{
			if (i != index)
			{
				camerasred[i].GetComponent<Camera>().enabled = false;
			}
			else
			{
				camerasred[i].GetComponent<Camera>().enabled = true;
			}
		}
	}

	void SwitchCamerablue(int index1)
	{
		int i = 0;
		for (i = 0; i < camerasblue.Length; i++)
		{
			if (i != index1)
			{
				camerasblue[i].GetComponent<Camera>().enabled = false;
			}
			else
			{
				camerasblue[i].GetComponent<Camera>().enabled = true;
			}
		}
	}
}
