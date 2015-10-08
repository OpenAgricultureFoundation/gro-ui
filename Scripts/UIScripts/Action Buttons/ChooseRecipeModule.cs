using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ChooseRecipeModule : MonoBehaviour, IReactivatable, ISelectionReceiver<RecipeChoice> {

	public GameObject recipeChoicePrefab, UploadNewRecipeModulePrefab;
	public GameObject overridePrompt;
	public Button selectButton, uploadNewButton, closeButton;

	[HideInInspector] public string baseURL, recipeListURL, recipeRunURL; 
	[HideInInspector] public string currentRecipeURL;//, currentRecipeName;
	[HideInInspector] public List<IReactivatable> toReactivate = new List<IReactivatable>();
	[HideInInspector] public FarmTray activeTrayObject;

	private Transform recipeChoiceScrollContent;
	private Dictionary<string, JSONNode> recipeDict = new Dictionary<string, JSONNode>();
	private const string RecipeURLSuffix = "recipe/", RecipeRunURLSuffix = "recipeRun/";
	private RecipeChoice selectedRecipe;

	public IEnumerator Initialize()
	{
		// Set RecipeListURL
		recipeListURL = string.Concat (baseURL, RecipeURLSuffix);
		recipeRunURL = string.Concat (baseURL, RecipeRunURLSuffix);
		// Set Scroll Content transform
		recipeChoiceScrollContent = transform.FindChild ("RecipeChoiceScrollPanel").FindChild ("RecipeScrollView").FindChild ("RecipeScrollContent");
		// Call Get Recipes
		StartCoroutine ("GetRecipes");

		yield return null;
	}

	IEnumerator GetRecipes()
	{
		WWW www = new WWW (recipeListURL);
		yield return www;
		JSONClass node = (JSONClass)JSON.Parse (www.text);

		JSONArray recipes = node ["results"].AsArray;

		foreach (JSONNode recipe in recipes) 
		{
			if(!recipeDict.ContainsKey(recipe["url"].Value))
			{
				recipeDict.Add (recipe["url"].Value, recipe);
				CreateRecipeChoice(recipe);
			}
		}

		yield return null;
	}

	void CreateRecipeChoice(JSONNode recipeChoice)
	{
		GameObject choice = Instantiate (recipeChoicePrefab) as GameObject;

		choice.GetComponentInChildren<Toggle>().group = GetComponent<ToggleGroup>();

		RecipeChoice script = choice.GetComponent<RecipeChoice> ();
		script.recipe = recipeChoice;
		script.selectionReceiver = this;

		script.StartCoroutine ("Initialize");

		choice.transform.SetParent (recipeChoiceScrollContent, false);
	}

	public void CloseButtonPress()
	{
		EndModule ();
	}

	public void Refresh()
	{
		StopCoroutine ("GetRecipes");
		StartCoroutine ("GetRecipes");
	}

	public void Reactivate()
	{
		transform.gameObject.SetActive (true);
		Refresh ();
	}

	public void MakeSelection (RecipeChoice choice)
	{
		selectedRecipe = choice;
		selectButton.interactable = true;
	}

	public void StartAddNewRecipeModule()
	{
		GameObject module = Instantiate (UploadNewRecipeModulePrefab) as GameObject;
		module.transform.SetParent (GameObject.FindGameObjectWithTag ("GUICanvas").transform, false);

		UploadNewRecipeModule script = module.GetComponent<UploadNewRecipeModule> ();
		script.baseURL = baseURL;
		script.toReactivate.Add (this);
		script.recipeURL = recipeListURL;

		script.StartCoroutine ("Initialize");

		transform.gameObject.SetActive (false);
	}

	public void SelectButtonPress()
	{
		
		selectButton.interactable = false;
		if (currentRecipeURL != null) 
		{
			closeButton.interactable = false;
			uploadNewButton.interactable = false;

			StartOverridePrompt();
			return;
		}

		StartCoroutine("RunRecipe");
	}

	public void StartOverridePrompt()
	{
		overridePrompt.SetActive (true);
	}

	public void OverrideConfirmButtonPress()
	{
		StartCoroutine ("OverrideThenRunNew");
	}

	public void OverrideCancelButtonPress()
	{
		closeButton.interactable = true;
		uploadNewButton.interactable = true;
		selectButton.interactable = true;
		overridePrompt.SetActive (false);
	}

	IEnumerator OverrideThenRunNew()
	{
		yield return StartCoroutine ("EndCurrentRecipe");
		StartCoroutine ("RunRecipe");
		yield return null;
	}

	IEnumerator EndCurrentRecipe()
	{
		int nowTimestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds - 1;

		WWWForm form = new WWWForm ();
		form.AddField ("end_timestamp", nowTimestamp);
		form.AddField ("tray", activeTrayObject.node["url"].Value);
		//form.AddField ("recipe", a);

		object[] parms = new object[2] { currentRecipeURL, form };

		yield return DataManager.dataManager.StartCoroutine("PutRequest", parms);

		//Dictionary<string, string> headers = new Dictionary<string, string> ();
		/*
		Dictionary<string, string> headers = form.headers;
		headers.Add ("X-HTTP-Method-Override", "PUT");

		byte[] formData = form.data;

		WWW www = new WWW (currentRecipeURL, formData, headers);
		yield return www;
		if (!string.IsNullOrEmpty (www.error)) 
		{
			Debug.Log (www.error);
			Debug.Log (www.text);
		}
		*/
		yield return null;
	}

	IEnumerator RunRecipe()
	{
		// Run RecipeChoice
		int nowTimestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds+15;

		WWWForm form = new WWWForm ();
		form.AddField ("recipe", selectedRecipe.url);
		form.AddField ("tray", activeTrayObject.transform.GetComponent<FarmObject>().myURL);
		form.AddField ("start_timestamp", nowTimestamp);
		
		object[] parms = new object[2] { recipeRunURL, form };

		yield return DataManager.dataManager.StartCoroutine("PostRequest", parms);
		/*
		WWW post = new WWW (recipeRunURL, form);
		yield return post;
		if (!string.IsNullOrEmpty (post.error)) 
		{
			selectButton.interactable = true;
			print (post.error);
			print (post.text);
			yield break;
		}
		*/

		EndModule ();
	}

	void ReactivateObjects()
	{
		foreach (IReactivatable item in toReactivate) 
		{
			item.Reactivate();
		}
	}

	void EndModule()
	{
		ReactivateObjects ();
		Destroy (transform.gameObject);
	}
}
