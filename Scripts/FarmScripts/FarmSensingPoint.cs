using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class FarmSensingPoint : MonoBehaviour {

	[HideInInspector] public FarmResource resource;
	[HideInInspector] public FarmSensor sensor;
	[HideInInspector] public string url, property, urlDataPoint, units;
	public bool activePoint = true;

	//public float refreshTime = 1.0f;

	public IEnumerator Initialize(string URL)
	{

		// Parse URL
		WWW www = new WWW (URL);
		yield return www;
		url = URL;
		JSONClass node = (JSONClass)JSON.Parse (www.text);
		
		www = new WWW (node["property"]);
		yield return www;
		JSONNode propertyNode = JSON.Parse (www.text);
		
		property = propertyNode ["name"];
		units = propertyNode ["units"];
		//type = myResource.resourceType;
		//sensorType = mySensor.sensorType;
		urlDataPoint = url + "value/";
		activePoint = node ["is_active"].AsBool;
		yield return null;
	}
}
