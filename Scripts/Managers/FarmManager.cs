using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class FarmManager : MonoBehaviour {

	[HideInInspector] public static FarmManager farmManager;

	public GameObject enclosurePrefab;
	public GameObject genericPrefab;
	public GameObject trayPrefab;

	public GameObject sitePrefab;
	public GameObject emptySite;
	public GameObject testSitePrefab;
	public GameObject testPlantLeaf;
	//public GameObject samplePlant1;

	public GameObject sensingPointPrefab;

	public Button AddPlantButton;
	public Button HarvestButton;
	public GameObject AddPlantModulePrefab;
	public GameObject HarvestModulePrefab;
	public GameObject AdjustSystemModulePrefab;
	public GameObject DownloadDataModulePrefab; 

	// bool to keep track of when panels are expanded. 
	public bool HarvestExpanded=false;
	public bool AddPlantExpanded=false;
	public bool AdjustExpanded=false;
	public bool DownloadExpanded=false;

	public List<PlantModelMapEntry> plantModelMap;
	public GameObject defaultPlantModel;

	public Transform activeObject;

	public void Awake()
	{
		if (farmManager == null) 
		{
			farmManager = this;
		}
	}

	public void InitializeFarm (string URL) 
	{
		GameObject initialPrefab = genericPrefab;
		string[] urlSplit = URL.Split ("/" [0]);
		//string objType = urlSplit [urlSplit.Length - 2];
		
		foreach (string split in urlSplit) 
		{
			if(string.Compare(split, "enclosure", true) == 0)
			{
				initialPrefab = enclosurePrefab;
				break;
			}
			else if(string.Compare(split, "tray", true) == 0 )
			{
				initialPrefab = trayPrefab;
				break;
			}
		}

		GameObject firstObject = Instantiate(initialPrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
		//firstObject.transform.parent.SetParent (transform);
		firstObject.GetComponent<FarmObject> ().parentTransform = transform;

		firstObject.GetComponent<FarmObject> ().StartCoroutine("Initialize", URL);
		//firstObject.GetComponent<FarmObject> ().Build (URL);

		StartCoroutine (InitializerHelp (firstObject));

	}

	IEnumerator InitializerHelp(GameObject obj)
	{
		//yield return new WaitForSeconds(2f);
		obj.GetComponent<FarmObject> ().childrenTransforms [0].GetComponent<FarmObject> ().selected ();
		yield return null;
	}

	public IEnumerator Initialize(string URL)
	{
		// Query
		// Setup

		// Load
		yield return StartCoroutine ("LoadFarmObject", URL);
		// Return
		yield return null;
	}

	public IEnumerator LoadFarmObject(string URL)
	{
		// Create first object
		// yield return InitializeFirstObject
		// Set fisrt active object
		GameObject enclosure = Instantiate (enclosurePrefab, new Vector3 (0, 0, 0), Quaternion.identity) as GameObject;
		FarmObject script = enclosure.GetComponent<FarmObject> ();
		script.parentTransform = transform;

		yield return script.StartCoroutine ("Initialize", URL);

		yield return StartCoroutine ("InitializerHelp", enclosure);

		yield return null;
	}

	public void SetActiveObject (Transform newActiveObject)
	{
		// Set parent to this this transform;
		newActiveObject.SetParent (transform);
		newActiveObject.GetComponent<Collider> ().enabled = false;
		newActiveObject.GetComponent<FarmObject> ().parentTransform.GetComponent<FarmObject> ().deactivate ();
		foreach(Transform child in newActiveObject.GetComponent<FarmObject>().childrenTransforms)
		{
			child.GetComponent<FarmObject>().activate();
			child.GetComponent<Collider>().enabled = true;
		}
		if (!(newActiveObject.GetComponent<FarmObject>().isTrayObject))
		{
			newActiveObject.gameObject.SetActive(false);

			AddPlantButton.enabled = false;
		}
		else
		{
			// Enable add plant button
			AddPlantButton.enabled = true;

		}
		// If not tray, make gameObject inactive
		// Turn off collider
		// Activate all children
		// Deactivate parentTransform

		activeObject = newActiveObject;
		GameObject cam = GameObject.FindGameObjectWithTag ("MainCamera");
		cam.GetComponent<CameraManager> ().MoveToObject (newActiveObject);
	}


	// Tool Bar Module loaders

	public void StartAddPlantModule()
	{
		if (AddPlantExpanded == false) {
			// Instantiate module
			GameObject module = Instantiate (AddPlantModulePrefab) as GameObject;
			AddPlantModule script = module.GetComponent<AddPlantModule> ();

			//shiftin prefab to adding to addplat button
			module.transform.SetParent (GameObject.FindGameObjectWithTag ("AddPlantPanel").transform, false);
			module.transform.SetAsFirstSibling ();

			//GameObject.FindWithTag ("HarvestPanel").GetComponent<RectTransform>().
			//GameObject.FindWithTag ("AdjustPanel").SetActive (false);
			//GameObject.FindWithTag ("DownloadPanel").SetActive (false);

			// Set active object
			script.activeObject = activeObject.GetComponent<FarmObject> ();
			// Set AddPlantButton
			script.AddPlantButton = AddPlantButton;
			// Set BaseURL
			script.baseURL = GameObject.FindGameObjectWithTag ("GameController").GetComponent<MainSystemViewManager> ().BaseURL;
			// Call initializer
			script.StartCoroutine ("Initialize");
			AddPlantExpanded = true;
			return;
		}
		if (AddPlantExpanded = true) {
			GameObject.FindWithTag("AddPlantModule").GetComponent<AddPlantModule>().DoneButtonPress();
			AddPlantExpanded = false;
			return;
		}
	}

	public void StartHarvestModule()
	{

		if (HarvestExpanded == false) {
			// Instantiate prefab
			GameObject module = Instantiate (HarvestModulePrefab) as GameObject;
			HarvestPlantModule script = module.GetComponent<HarvestPlantModule> ();

			//shifting harvest to new parent 
			module.transform.SetParent (GameObject.FindGameObjectWithTag ("HarvestPanel").transform, false);
			module.transform.SetAsFirstSibling ();

			script.activeObject = activeObject.GetComponent<FarmObject> ();
			script.harvestButton = HarvestButton;
			script.StartCoroutine ("Initialize");
			HarvestExpanded = true;
			return;
		}
		if (HarvestExpanded == true) {
			GameObject.FindWithTag("HarvestPlantModule").GetComponent<HarvestPlantModule>().CancelButtonPress();
			HarvestExpanded = false;
			return;
		}
	}

	public void StartAdjustModule()
	{

		if (AdjustExpanded == false) {
			GameObject module = Instantiate (AdjustSystemModulePrefab) as GameObject;

			//shiftig adjust to new parent

			module.transform.SetParent (GameObject.FindGameObjectWithTag ("AdjustPanel").transform, false);
			module.transform.SetAsFirstSibling ();

			AdjustSystemModule script = module.GetComponent<AdjustSystemModule> ();
			script.activeFarmObject = activeObject.GetComponent<FarmObject> ();
			script.StartCoroutine ("Initialize");
			AdjustExpanded = true;
			return;
		}
		if (AdjustExpanded = true) {
			GameObject.FindWithTag("AdjustSystemModule").GetComponent<AdjustSystemModule>().CloseButtonPress();
			AdjustExpanded = false;
			return;
		}
	}
	
	public void StartDownloadDataModule()
	{
		if (DownloadExpanded == false) {

			GameObject module = Instantiate (DownloadDataModulePrefab) as GameObject;

			//shifting to new parent
			module.transform.SetParent (GameObject.FindGameObjectWithTag ("DownloadPanel").transform, false);
			module.transform.SetAsFirstSibling ();

			DownloadDataModule script = module.GetComponent<DownloadDataModule> ();
			script.StartCoroutine ("Initialize");
			DownloadExpanded = true;
			return;
		}

		if (DownloadExpanded = true){
			GameObject.FindWithTag("DownloadDataModule").GetComponent<DownloadDataModule>().CloseButtonPress();
			DownloadExpanded=false;
			return;
		}

	}

	public GameObject GetPlantModel(string plantType)
	{
		if (plantType == "empty") {
			return emptySite;
		}
		else
		{
			foreach(PlantModelMapEntry entry in plantModelMap)
			{
				if(string.Compare(entry.plantType, plantType, true) == 0)
				{
					return entry.plantModel;
				}
			}
			return defaultPlantModel;
		}
	}
}

[System.Serializable]
public class PlantModelMapEntry
{
	public string plantType;
	public GameObject plantModel;
}
