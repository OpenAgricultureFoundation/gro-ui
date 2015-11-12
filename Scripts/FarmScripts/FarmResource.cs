using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class FarmResource : MonoBehaviour {

	public GameObject SensorPrefab;

	public List<FarmSensor> sensorsList;
	public List<FarmActuator> actuatorList;
	public FarmObject origin;
	public string resourceType;
	public string url;
	public string name;

	private JSONNode node;


	public IEnumerator Initialize(string URL)
	{
		WWW www = new WWW (URL);
		yield return www;
		url = URL;


		node = JSON.Parse (www.text);

		transform.name = node["name"];
		string typeURL = node ["resource_type"].Value.Replace(System.Environment.NewLine, "");
		www = new WWW (typeURL);
		yield return www;

		JSONNode typeNode = JSON.Parse (www.text);
		resourceType = typeNode ["name"];
		if (string.Compare( resourceType, "general", true) == 0)
		{
			Destroy(gameObject);
			yield break;
		}

		JSONArray sensors = node ["sensors"].AsArray;
		JSONArray actuators = node ["actuators"].AsArray;
		FarmSensorManager sensorManager = GameObject.FindGameObjectWithTag ("FarmSensorManager").GetComponent<FarmSensorManager> ();
		FarmActuatorManager actuatorManager = GameObject.FindGameObjectWithTag ("FarmActuatorManager").GetComponent<FarmActuatorManager> ();

		foreach(JSONNode sensor in sensors)
		{
			object[] multiParams = new object[2] {sensor.Value, this};
			yield return sensorManager.StartCoroutine("CreateSensor", multiParams);
		}
		foreach(JSONNode act in actuators)
		{
			object[] multiParams = new object[2] {act.Value, this};
			yield return actuatorManager.StartCoroutine("CreateActuator", multiParams);
		}

		yield return StartCoroutine ("CreateDisplayModule");

		yield return null;
	}

	public IEnumerator LoadSensor(JSONNode sensor)
	{
		// foreach Sensor
			// Create sensor
			// yield return InitializeSensor
		yield return null;
	}

	public IEnumerator LoadActuator(JSONNode actuator)
	{
		//foreach Actuator
			// Create Actuator
			// yield return InitializeActuator
		yield return null;
	}

	public IEnumerator CreateDisplayModule()
	{
		yield return GameObject.FindGameObjectWithTag ("SensorDisplayManager").GetComponent<SensorDisplayManager> ().StartCoroutine ("CreateResourceDisplayModule", this);

		yield return null;
	}

	public IEnumerator AddSensor()
	{
		yield return null;
	}

	public IEnumerator AddActuator()
	{
		yield return null;
	}

}
