using UnityEngine;
using System.Collections;

public class FarmSensorManager : MonoBehaviour {

	public GameObject sensorPrefab;

	public IEnumerator CreateSensor(object[] parms)//string URL, FarmResource resource)
	{
		string URL = ((string)parms [0]).Replace(System.Environment.NewLine, "");
		FarmResource resource = (FarmResource)parms [1];

		GameObject sensor = Instantiate (sensorPrefab) as GameObject;
		FarmSensor farmSensor = sensor.GetComponent<FarmSensor> ();

		sensor.transform.SetParent (transform);

		resource.sensorsList.Add (farmSensor);
		farmSensor.myResource = resource;

		yield return farmSensor.StartCoroutine ("Initialize", URL);
		yield return null;
	}
}
