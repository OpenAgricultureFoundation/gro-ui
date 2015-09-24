using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using SimpleJSON;

public class AdjustSystemModule : MonoBehaviour, IReactivatable {

	public GameObject actuatorOverridePrefab, chooseRecipeModulePrefab;
	public Button changeRecipeButton;
	public Text currentRecipeDisplay;

	[HideInInspector] public FarmObject activeFarmObject;
	[HideInInspector] public FarmTray activeTrayObject;

	private Transform overrideScrollContent;
	private string currentRecipe;

	public IEnumerator Initialize()
	{

		ActionButtonManager.actionButtonManager.ModuleStart ();
		FarmActuatorManager actManager = GameObject.FindGameObjectWithTag("FarmActuatorManager").GetComponent<FarmActuatorManager>();
		overrideScrollContent = transform.FindChild ("ManualOverridePanel").FindChild ("MOScrollRect").FindChild ("MOScrollContent");
		foreach (FarmActuator act in actManager.actuatorList) 
		{
			CreateOverrideModule(act);
		}
		if (activeFarmObject.isTrayObject) 
		{
			activeTrayObject = activeFarmObject.gameObject.GetComponent<FarmTray>();
			WWW www = new WWW(activeFarmObject.myURL);
			yield return www;
			JSONNode node = JSON.Parse (www.text);
			if(node["current_recipe_run"].Value != "null")
			{
				currentRecipe = node["current_recipe_run"].Value;
				www = new WWW(currentRecipe);
				yield return www;
				JSONNode recipeNode = JSON.Parse (www.text);
				www = new WWW(recipeNode["recipe"].Value);
				yield return www;
				recipeNode = JSON.Parse (www.text);
				currentRecipeDisplay.text = recipeNode["name"].Value;
			}
			else
			{
				currentRecipeDisplay.text = "None";
			}

			changeRecipeButton.interactable = true;
		}
		else
		{
			currentRecipeDisplay.text = "Selection is not a tray";
		}

		yield return null;
	}

	public void CreateOverrideModule(FarmActuator act)
	{
		GameObject module = Instantiate (actuatorOverridePrefab) as GameObject;
		module.transform.SetParent (overrideScrollContent, false);
		//print ("Created " + act.ToString ());

		ActuatorOverrideModule script = module.GetComponent<ActuatorOverrideModule> ();
		script.myActuator = act;
		script.myResource = act.myResource;
		script.StartCoroutine ("Initialize");

	}

	public void Reactivate()
	{
		transform.gameObject.SetActive (true);
	}

	public void CloseButtonPress()
	{
		ActionButtonManager.actionButtonManager.ModuleEnd ();
		Destroy (transform.gameObject);
	}

	public void ChangeRecipeButtonPress()
	{
		GameObject module = Instantiate (chooseRecipeModulePrefab) as GameObject;
		module.transform.SetParent (GameObject.FindGameObjectWithTag ("GUICanvas").transform, false);

		ChooseRecipeModule script = module.GetComponent<ChooseRecipeModule> ();
		script.toReactivate.Add (this);
		script.baseURL = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainSystemViewManager>().BaseURL;
		script.activeTrayObject = activeTrayObject;
		script.currentRecipeURL = currentRecipe;
		script.StartCoroutine ("Initialize");

		transform.gameObject.SetActive (false);
	}
}
