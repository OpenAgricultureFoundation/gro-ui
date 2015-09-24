using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActuatorOverrideModule : MonoBehaviour {

	public Text nameDisplay, currentStateDisplay, resourceDisplay;
	public InputField hours, minutes, seconds;
	public Button setStateButton;
	public Toggle toggleOn, toggleOff;

	[HideInInspector] public FarmResource myResource;
	[HideInInspector] public FarmActuator myActuator;

	private int durationInSeconds, stateValue;
	private bool validDuration, toggleSet;
	private string overrideURL = "override/";
	private string newOverrideURL = "/actuatorOverride/";

	void Update()
	{
		if (validDuration && toggleSet) 
		{
			setStateButton.interactable = true;
		}
		else
		{
			setStateButton.interactable = false;
		}
	}

	public IEnumerator Initialize()
	{
		nameDisplay.text = myActuator.name;
		// Set current state
		resourceDisplay.text = myResource.name;
		yield return null;
	}

	public void setDuration()
	{
		int dur = 0;
		int h=0, m=0, s=0;
		bool parseAttempt;
		parseAttempt = int.TryParse (hours.text, out h);
		parseAttempt = int.TryParse (minutes.text, out m);
		parseAttempt = int.TryParse (seconds.text, out s);
		dur = s + 60 * (m + 60 * (h));

		durationInSeconds = dur;
		if (dur > 0) 
		{
			validDuration = true;
		}
		else 
		{
			 validDuration = false;
		}
	}

	public void ToggleState()
	{
		toggleSet = true;
		if (toggleOn.isOn) 
		{
			stateValue = 1;
		}
		else
		{
			stateValue = 0;
		}
	}

	void ResetModule()
	{
		toggleOn.group.allowSwitchOff = true;
		toggleOn.isOn = false;
		toggleOff.isOn = false;
		toggleOn.group.allowSwitchOff = false;
		hours.text = "";
		minutes.text = "";
		seconds.text = "";
		validDuration = false;
		toggleSet = false;

		setStateButton.interactable = false;

	}

	public void SetButtonPress()
	{
		StopCoroutine ("SetState");
		StartCoroutine("SetState");
	}


	public IEnumerator SetState()
	{
		string url = string.Concat(DataManager.dataManager.ipAddress, newOverrideURL);
		int startTime = DataManager.DateTimeNow();
		int endTime = startTime + durationInSeconds;
		
		WWWForm form = new WWWForm ();
		form.AddField("actuator", myActuator.url);
		form.AddField("value", stateValue);		
		form.AddField("start_timestamp", startTime);
		form.AddField("end_timestamp", endTime);
		
		object[] parms = new object[2] { url, form };
		yield return DataManager.dataManager.StartCoroutine("PostRequest", parms);

		ResetModule ();

		yield return null;
	}
}
