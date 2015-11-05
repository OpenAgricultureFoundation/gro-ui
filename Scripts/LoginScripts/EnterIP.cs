using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class EnterIP : MonoBehaviour {

	public InputField ipAddressInput;
	public Button AttemptButton;


	void Start(){
		ipAddressInput.ActivateInputField ();
	}
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

	void Update(){
		if (Input.GetKeyDown("return")){
			AttemptButtonPress ();
		}
	}

	public void AttemptButtonPress()
	{
		string ipAddress = ipAddressInput.text.Replace(System.Environment.NewLine, "");
		LoginManager.loginManager.StartCoroutine("ValidateIP", ipAddress);
	}
}
