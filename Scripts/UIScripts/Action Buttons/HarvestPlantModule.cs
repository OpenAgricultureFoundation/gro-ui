using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class HarvestPlantModule : MonoBehaviour, ISelectionReceiver<FarmSite> {
	
	public Button confirmButton;
	public Button cancelButton;
	public Text contextDisplay;

	[HideInInspector] public Button harvestButton;
	[HideInInspector] public FarmObject activeObject;

	private string context;
	private List<FarmSite> selectedSites = new List<FarmSite>();

	public void Start()
	{
		context = " plants selected.";
		contextDisplay.text = string.Concat("0", context);
	}

	public IEnumerator Initialize()
	{
		//harvestButton.interactable = false;
		ActionButtonManager.actionButtonManager.ModuleStart ();
		EnablePlants (true);

		yield return null;
	}

	public void MakeSelection(FarmSite site)
	{
		if(site.Selected)
		{
			if(!selectedSites.Contains(site))
			{
				selectedSites.Add(site);
			}

		}
		else
		{
			if(selectedSites.Contains(site))
			{
				selectedSites.Remove(site);
			}
		}
		if (selectedSites.Count > 0) 
		{
			contextDisplay.text = selectedSites.Count.ToString() + context;
			confirmButton.interactable = true;
		}
		else
		{
			contextDisplay.text = "0" + context;
			confirmButton.interactable = false;
		}
	}

	public void EnablePlants(bool val)
	{
		FarmTray tray = activeObject.transform.GetComponent<FarmTray> ();

		foreach(FarmSite site in tray.mySites)
		{
			if(!site.isEmpty)
			{
				site.Selectable = val;
				site.selectionReciever = this;
			}
		}
	}

	public void ConfirmButtonPress()
	{
		StartCoroutine ("CompleteHarvest");
	}

	public void CancelButtonPress()
	{
		EndModule ();
	}

	public IEnumerator CompleteHarvest()
	{
		//Each site
		// DeletePlant

		// Update Site
		/*
		byte[] noData = new byte[]{1};
		Dictionary<string, string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/json");
		headers.Add ("X-HTTP-Method-Override", "DELETE");
		*/
		foreach(FarmSite site in selectedSites)
		{
			string plantTypeURL = site.plantTypeNode["url"].Value;
			/*
			WWW www = new WWW(site.plantURL, noData, headers);
			yield return www;
			if(!string.IsNullOrEmpty(www.error))
			{
				print (www.error);
			}
			*/
			
			WWWForm form = new WWWForm();
			form.AddField("site", "");
			form.AddField("plant_type", plantTypeURL);
			object[] parms = new object[2] {site.plantURL,form};
			
			yield return DataManager.dataManager.StartCoroutine("PutRequest", parms);
			
			yield return site.StartCoroutine("UpdateSite");
		}
		
		
		
		

		EndModule();
		yield return null;
	}


	public void EndModule()
	{
		EnablePlants (false);
		//harvestButton.interactable = true;
		ActionButtonManager.actionButtonManager.ModuleEnd ();
		Destroy (transform.gameObject);
	}
}
