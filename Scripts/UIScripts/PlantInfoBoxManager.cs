using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PlantInfoBoxManager : MonoBehaviour {

	[HideInInspector] public static PlantInfoBoxManager plantInfoBox;

	public GameObject infoEntryPrefab;
	public Text commonNameDisplay;

	public bool hasActivePlant = false;

	private List<PlantInfoEntry> allInfoEntries = new List<PlantInfoEntry>();
	private const int MaxInfoEntries = 6;

	void Awake()
	{
		if (plantInfoBox == null) 
		{
			plantInfoBox = this;
		}
	}

	public void FixedUpdate()
	{
		if (hasActivePlant) 
		{
			gameObject.SetActive(true);
		} 
		else 
		{
			gameObject.SetActive(false);
		}
	}

	public void AddInfo (string label, string value, int style = 0)
	{
		if (allInfoEntries.Count >= MaxInfoEntries) 
		{
			return;
		}

		GameObject entry = Instantiate (infoEntryPrefab) as GameObject;
		PlantInfoEntry script = entry.GetComponent<PlantInfoEntry> ();

		entry.transform.SetParent (transform);
		entry.transform.localScale = new Vector3 (1,1,1);
		script.SetLabel (label);
		script.SetValue (value, style);

		allInfoEntries.Add (script);
	}

	public void AddInfo (string label, float value, int style = 0)
	{
		string valueAsString = value.ToString ();
		AddInfo (label, valueAsString, style);
	}

	public void SetCommonName (string commonName)
	{
		commonNameDisplay.text = commonName;
	}


	public void ClearActivePlant()
	{
		hasActivePlant = false;
		int count = allInfoEntries.Count;
		if (count <= 0) 
		{
			return;
		}

		for (int i = 0; i < count; i++) 
		{
			Destroy(allInfoEntries[0].gameObject);
			allInfoEntries.Remove(allInfoEntries[0]);
		}

	}
}
