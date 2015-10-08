using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class DownloadDataModule : MonoBehaviour, ISelectionReceiver<SensingPointChoice> {

	public GameObject sensingPointChoicePrefab;
	public Transform sensingPointChoiceScrollPanel;
	public InputField startDateInput, endDateInput;
	private string saveFilePath;

	private List<SensingPointChoice> selectedSensingPoints = new List<SensingPointChoice>();

	public IEnumerator Initialize()
	{
		StartCoroutine("GetSensingPoints");
		yield return null;	
	}
	
	public IEnumerator GetSensingPoints() 
	{
		/* 
		 * Base URL + Sensing Points
		 * For each Sensing Point
		 * Create a sensing point choice
		 */
		string sensingpointURL = DataManager.dataManager.ipAddress + "/sensingPoint/";
		WWW www = new WWW(sensingpointURL);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.Log (www.error);
			Debug.Log (www.text);
		}
		JSONNode sensingPointNode = JSONNode.Parse(www.text);
		foreach (JSONNode point in sensingPointNode["results"].AsArray)
		{
			//Debug.Log (point["url"]);
			www = new WWW(point["property"].Value);
			yield return www;
			point["property"] = JSON.Parse(www.text);
			CreateSensingPointChoice(point);
		}
		
		yield return null;
	}
	
	public void CreateSensingPointChoice(JSONNode sensingPoint)
	{
		if(!sensingPoint["is_active"].AsBool)
		{
			return;
		}
		//Debug.Log (sensingPoint["property"]["name"].Value);
		GameObject choice = Instantiate(sensingPointChoicePrefab) as GameObject;
		choice.transform.SetParent(sensingPointChoiceScrollPanel, false);
		
		SensingPointChoice script = choice.GetComponent<SensingPointChoice>();
		script.selectionReceiver = this;
		script.SetName(sensingPoint["property"]["name"].Value);
		
	}
	
	public void MakeSelection(SensingPointChoice choice)
	{
		if(choice.selected){
			selectedSensingPoints.Add(choice);
		}
		else{
			selectedSensingPoints.Remove(choice);
		}
	}
	
	public bool ValidateInputs()
	{
		if(! (selectedSensingPoints.Count > 0))
		{
			return false;
		}
		System.DateTime dt1, dt2;
		bool isValid1 = System.DateTime.TryParseExact(startDateInput.text, 
				"MM/dd/yyyy", 
				System.Globalization.CultureInfo.InvariantCulture, 
				System.Globalization.DateTimeStyles.None, 
				out dt1);
		bool isValid2 = System.DateTime.TryParseExact(endDateInput.text, 
				"MM/dd/yyyy", 
				System.Globalization.CultureInfo.InvariantCulture, 
				System.Globalization.DateTimeStyles.None, 
				out dt2);
		if(!(isValid1 && isValid2))
		{
			return false;
		}
		return true;
	}
	
	public void DownloadButtonPress() 
	{
		if(ValidateInputs())
		{
			StartDownloadSequence();
		}
	}
	
	public void StartDownloadSequence()
	{
		UniFileBrowser.use.SaveFileWindow (SaveFileFunction);
	}
	
	void SaveFileFunction(string path)
	{
		saveFilePath = path;
		Debug.Log (saveFilePath);
	}
	
	void GetData()
	{
		
	}
	
	void SaveDataFile(string data)
	{
		
	}
	
	private string FormatDataToCSV(string data)
	{
		return "";
	}
	public void CloseButtonPress() 
	{
		EndModule();
	}
	
	public void EndModule() 
	{
		ActionButtonManager.actionButtonManager.ModuleEnd ();
		Destroy (transform.gameObject);
	}
	
}
