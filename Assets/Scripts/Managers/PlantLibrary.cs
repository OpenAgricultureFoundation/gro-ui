using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlantLibrary : MonoBehaviour {

	public Dictionary<string, GameObject> PlantLibraryDict = new Dictionary<string, GameObject>();

	public GameObject[] PlantModelList = new GameObject[] {};
	public string[] PlantNameList = new string[] {};

	void Start()
	{
		for(int i = 0; i < PlantNameList.Length; i++)
		{
			PlantLibraryDict.Add (PlantNameList[i], PlantModelList[i]);
		}
	}


}
