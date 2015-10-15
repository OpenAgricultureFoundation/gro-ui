using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class SensingPointChoice : MonoBehaviour {

	public Text sensingPointName;
	public ISelectionReceiver<SensingPointChoice> selectionReceiver;
	public bool selected = false;
	public JSONNode node;
	
	public void SetName(string name)
	{
		sensingPointName.text = name;
	}
	
	public void ToggleChoice(bool toggle)
	{
		selected = toggle;
		selectionReceiver.MakeSelection(this);
	}
}
