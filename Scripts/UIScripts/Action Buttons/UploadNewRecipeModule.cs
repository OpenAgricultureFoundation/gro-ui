using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class UploadNewRecipeModule : MonoBehaviour, ISelectionReceiver<PlantTypeChoice> {

	public GameObject plantTypeChoicePrefab;
	public InputField recipeNameInput;
	public Button uploadButton;
	public Text uploadFilePathDisplay;

	[HideInInspector] public List<IReactivatable> toReactivate = new List<IReactivatable>();
	[HideInInspector] public string baseURL, recipeURL;

	private Dictionary<string, JSONNode> plantTypeDict = new Dictionary<string, JSONNode> ();
	private List<PlantTypeChoice> selectedPlantTypes = new List<PlantTypeChoice> ();
	private Transform plantTypeChoiceScrollContent;
	private const string plantTypeListURLSuffix = "plantType/";
	private string plantTypeListURL, uploadFilePath, inputRecipeName;


	public IEnumerator Initialize()
	{
		// Set plantTypeListURL
		plantTypeListURL = string.Concat (baseURL, plantTypeListURLSuffix);
		// Get Scroll Content
		plantTypeChoiceScrollContent = transform.FindChild ("PlantTypeScrollPanel").FindChild ("PlantTypeScrollView").FindChild ("PlantTypeScrollContent");
		// Get + populate plantType choices
		StartCoroutine ("GetPlantTypes");

		yield return null;
	}

	IEnumerator GetPlantTypes()
	{
		WWW www = new WWW (plantTypeListURL);
		yield return www;
		if (!string.IsNullOrEmpty (www.error)) 
		{
			print (www.error);
		}
		JSONNode node = JSON.Parse (www.text);
		JSONArray plantTypeList = node ["results"].AsArray;

		foreach (JSONNode plantType in plantTypeList) 
		{
			if(!plantTypeDict.ContainsKey (plantType["url"].Value))
			{
				plantTypeDict.Add (plantType["url"].Value, plantType);
				CreatePlantTypeChoice (plantType);

			}
		}

		yield return null;
	}
	public void MakeSelection(PlantTypeChoice choice)
	{
		if (choice.selected) 
		{
			selectedPlantTypes.Add (choice);
		} 
		else 
		{
			selectedPlantTypes.Remove (choice);
		}
		ValidateInputs ();
	}

	void CreatePlantTypeChoice(JSONNode plantType)
	{
		GameObject choice = Instantiate (plantTypeChoicePrefab) as GameObject;
		PlantTypeChoice script = choice.GetComponent<PlantTypeChoice> ();
		script.SetCommonName(plantType["common_name"].Value);
		script.SetLatinName (plantType ["latin_name"].Value);
		script.selectionReceiver = this;

		choice.transform.SetParent (plantTypeChoiceScrollContent, false);
	}

	void OpenFileFunction(string path)
	{
		uploadFilePath = path;
		string shortPath = "";
		string[] split = path.Split ("/" [0]);
		for (int i=0; i<1; i++) 
		{
			if(split.Length > i)
			{
				//shortPath = "/" + split[split.Length-i-1] + shortPath;
				shortPath = split[split.Length-1-i];
			}
		}
		uploadFilePathDisplay.text = shortPath;
		ValidateInputs ();
	}

	public void ChooseFileButtonPress()
	{
		UniFileBrowser.use.OpenFileWindow (OpenFileFunction);
	}

	void ReactivateObjects()
	{
		foreach (IReactivatable item in toReactivate) 
		{
			item.Reactivate();
		}
	}

	public void UpdateName(string name)
	{
		inputRecipeName = name;
		ValidateInputs ();
	}

	public void ValidateInputs()
	{
		if (!(recipeNameInput.text.Length > 0)) 
		{
			uploadButton.interactable = false;
			return;
		}
		if (string.IsNullOrEmpty(uploadFilePath))
		{
			uploadButton.interactable = false;
			return;
		}
		/*
		if (selectedPlantTypes.Count == 0) 
		{
			uploadButton.interactable = false;
			return;
		}
		*/
		uploadButton.interactable = true;

	}
	IEnumerator UploadRecipe()
	{
		WWWForm upload = new WWWForm ();

		// Add file to upload form
		WWW localFile = new WWW ("file:///" + uploadFilePath);
		yield return localFile;
		if (!string.IsNullOrEmpty (localFile.error)) 
		{
			Debug.Log (localFile.error);
		}
		
		byte[] fileData = localFile.bytes;
		upload.AddBinaryData ("file", fileData);

		// Add plant Types to upload form
		string[] plantTypeURLs = new string[plantTypeDict.Count];
		if (plantTypeDict.Count > 0) 
		{
			foreach(PlantTypeChoice plantType in selectedPlantTypes)
			{
				string typeURL = plantType.url;
				upload.AddField("plant_type", typeURL);
			}
		}

		// Add name to upload form
		upload.AddField ("name", inputRecipeName);

		object[] parms = new object[2] { recipeURL, upload };
		yield return DataManager.dataManager.StartCoroutine("PostRequest", parms);
		
		/*
		WWW www = new WWW (recipeURL, rawData, headers);
		yield return www;
		if (!string.IsNullOrEmpty (www.error)) 
		{
			Debug.Log (www.error);
			Debug.Log (www.text);
			uploadButton.interactable = true;
			yield break;
		}
		print (www.text);
		*/
		
		EndModule ();

		yield return null;
	}

	public void UploadButtonPress()
	{
		StopCoroutine ("UploadRecipe");
		StartCoroutine ("UploadRecipe");
		uploadButton.interactable = false;
	}

	public void CloseButtonPress()
	{
		EndModule ();
	}

	void EndModule()
	{
		ReactivateObjects ();
		Destroy (transform.gameObject);
	}
}
