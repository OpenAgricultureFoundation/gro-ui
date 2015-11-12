using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class FarmTray : MonoBehaviour {

	public JSONNode node;

	public float margin = 0.03f;
	public List<FarmSite> mySites;
	
	public float marginOffset, rowScale, colScale;
	public float rowOffset, colOffset;

	private FarmManager manager;


	//OLD
	public void PopulateTray()
	{
		float numRows, numCols;

		numRows = node ["num_rows"].AsFloat;
		numCols = node ["num_cols"].AsFloat;

		//marginOffset = Mathf.Min(transform.localScale.z * margin, transform.localScale.x * margin);

		rowOffset = transform.localScale.z * margin;
		colOffset = margin;//transform.localScale.x * margin;

		rowScale = (1.0f - 2 * rowOffset) / (numRows);
		colScale = (1.0f - 2 * colOffset) / (numCols);

		JSONArray sites = node ["plant_sites"].AsArray;
		FarmManager manager = GameObject.FindGameObjectWithTag ("FarmManager").GetComponent<FarmManager> ();
		foreach(JSONNode site in sites)
		{
			GameObject siteObj = Instantiate(manager.sitePrefab) as GameObject;
			FarmSite script = siteObj.GetComponent<FarmSite>();
			script.myTray = this;
			script.StartCoroutine ("Initialize", site.Value);
			mySites.Add(script);
		}
	}

	public IEnumerator Initialize()
	{
		manager = GameObject.FindGameObjectWithTag ("FarmManager").GetComponent<FarmManager> ();

		float numRows, numCols;
		
		numRows = node ["num_rows"].AsFloat;
		numCols = node ["num_cols"].AsFloat;
		
		marginOffset = Mathf.Min(transform.localScale.z * margin, transform.localScale.x * margin);
		
		rowOffset = transform.localScale.z * margin;
		colOffset = transform.localScale.x * margin;
		
		rowScale = (1.0f - 2 * rowOffset) / (numRows);
		colScale = (1.0f - 2 * colOffset) / (numCols);
		
		JSONArray sites = node ["plant_sites"].AsArray;
		foreach (JSONNode site in sites) 
		{
			yield return StartCoroutine("LoadSite", site);
		}

		yield return null;
	}

	public IEnumerator LoadSite(JSONNode site)
	{
		// forreach FarmSite
			// Create Site
			// yield return InitializeSite
		GameObject siteObj = Instantiate(manager.sitePrefab) as GameObject;
		FarmSite script = siteObj.GetComponent<FarmSite>();
		script.myTray = this;
		yield return script.StartCoroutine ("Initialize", site.Value);
		mySites.Add(script);

		yield return null;
	}
}
