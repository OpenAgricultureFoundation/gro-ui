using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class PlantTypeChoice : MonoBehaviour {

	public Text commonNameDisplay;
	public Text latinNameDisplay;
	public string url;
	public Button removeButton;
	public JSONNode plantTypeNode;

	[HideInInspector] public ISelectionReceiver<PlantTypeChoice> selectionReceiver;
	[HideInInspector] public IRemover<PlantTypeChoice> remover;
	[HideInInspector] public bool selected;

	private string commonName;
	private string latinName;

	public string CommonName
	{
		get
		{
			return commonName;
		}
		set
		{
			commonName = value;
		}
	}


	public string LatinName
	{
		get
		{
			return latinName;
		}
		set
		{
			latinName = value;
		}
	}


	public void ToggleSet(bool val)
	{
		selected = val;

		selectionReceiver.MakeSelection(this);
		
	}

	public void SetCommonName(string name)
	{
		commonNameDisplay.text = name;
		commonName = name;
	}

	public void SetLatinName(string name)
	{
		latinNameDisplay.text = name;
		latinName = name;
	}

	public void RemoveButtonPress()
	{
		remover.Remove (this);
	}
}
