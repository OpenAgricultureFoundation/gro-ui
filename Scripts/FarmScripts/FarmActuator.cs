using UnityEngine;
using System.Collections;
using SimpleJSON;

public class FarmActuator : MonoBehaviour {

	//public string sensorType;

	[HideInInspector] public string url, name, actTypeURL;
	[HideInInspector] public FarmResource myResource;
	
	private string stateUrlSuffix = "state";
	private JSONClass node;
	private JSONNode stateNode;

	public IEnumerator Initialize(string URL)
	{
		WWW www = new WWW (URL);
		yield return www;
		if (!string.IsNullOrEmpty (www.error)) 
		{
			Debug.Log (www.error);
			yield return null;
		}
		url = URL;
		node = (JSONClass)JSON.Parse (www.text);

		name = node ["name"].Value;
		transform.name = name;
		//actTypeURL = node[

		yield return null;
	}

	public IEnumerator GetCurrentState()
	{
		string stateURL = string.Concat (url, stateUrlSuffix);
		WWW www = new WWW (stateURL);
		yield return www;
		stateNode = JSON.Parse (www.text);
		if (!string.IsNullOrEmpty (www.error)) 
		{
			Debug.Log (www.error);
		}
		yield return null;
	}
}
