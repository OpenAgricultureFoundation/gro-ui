using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class DownloadDataModule : MonoBehaviour, ISelectionReceiver<SensingPointChoice> {

	public GameObject sensingPointChoicePrefab;
	public Transform sensingPointChoiceScrollPanel;
	public InputField startDateInput, endDateInput, startTimeInput, endTimeInput;
	private string saveFilePath, folderName;
	private System.DateTime startTime, endTime;
	

	private List<SensingPointChoice> selectedSensingPoints = new List<SensingPointChoice>();
	private Dictionary<SensingPointChoice, Dictionary<string, string>> downloadedData = new Dictionary<SensingPointChoice, Dictionary<string, string>>(); 


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
			www = new WWW(point["property"]["resource_type"]);
			yield return www;
			point["property"]["resource_type"] = JSON.Parse(www.text);
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
		script.node = sensingPoint;
		string name = sensingPoint["property"]["name"].Value
			+ " - "
			+ sensingPoint["property"]["resource_type"]["name"];
		script.SetName(name);
		
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
		bool isValid1 = System.DateTime.TryParseExact(startDateInput.text, 
				"MM/dd/yyyy", 
				System.Globalization.CultureInfo.InvariantCulture, 
				System.Globalization.DateTimeStyles.None, 
				out startTime);
		bool isValid2 = System.DateTime.TryParseExact(endDateInput.text, 
				"MM/dd/yyyy", 
				System.Globalization.CultureInfo.InvariantCulture, 
				System.Globalization.DateTimeStyles.None, 
				out endTime);
		
		if(!(isValid1 && isValid2))
		{
			return false;
		}
		if(!string.IsNullOrEmpty(startTimeInput.text))
		{
			System.TimeSpan time = System.TimeSpan.Parse(startTimeInput.text);
			startTime = startTime.Add(time);
			//Debug.Log(startTime);
		}
		if(!(string.IsNullOrEmpty(endTimeInput.text)))
		{
			System.TimeSpan time = System.TimeSpan.Parse(endTimeInput.text);
			endTime = endTime.Add(time);
		}
		
		System.TimeSpan timezoneOffset = System.DateTime.UtcNow - System.DateTime.Now;
		startTime = startTime.Add(timezoneOffset);
		endTime = endTime.Add(timezoneOffset);
		
		return true;
	}
	
	public void DownloadButtonPress() 
	{
		if(ValidateInputs())
		{
			
			string defaultFolderName = "GroData from " 
										+ startTime.ToString("MMM d") 
										+ " to "
										+ endTime.ToString("MMM d");
			
			UniFileBrowser.use.defaultFileName = defaultFolderName;
			UniFileBrowser.use.SaveFileWindow (SaveFileFunction);
		}
	}
	
	public IEnumerator DownloadSequence()
	{
		foreach(SensingPointChoice choice in selectedSensingPoints)
		{
			Debug.Log("Getting data...");
			yield return StartCoroutine("GetData", choice);
		}
		foreach(SensingPointChoice sensingPoint in downloadedData.Keys)
		{
			// Convert to CSV
			Debug.Log("Converting to csv string...");
			string dataString = FormatDataToCSV(downloadedData[sensingPoint]);
			// Write File
			Debug.Log("Saving File...");
			string fileName = sensingPoint.sensingPointName.text + "_data.csv";
			SaveDataFile(dataString, fileName);
		}
		
		yield return null;
	}
	
	void SaveFileFunction(string path)
	{
		saveFilePath = path;
		Debug.Log (saveFilePath);
		// Create folder
		System.IO.Directory.CreateDirectory(path);
		StartCoroutine("DownloadSequence");
	}
	
	IEnumerator GetData(SensingPointChoice choice)
	{
		System.DateTime epoch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		int startTimeStamp = (int)(startTime-epoch).TotalSeconds;
		int endTimeStamp = (int)(endTime-epoch).TotalSeconds;
		string spURL = choice.node["url"].Value;
		string[] split = spURL.Split(new[] {'/'});
		string spID = split[split.Length-2];
		
		string firstURL = DataManager.dataManager.ipAddress 
								+ "/dataPoint/" 
								+ "?min_time=" + startTimeStamp.ToString()
								+ "&max_time=" + endTimeStamp.ToString()
								+ "&sensing_point=" + spID
								+ "&limit=200";
		
		WWW www = new WWW(firstURL);
		yield return www;
		if(!string.IsNullOrEmpty(www.error))
		{
			Debug.Log(www.error);
			Debug.Log(www.text);
		}
		bool next = true;
		Dictionary<string, string> sensingPointData = new Dictionary<string, string>();
		int pageCount = 1;
		int numPoints = 0;
		
		while (next) 
		{
			//Debug.Log("Page number " + pageCount);
			JSONNode node = JSON.Parse(www.text);
			JSONArray dataPoints = node["results"].AsArray;
			foreach(JSONNode point in dataPoints)
			{
				int time = point["timestamp"].AsInt;
				float value = point["value"].AsFloat;//.ToString();
				sensingPointData[time.ToString()] = value.ToString();
				numPoints+=1;
			}
			if(node["next"].Value == "null")
			{
				next = false;
			}
			else
			{
				www = new WWW(node["next"].Value);
				yield return www;
				if(!string.IsNullOrEmpty(www.error))
				{
					Debug.Log(www.error);
					Debug.Log(www.text);
				}
				pageCount++;
			}	
		}
		downloadedData[choice] = sensingPointData;
		Debug.Log ("Number of points: " + numPoints);
		
		yield return null;
	}
	
	void SaveDataFile(string data, string fileName)
	{
		string path = saveFilePath + "/" + fileName;
		System.IO.File.WriteAllText(path, data);
		downloadedData = new Dictionary<SensingPointChoice, Dictionary<string, string>>();
	}
	
	private string FormatDataToCSV(Dictionary<string, string> data)
	{
		string output = "Time, Value";
		foreach(KeyValuePair<string, string> kvp in data)
		{
			output = output + "\n" + kvp.Key + ", " + kvp.Value;
		}
		
		return output;
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
