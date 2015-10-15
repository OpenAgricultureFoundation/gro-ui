using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class SensingPointModule : MonoBehaviour {

	[SerializeField] private static float _refreshTime = 4.20f;
	private static int _thresholdForOldValues = 15*60;

	public Text resourcePropertyDisplay, valueDisplay;

	[HideInInspector] public FarmSensingPoint sensingpoint;
	[HideInInspector] public bool isLive = true;

	private string urlDataPoint;

	public IEnumerator Initialize()
	{
		resourcePropertyDisplay.text = sensingpoint.property;
		urlDataPoint = sensingpoint.urlDataPoint;
		StartCoroutine ("GetReadings");

		yield return null;
	}

	public void SetSensorReadingValue(string value)
	{
		valueDisplay.text = value;
	}

	public IEnumerator GetReadings()
	{
		WWW www;
		JSONNode dataPoint = null;
		while (isLive)
		{
			int now = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds - 1;
			www = new WWW(urlDataPoint);
			yield return www;
			
			if(!string.IsNullOrEmpty(www.error))
			{
				Debug.Log (www.error);
				Debug.Log (www.text);
				SetSensorReadingValue("ERROR");
				isLive = false;
				//SetSensorReadingValue (dataPoint["value"].Value);
			}
			/*
			else if(dataPoint["value"].Value == "null")
			{
				SetSensorReadingValue("ERR: Cannot read sensor!");
				isLive = false;
				//tryAgainButton.SetActive(true);
			}
			*/
			else
			{
				/*
				if(now - timestamp > threshold)
				*/
				dataPoint = JSON.Parse (www.text);
				SetSensorReadingValue (dataPoint["value"].Value);
			}
			
			yield return new WaitForSeconds(_refreshTime);
		}
		yield return null;
	}
}
