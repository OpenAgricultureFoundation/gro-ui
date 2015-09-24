using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SensorDisplayManager : MonoBehaviour {

	public GameObject sensorModulePrefab, resourceDisplayModulePrefab;
	public float standardRefreshTime;
	public List<SensorDisplayModule> sensorModules;
	public Transform sensorBarScrollContent;
	//List of all sensor Modules instantiated

	public Dictionary<string, Sprite> icons = new Dictionary<string, Sprite> ();

	public Sprite airIcon;
	public Sprite waterIcon;
	public Sprite lightIcon;

	public void Start()
	{
		icons.Add ("air", airIcon);
		icons.Add ("water", waterIcon);
		icons.Add ("light", lightIcon);
	}

	public void CreateModule (string url, FarmSensor sensor)
	{
		// Instantiate Module prefab
		GameObject module = Instantiate (sensorModulePrefab) as GameObject;
		module.transform.SetParent (sensorBarScrollContent);

		SensorDisplayModule script = module.GetComponent<SensorDisplayModule> ();
		script.mySensor = sensor;
		script.myResource = sensor.myResource;
		script.StartCoroutine ("Initialize", url);
		sensorModules.Add (script);
		sensor.sensingPoints.Add (script);
		// Set module URL
		// Call initialize function
		// Add to list of sensor modules
	}

	public void Initializer ()
	{
		// Query for all sensors
		// Create a module for each sensor
	}

	public IEnumerator CreateResourceDisplayModule(FarmResource resource)
	{
		GameObject module = Instantiate (resourceDisplayModulePrefab) as GameObject;
		module.transform.SetParent(sensorBarScrollContent);

		ResourceDisplayModule script = module.GetComponent<ResourceDisplayModule>();
		script.sensorDisplayManager = this;
		yield return script.StartCoroutine("Initialize", resource);

		yield return null;
	}
}
