using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnterLoginCredentials : MonoBehaviour {

	public InputField emailInput, passwordInput;
	public Button attemptbutton;
	public Text ipLabel;

	public void AttemptLogin()
	{
		//LoginManager.loginManager.SuccessfulLogin ();
		object[] creds = new object[2] {emailInput.text, passwordInput.text};
		LoginManager.loginManager.StartCoroutine("LoginAttempt", creds);
	}

	public void InputEdit()
	{
		if (emailInput.text.Length > 0 && passwordInput.text.Length > 0) 
		{
			attemptbutton.interactable = true;
		}
	}
	
	public void BackButtonPress()
	{
		LoginManager.loginManager.ChangeIP();
	}
}
