using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class ResourceDisplayModule : MonoBehaviour {

	public GameObject sensingPointModulePrefab;
	public Text nameDisplay, locationDisplay;
	public Button iconDisplayButton;
	public Image icon;
	public Transform sensorListContent, sensorListOutter;
	public LayoutElement layoutElement;

	[HideInInspector] public SensorDisplayManager sensorDisplayManager;
	[HideInInspector] public FarmResource resource;
	[HideInInspector] public FarmObject farmObject;
	[HideInInspector] public List<FarmSensingPoint> sensingPoints = new List<FarmSensingPoint>();
	[HideInInspector] public string name, location;
	[HideInInspector] public bool expanded = true;

	private float baseHeight = 80.0f;
	private List<SensingPointModule> sensingPointModules = new List<SensingPointModule> ();

	public IEnumerator Initialize(FarmResource re)
	{

		resource = re;
		farmObject = resource.origin;
		// Set name and location
		name = resource.resourceType;
		location = farmObject.name;
		nameDisplay.text = name;
		locationDisplay.text = location;
		//print (resource.resourceType);
		icon.sprite = sensorDisplayManager.icons [resource.resourceType.ToLower()];
		// Create all sensing points
		foreach (FarmSensor sensor in resource.sensorsList) 
		{
			foreach (FarmSensingPoint point in sensor.farmSensingPoints)
			{
				yield return StartCoroutine("AddSensingPoint", point);
			}
		}
		transform.localScale = new Vector3 (1,1,1);

		yield return StartCoroutine ("UpdateSize");

		yield return null;
	}

	public IEnumerator AddSensingPoint(FarmSensingPoint point)
	{
		if (!point.activePoint) 
		{
			yield break;
		}

		GameObject module = Instantiate (sensingPointModulePrefab) as GameObject;
		// Set parent to GUICanvas item
		module.transform.SetParent (sensorListContent);

		// Call get values
		sensingPoints.Add (point);

		SensingPointModule script = module.GetComponent<SensingPointModule> ();
		script.sensingpoint = point;

		sensingPointModules.Add (script);

		script.StartCoroutine ("Initialize");

		yield return null;
	}

	public void RestartSensors()
	{
		foreach (SensingPointModule point in sensingPointModules) 
		{
			point.StopCoroutine("GetReadings");

			if(expanded)
			{
				point.StartCoroutine("GetReadings");
			}
		}
	}

	public IEnumerator UpdateSize()
	{


		float iconDim = iconDisplayButton.GetComponent<RectTransform> ().rect.width;
		float spContentHeight = sensorListContent.GetComponent<RectTransform>().rect.height;

		float newHeight, newWidth;
		if (expanded) 
		{
			newWidth = iconDim + 120.0f + 10f;
			newHeight = iconDim + spContentHeight + 10.0f;
		}
		else
		{
			newWidth = iconDim + 10.0f;
			newHeight = iconDim + 10.0f;
		}


		layoutElement.minWidth = newWidth;
		layoutElement.minHeight = newHeight;
		yield return null;
	}

	public void IconButtonPress()
	{
		expanded = !expanded;

		sensorListOutter.gameObject.SetActive (expanded);
		nameDisplay.gameObject.SetActive (expanded);
		locationDisplay.gameObject.SetActive (expanded);

		RestartSensors ();

		StopCoroutine ("UpdateSize");
		StartCoroutine ("UpdateSize");


	}
}
