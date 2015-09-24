using UnityEngine;
using System.Collections;
using SimpleJSON;

public class FarmSite : MonoBehaviour {

	public string url;
	public FarmTray myTray;
	public int row, col;
	public string plantName;
	public bool isEmpty;
	[HideInInspector] public string plantURL = "";
	public GameObject siteObj;
	public GameObject plant;

	public ISelectionReceiver<FarmSite> selectionReciever;


	public JSONNode node, plantNode, plantTypeNode;

	private bool selected = false;
	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			selectionReciever.MakeSelection(this);
		}
	}

	private bool selectable = false;
	public bool Selectable
	{
		get
		{
			return selectable;
		}
		set
		{
			selectable = value;
			FarmPlant script = plant.GetComponent<FarmPlant>();
			script.selectable = value;
			script.hoverable = selectable || (!isEmpty);
			/*
			FarmSiteObject script = siteObj.GetComponent<FarmSiteObject>();
			script.clickable = selectable;
			script.hoverable = selectable || (!isEmpty);
			if(!value)
			{
				siteObj.GetComponent<FarmSiteObject>().Deselect();
			}
			*/
		}
	}

	public IEnumerator LoadPlant()
	{
		// Create plant (FarmSiteObject)
		// yield return InitializePlant
		WWW www;
		GameObject plantModel;
		string plantCommonName = "empty";
		isEmpty = true;
		if (node ["plant"].Value != "null") 
		{
			plantURL = node["plant"].Value;

			www = new WWW(plantURL);
			yield return www;
			plantNode = JSON.Parse(www.text);
			
			www = new WWW(plantNode["plant_type"].Value);
			yield return www;
			plantTypeNode = JSON.Parse (www.text);

			isEmpty = false;
			plantCommonName = plantTypeNode["common_name"];
		}

		plantModel = FarmManager.farmManager.GetPlantModel(plantCommonName);

		GameObject plantObj = Instantiate (plantModel) as GameObject;
		FarmPlant script = plantObj.GetComponent<FarmPlant>();

		plant = plantObj;

		plantObj.transform.SetParent (transform);
		plantObj.name = plantCommonName;
		script.site = this;
		script.isEmpty = isEmpty;
		if (!isEmpty) 
		{
			script.plantNode = plantNode;
			script.plantTypeNode = plantTypeNode;
		}

		yield return script.StartCoroutine ("Initialize");

		yield return null;
	}

	public IEnumerator Initialize(string URL)
	{
		// Query URL, parse
		WWW www = new WWW (URL);
		yield return www;

		node = JSON.Parse (www.text);

		// Set object variables
		url = URL;

		row = node ["row"].AsInt;
		col = node ["col"].AsInt;
		transform.name = "Site " + row + "-" + col;

		// Set scale, then parent, then position

		float scalar = Mathf.Min (myTray.rowScale / 2.0f, myTray.colScale / 2.0f);
		Vector3 scale = new Vector3 (scalar, scalar/2f, scalar);
		Vector3 pos = new Vector3 (-0.5f + myTray.colOffset + (1.0f * col + 0.5f) * myTray.colScale,
		                           0.5f,
		                           -0.5f + myTray.rowOffset + (1.0f * row + 0.5f) * myTray.rowScale);

		transform.localScale = scale;
		transform.SetParent (myTray.transform);
		transform.localPosition = pos;
		/*
		if (node ["plant"].Value != "null") 
		{
			string url2 = node["plant"].Value;
			www = new WWW(url2);
			yield return www;
			plantNode = JSON.Parse(www.text);

			www = new WWW(plantNode["plant_type"].Value);
			yield return www;
			plantTypeNode = JSON.Parse (www.text);

		}
		*/
		yield return StartCoroutine ("LoadPlant");

		yield return null;
	}



	public IEnumerator SetPlant()
	{

		FarmManager manager = GameObject.FindGameObjectWithTag ("FarmManager").GetComponent<FarmManager> ();

		GameObject siteObject;

		if(node["plant"].Value != "null")
		{
			isEmpty = false;
			Vector3 lp, ls;
			/*
			PlantLibrary pLib = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlantLibrary>();

			if(false)//pLib.PlantLibraryDict.ContainsKey(plantNode["plant_type"]["common_name"].Value))
			{
				siteObject = Instantiate(pLib.PlantLibraryDict[plantNode["plant_type"]["common_name"].Value]) as GameObject;
				lp = new Vector3(0f, -1f, 0f);
				ls = new Vector3(0.15f, 0.15f, 0.15f);
			}


			*/
			if(false){}
			else
			{
				siteObject = Instantiate(manager.testPlantLeaf) as GameObject;
				lp = new Vector3(0f, 1f, 0f);
				ls = new Vector3(1f, 1f, 0.03f);
			}

			plantURL = node["plant"];
			plantName = plantTypeNode["common_name"].Value;
			//siteObject = Instantiate(manager.testSitePrefab) as GameObject;
			//siteObject = Instantiate(manager.testPlantLeaf) as GameObject;
			siteObject.transform.SetParent(transform);
			siteObject.transform.localPosition = lp;
			siteObject.transform.localScale = ls;
			siteObj = siteObject;
			FarmSiteObject script = siteObject.GetComponent<FarmSiteObject>();
			script.mySite = this;
			script.hoverable = true;
			script.name = plantName;

		}
		else
		{
			isEmpty = true;
			plantName = "EmptySite";
			siteObject = Instantiate(manager.emptySite) as GameObject;
			siteObject.transform.SetParent(transform);
			siteObject.transform.localPosition = new Vector3(0,0,0);
			siteObj = siteObject;
			FarmSiteObject script = siteObject.GetComponent<FarmSiteObject>();
			script.mySite = this;
			script.hoverable = false;
			script.name = plantName;
		}

		yield return null;
	}


	
	/*
	public IEnumerator SetPlant()
	{
		WWW www;

		if (node ["plant"].Value != "null") 
		{
			www = new WWW(node["plant"].Value);
			yield return www;
			plantNode = JSON.Parse(www.text);
			
			www = new WWW(plantNode["plant_type"].Value);
			yield return www;
			plantTypeNode = JSON.Parse (www.text);
			
			isEmpty = false;
		}

		if (false) 
		{

		} 
		else 
		{

		}

		yield return null;
	}
	*/


	public IEnumerator UpdateSite()
	{
		Destroy (plant.gameObject);
		WWW www = new WWW (url);
		yield return www;

		node = JSON.Parse (www.text);

		yield return StartCoroutine ("LoadPlant");
		/*
		if (node ["plant"].Value != "null") 
		{
			www = new WWW(node["plant"].Value);
			yield return www;
			plantNode = JSON.Parse (www.text);

			www = new WWW(plantNode["plant_type"].Value);
			yield return www;
			plantTypeNode = JSON.Parse (www.text);
		}

		yield return StartCoroutine ("SetPlant");

		*/
		yield return null;

	}
	

}
