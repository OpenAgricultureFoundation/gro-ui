using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public class AddPlantModule : MonoBehaviour, ISelectionReceiver<FarmSite>, ISelectionReceiver<PlantTypeChoice>, IRemover<PlantTypeChoice> {

	public FarmObject activeObject;
	public Button AddPlantButton;
	public Button selectButton;
	public Button cancelButton;
	public Button backButton;
	public Button AddNewPlantTypeButton;
	public Transform choicePanelScrollContent;
	public Transform selectionIconDisplay;
	public Transform topText;
	public string baseURL;
	public GameObject plantTypeChoicePrefab;
	public GameObject testPlantPrefab;
	public GameObject newPlantTypeModulePrefab;

	private PlantTypeChoice selectedPlantType;
	private List<FarmSite> selectedSites = new List<FarmSite>();
	private Dictionary<string, JSONNode> plantTypeDict = new Dictionary<string, JSONNode> ();
	private string[] instructions;
	private string plantTypeURL;
	private bool emptySitesEnabled = false;


	private int step = 0;
	private float[] height;
	private float[] width;
	

	public void Start()
	{
		instructions = new string[] {
			"Step 1. Choose a plant type",
			"Step 2. Select empty site(s)"
		};

	}

	public IEnumerator Initialize()
	{
		choicePanelScrollContent = transform.FindChild("ChoicesPanel").FindChild("ChoiceScrollView").FindChild("ChoiceScrollContent");
		//AddPlantButton.interactable = false;
		ActionButtonManager.actionButtonManager.ModuleStart ();
		string plantTypeURLSuffix = "plantType/";
		plantTypeURL = string.Concat (baseURL, plantTypeURLSuffix);

		yield return StartCoroutine ("GetPlantTypes");

		CameraManager.cameraManager.TopDownView ();
		
		yield return null;
	}

	private IEnumerator GetPlantTypes()
	{
		// Query URL for all plant Types
		WWW www = new WWW (plantTypeURL);
		yield return www;
		if(!string.IsNullOrEmpty(www.error))
		{
			print (www.error);
		}

		JSONNode node = JSON.Parse (www.text);

		JSONArray typeArray = node["results"].AsArray;

		foreach(JSONNode plantType in typeArray)
		{
			if(!plantTypeDict.ContainsKey(plantType["url"].Value))
			{
				plantTypeDict.Add(plantType["url"].Value, plantType);
				CreateChoice(plantType);
			}
		}
		// Check if in dict, and put in if needed
		// Once in dict, add to list also via CreateChoice()

		yield return null;
	}

	private void CreateChoice(JSONNode plantType)
	{
		// Get JSONNode from dict via url
		// Instantiate
		// Set Parent
		// Set script variables

		GameObject choice = Instantiate(plantTypeChoicePrefab) as GameObject;
		PlantTypeChoice script = choice.GetComponent<PlantTypeChoice>();
		
		choice.transform.SetParent(choicePanelScrollContent, false);
		choice.GetComponentInChildren<Toggle>().group = GetComponent<ToggleGroup>();
		// set script variables
		script.selectionReceiver = this;
		
		script.SetCommonName(plantType ["common_name"].Value);
		script.SetLatinName(plantType["latin_name"].Value);
		script.url = plantType["url"].Value;
		script.remover = this;
		script.plantTypeNode = plantType;
		if (plantType ["plant_count"].AsInt > 0) 
		{
			script.removeButton.gameObject.SetActive(false);
		}
	}
	
	private void EnableEmptySites(bool val)
	{
		FarmTray tray = activeObject.transform.GetComponent<FarmTray> ();

		foreach(FarmSite site in tray.mySites)
		{
			if(site.isEmpty)
			{
				site.Selectable = val;
				site.selectionReciever = this;
			}
			else 
			{
				site.Selectable = (!val);
			}
		}
		emptySitesEnabled = val;
	}

	public void MakeSelection(FarmSite site)
	{
		/*
		if(site.Selected)
		{
			selectedSites.Add(site);
		}
		else
		{
			if(selectedSites.Contains(site))
			{
				selectedSites.Remove(site);
			}
		}
		*/

		if (emptySitesEnabled) 
		{
			site.Selectable = false;
			StartCoroutine("CreatePlant", site);
		}

	}
	public void MakeSelection(PlantTypeChoice choice)
	{
		// Set selectedPlantType
		if (choice.selected) 
		{
			selectedPlantType = choice;
		}
		// Enable selectButton
		selectButton.interactable = true;
		// Set Icon - Implement later

		if(!emptySitesEnabled)
		{
			EnableEmptySites(true);
		}
	}

	public void DoneButtonPress()
	{
		//GameObject.FindWithTag ("HarvestPanel").SetActive (true);
		//GameObject.FindWithTag ("AdjustPanel").SetActive (true);
		//GameObject.FindWithTag ("DownloadPanel").SetActive (true);		
		EndModule ();
	}

	public void AddPlantType()
	{
		transform.gameObject.SetActive (false);
		// Instantiate Add type module
		GameObject newPlantType = Instantiate (newPlantTypeModulePrefab) as GameObject;
		newPlantType.transform.SetParent (GameObject.FindGameObjectWithTag ("GUICanvas").transform, false);

		// Initialize Module (if needed) and set variables
		NewPlantTypeModule script = newPlantType.GetComponent<NewPlantTypeModule> ();
		script.plantTypeURL = plantTypeURL;
		script.addPlantModule = this;
	}

	public IEnumerator CreatePlant(FarmSite site)
	{
		// get values for the plant
		string now = System.DateTime.UtcNow.ToString ("o");
		string plantURL = DataManager.dataManager.ipAddress + "/plant/";
		string typeURL = selectedPlantType.url;
		string siteURL = site.url;

		WWWForm form = new WWWForm ();
		form.AddField ("sown_date", now);
		form.AddField ("plant_type", typeURL);
		form.AddField ("site", siteURL);
		
		object[] parms = new object[2] {plantURL, form};
		
		yield return DataManager.dataManager.StartCoroutine("PostRequest", parms);
		
		yield return site.StartCoroutine("UpdateSite");

		yield return null;
	}

	public void Remove(PlantTypeChoice item)
	{
		plantTypeDict.Remove (item.url);

		DataManager.dataManager.StartCoroutine ("DeleteRequest", item.url);

		Destroy (item.gameObject);


	}

	public IEnumerator CompletePlanting()
	{
		// May need to be a coroutine

		// Create Plants
		foreach(FarmSite site in selectedSites)
		{
			yield return StartCoroutine("CreatePlant", site);
		}
		EndModule();

		yield return null;
	}

	public void EndModule()
	{
		EnableEmptySites (false);
		//AddPlantButton.interactable = true;
		CameraManager.cameraManager.ReturnToInterimRotation ();
		ActionButtonManager.actionButtonManager.ModuleEnd ();
		Destroy (transform.gameObject);
	}


}
