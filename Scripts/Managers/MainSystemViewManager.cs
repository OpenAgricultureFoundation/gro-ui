using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainSystemViewManager : MonoBehaviour {

	public string defaultQueryURL;

	public string BaseURL;
	public string enclosureSuffix;

	public GameObject FarmManager;
	public GameObject LoadingScreen;


	// Use this for initialization
	void Start () 
	{
		if (!string.IsNullOrEmpty(DataManager.dataManager.ipAddress))
		{
			BaseURL = DataManager.dataManager.ipAddress + "/";
		}
		else
		{
			BaseURL = GetURL();
		}
		StartCoroutine ("InitializeScene");
		//StartCoroutine ("Test1");
	}

	IEnumerator InitializeScene ()
	{
		LoadingScreen.SetActive (true);
		/*
		string url = GetURL ();
		yield return url;
		BaseURL = url;
		*/
		string farmQuery = BaseURL + enclosureSuffix;
		//FarmManager.GetComponent<FarmManager> ().InitializeFarm (farmQuery);
		yield return FarmManager.GetComponent<FarmManager> ().StartCoroutine ("Initialize", farmQuery);

		LoadingScreen.SetActive (false);
	}

	string GetURL()
	{
		// Get URL for the farm here

		// Temporarily return our default URL
		return defaultQueryURL;
	}

}
