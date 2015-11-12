using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class FarmActuatorManager : MonoBehaviour {

	public GameObject actuatorPrefab;
	public List<FarmActuator> actuatorList = new List<FarmActuator>();

	public IEnumerator CreateActuator(object[] parms)//string URL, FarmResource resource)
	{
		string URL = ((string)parms [0]).Replace(System.Environment.NewLine, "");;
		FarmResource resource = (FarmResource)parms [1];
		GameObject act = Instantiate (actuatorPrefab) as GameObject;
		FarmActuator farmAct = act.GetComponent<FarmActuator> ();
		
		act.transform.SetParent (transform);
		
		actuatorList.Add (farmAct);
		farmAct.myResource = resource;
		
		yield return farmAct.StartCoroutine ("Initialize", URL);
		yield return null;
	}


}
