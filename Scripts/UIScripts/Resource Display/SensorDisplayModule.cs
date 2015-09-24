using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class SensorDisplayModule : MonoBehaviour {

	public FarmResource myResource;
	public string resourceType;
	public string resourceProperty;
	public FarmSensor mySensor;
	public string sensorType;
	public string url; // This is the url for the sensing point
	public string urlDataPoint;


	// GUI Elements //
	public Text valueDisplay;
	public Text nameDisplay;
	public GameObject iconDisplay;
	public GameObject tryAgainButton;
	//public Sprite movementDisplay;
	private bool isLive = true;

	private int sensorMovement;
	private SensorDisplayManager manager;
	public float refreshTime = 1.5f;

	private GameObject sensedObject;


	void Start()
	{
		manager = GameObject.FindGameObjectWithTag ("SensorDisplayManager").GetComponent<SensorDisplayManager> ();
	}
	/**
	 * Requires that sensorURL has been set
	 * Initializes the sensor display module
	 * Starts the value updating function (coroutine);
	 */
	public IEnumerator Initialize (string URL)
	{
		// Parse URL
		WWW www = new WWW (URL);
		yield return www;
		url = URL;
		JSONClass node = (JSONClass)JSON.Parse (www.text);

		www = new WWW (node["property"]);
		yield return www;
		JSONNode propertyNode = JSON.Parse (www.text);

		resourceProperty = propertyNode ["name"];
		resourceType = myResource.resourceType;
		sensorType = mySensor.sensorType;
		urlDataPoint = url + "value/";

		// Set Base values (name, icon, refreshTime)
		SetIcon ();
		SetName ();

		// Starts UpdateSensorValue coroutine
		StartCoroutine ("UpdateSensorValue");

		yield return null;
	}

	IEnumerator UpdateSensorValue () 
	{
		WWW www;
		JSONNode dataPoint = null;
		while (isLive)
		{
			int now = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds - 1;
			www = new WWW(urlDataPoint);
			yield return www;

			dataPoint = JSON.Parse (www.text);
			

			if(dataPoint["value"] == null)
			{
				SetValue("ERR: Cannot read sensor!");
				isLive = false;
				tryAgainButton.SetActive(true);
			}
			else
			{
				SetValue (dataPoint["value"]);
			}

			yield return new WaitForSeconds(refreshTime);
		}
	}

	void SetIcon()
	{
		Color imageColor = iconDisplay.GetComponent<Image> ().color;
		if(string.Compare(resourceType, "water", true)==0)
		{
			iconDisplay.GetComponent<Image>().sprite = manager.waterIcon;
			imageColor.a = 1;
		}
		else if (string.Compare(resourceType, "air", true)==0)
		{
			iconDisplay.GetComponent<Image>().sprite = manager.airIcon;
			imageColor.a = 1;
		}
		else if (string.Compare(resourceType, "light", true)==0)
		{
			iconDisplay.GetComponent<Image>().sprite = manager.lightIcon;
			imageColor.a = 1;
		}
		iconDisplay.GetComponent<Image> ().color = imageColor;

	}
	void SetName()
	{
		nameDisplay.text = resourceProperty;
	}
	void SetValue(string value)
	{
		valueDisplay.text = value;

	}

	public void TryAgainButton()
	{
		StopCoroutine ("UpdateSensorValue");
		isLive = true;
		tryAgainButton.SetActive(false);
		StartCoroutine ("UpdateSensorValue");
	}
	 
}
