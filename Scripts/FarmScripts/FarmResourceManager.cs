using UnityEngine;
using System.Collections;

public class FarmResourceManager : MonoBehaviour {

	public GameObject resourcePrefab;

	public void CreateResource(string url, FarmObject origin)
	{
		GameObject resource = Instantiate (resourcePrefab) as GameObject;
		FarmResource script = resource.GetComponent<FarmResource> ();
		
		resource.transform.SetParent (transform);

		origin.resourceList.Add (script);

		script.origin = origin;
		script.StartCoroutine ("Initialize", url);
	}
}
