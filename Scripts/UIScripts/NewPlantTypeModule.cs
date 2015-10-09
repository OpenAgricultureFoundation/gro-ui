using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class NewPlantTypeModule : MonoBehaviour {

	public InputField commonNameInputField, latinNameInputField;

	[HideInInspector] public AddPlantModule addPlantModule;
	[HideInInspector] public string plantTypeURL;

	private string commonName, latinName;
	private int minNameLength = 3;

	public IEnumerator CreatePlantType()
	{
		WWWForm form = new WWWForm ();
		form.AddField ("common_name", commonName);
		form.AddField ("latin_name", latinName);
		form.AddField ("plant_size", "short-leafy");
		//form.AddField ("model", "http://" + DataManager.dataManager.ipAddress + "/plantModel/1/");
		print (DataManager.dataManager.ipAddress);

		string url = string.Concat (plantTypeURL, "/");
		
		object[] parms = new object[2] { url, form };
		yield return DataManager.dataManager.StartCoroutine("PostRequest", parms);
		
		/*
		WWW www = new WWW (url, form);
		yield return www;
		if (!string.IsNullOrEmpty (www.error)) 
		{
			print (www.error);
			print (www.text);
		}
		*/
		
		addPlantModule.gameObject.SetActive (true);
		addPlantModule.StartCoroutine ("GetPlantTypes");
		EndModule ();
		yield return null;
	}

	public void CreateButtonPress()
	{
		commonName = commonNameInputField.text;
		latinName = latinNameInputField.text;

		if (commonName.Length < minNameLength || latinName.Length < minNameLength) 
		{
			// Entry is too short
			return;
		}

		StartCoroutine ("CreatePlantType");
	}

	public void CancelButtonPress()
	{
		addPlantModule.gameObject.SetActive (true);
		EndModule ();
	}

	public void EndModule()
	{
		Destroy (transform.gameObject);
	}
}
