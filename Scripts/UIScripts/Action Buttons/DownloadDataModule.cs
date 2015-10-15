using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class DownloadDataModule : MonoBehaviour {

	public GameObject sensingPointChoicePrefab;
	public Transform sensingPointChoiceScrollPanel;

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
		SensingPointChoice script = choice.GetComponent<SensingPointChoice>();
		
		choice.transform.SetParent(sensingPointChoiceScrollPanel, false);
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
	
	public void DownloadButtonPress() 
	{
		
	}
	
	private void FormatDataToCSV(string data)
	{
		
	}
	
}
