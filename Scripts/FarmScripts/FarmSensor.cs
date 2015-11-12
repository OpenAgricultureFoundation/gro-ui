using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class FarmSensor : MonoBehaviour {

	public string sensorType;
	public List<SensorDisplayModule> sensingPoints;
	public List<FarmSensingPoint> farmSensingPoints = new List<FarmSensingPoint>();
	public FarmResource myResource;
	public string url;
	[HideInInspector] public FarmManager farmManager;

	public IEnumerator Initialize (string URL)
	{
		farmManager = GameObject.FindGameObjectWithTag ("FarmManager").GetComponent<FarmManager> ();
		url = URL.Replace(System.Environment.NewLine, "");
		WWW www = new WWW (url);
		yield return www;
		JSONClass node = (JSONClass)JSON.Parse (www.text);

		string typeURL = node ["sensor_type"].Value.Replace(System.Environment.NewLine, "");
		www = new WWW (typeURL);
		yield return www;
		
		JSONNode typeNode = JSON.Parse (www.text);
		sensorType = typeNode ["name"];

		JSONArray sensingPoints = node ["sensing_points"].AsArray;
		SensorDisplayManager manager = GameObject.FindGameObjectWithTag ("SensorDisplayManager").GetComponent<SensorDisplayManager> ();
		foreach(JSONNode point in sensingPoints)
		{
			//manager.CreateModule(point, this);
			yield return StartCoroutine("LoadSensingPoint", point);
		}

		yield return null;
	}

	public IEnumerator LoadSensingPoint(JSONNode sensingPoint)
	{
		// foreach SensingPoint
		GameObject obj = Instantiate (farmManager.sensingPointPrefab) as GameObject;
		obj.transform.SetParent (transform);
		FarmSensingPoint point = obj.GetComponent<FarmSensingPoint> ();
		point.sensor = this;
		point.resource = myResource;
		farmSensingPoints.Add (point);

		yield return point.StartCoroutine("Initialize", sensingPoint.Value);

		yield return null;
	}

}
