using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlantInfoEntry : MonoBehaviour {

	public Text labelDisplay, valueDisplay;

	public void SetLabel(string label)
	{
		labelDisplay.text = label;
	}

	public void SetValue(string value, int style = 0)
	{
		valueDisplay.text = value;
		if (style == 1) {
			valueDisplay.fontStyle = FontStyle.Italic;
		}
	}
}
