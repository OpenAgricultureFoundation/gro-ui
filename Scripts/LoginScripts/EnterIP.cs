using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnterIP : MonoBehaviour {

	public InputField ipAddressInput;
	public Button AttemptButton;

	public void ValidateIP()
	{
		string ip = ipAddressInput.text;
		LoginManager.loginManager.SuccessfulIP (ip);
	}

	public void InputEdit()
	{
		string inputText = ipAddressInput.text; 
		if (inputText.Length > 0) 
		{
			AttemptButton.interactable = true;
		}
		else
		{
			AttemptButton.interactable = false;
		}
	}
	
	public void AttemptButtonPress()
	{
		LoginManager.loginManager.StartCoroutine("ValidateIP", ipAddressInput.text);
	}
}
