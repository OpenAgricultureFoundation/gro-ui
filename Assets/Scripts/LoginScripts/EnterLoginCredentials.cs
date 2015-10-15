using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


public class EnterLoginCredentials : MonoBehaviour {

	public InputField emailInput, passwordInput;
	public Button attemptbutton;
	public Text ipLabel;
	EventSystem system;

	void Start()
	{
		system = EventSystem.current;// EventSystemManager.currentSystem;
		emailInput.ActivateInputField ();
	}
	public void AttemptLogin()
	{
		//LoginManager.loginManager.SuccessfulLogin ();
		object[] creds = new object[2] {emailInput.text, passwordInput.text};
		LoginManager.loginManager.StartCoroutine("LoginAttempt", creds);
	}

	public void GuestLogin()
	{
		//Login as Guest
		DataManager.dataManager.isGuest = true;
		LoginManager.loginManager.StartCoroutine ("LoginGuest");
	}

	public void InputEdit()
	{
		if (emailInput.text.Length > 0 && passwordInput.text.Length > 0) 
		{
			attemptbutton.interactable = true;
		}
	}

	void Update(){
		if (Input.GetKeyDown ("return")) {
			AttemptLogin ();
		}

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
			
			if (next != null)
			{
				
				InputField inputfield = next.GetComponent<InputField>();
				if (inputfield != null)
					inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret
				
				system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
			}
			//else Debug.Log("next nagivation element not found");
			
		}
	}

	
	public void BackButtonPress()
	{
		LoginManager.loginManager.ChangeIP();
	}
}
