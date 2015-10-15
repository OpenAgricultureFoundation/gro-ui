using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class LoginManager : MonoBehaviour {

	public static LoginManager loginManager;

	public GameObject ipEnterPanel, loginPanel, loadingPanel;

	private string ipAddress, username, password, token;
	private int numLoginAttempts = 0;
	private string loginSuffix = "/auth/login/";

	void Awake()
	{
		if (loginManager == null) 
		{
			loginManager = this;
		}
	}

	IEnumerator Start()
	{
		//StartCoroutine("GetIP");
		while (!DataManager.dataManager.initialLoad)
		{
			yield return new WaitForSeconds(0.5f);
		}
		StartCoroutine("GetIP");
		yield return null;
	}

	public IEnumerator GetIP()
	{
		// attempt to validate IP
		if(!string.IsNullOrEmpty (DataManager.dataManager.ipAddress))
		{
			yield return StartCoroutine("ValidateIP", DataManager.dataManager.ipAddress);
		}
		// If valid, save IP in data manager, move to login

		// Else, return to IP screen and notify of bad IP
		else
		{
			ipEnterPanel.SetActive (true);
		}
		
		yield return null;
	}
	
	public IEnumerator GetCredentials()
	{
		if(DataManager.dataManager.stayLoggedIn)
		{
			
		}	
		yield return null;
	}

	public void SuccessfulIP(string ip)
	{
		StopCoroutine("GetIP");
		DataManager.dataManager.ipAddress = ip;
		DataManager.dataManager.baseURL = ip;
		DataManager.dataManager.StartCoroutine("Save");
		ipEnterPanel.SetActive (false);
		loadingPanel.SetActive (false);
		loginPanel.SetActive (true);
		loginPanel.GetComponent<EnterLoginCredentials>().ipLabel.text = ip;
	}
	
	public IEnumerator ValidateIP(string ip)
	{
		WWW www = new WWW(ip);
		yield return www;
		if(!string.IsNullOrEmpty(www.error))
		{
			// Not a valid IP address at all
			// Display a message like:
			// "Not a valid IP address. IP addreses generally follow the format 'xx.xxx.xx.xxx or xxx.xxx.x.x'."
			DataManager.dataManager.ipAddress = "";
			Debug.Log (www.error);
			StartCoroutine("GetIP");
			yield break;
		}
		
		var node = JSON.Parse(www.text);
		if(node ["farm"] != null)
		{
			StartCoroutine("SuccessfulIP", ip);
			yield break;
		}
		else
		{
			ChangeIP();
		}
		yield return null;
	}

	public IEnumerator LoginAttempt(object[] parms)
	{
		loadingPanel.SetActive(true);
		string user = (string)parms [0];
		string pass = (string)parms [1];

		// attempt login
		WWWForm form = new WWWForm();
		form.AddField("username", user);
		form.AddField("password", pass);
		string loginURL = DataManager.dataManager.ipAddress + loginSuffix;
		WWW login = new WWW(loginURL, form);
		yield return login;
		loadingPanel.SetActive(false);
		if(!string.IsNullOrEmpty(login.error))
		{
			Debug.Log (login.error);
			Debug.Log (login.text);
			
			yield break;
		}
		
		// Get token
		string token = "Token ";
		token += (JSON.Parse(login.text))["key"].Value;
		//print (token);
		DataManager.dataManager.token = token;
		DataManager.dataManager.username = user;
		DataManager.dataManager.StartCoroutine("Save");
		SuccessfulLogin();
		// If valid, set token, user, NOT PASSWORD in data manager, save data, load mainView scene

		// Else return to login screen and notify bad of login
		yield return null;
	}

	public IEnumerator LoginGuest()
		//Login as Guest. 
	{
		SuccessfulLogin();
		//Set Login bool in action buttons as guest. 

		yield return null;
	}

	private IEnumerator ValidateToken(string token)
	{
		yield return null;
	}

	public void SuccessfulLogin()
	{
		Application.LoadLevel (1);
	}
	
	public void ChangeIP()
	{
		string oldIP = DataManager.dataManager.ipAddress;
		DataManager.dataManager.ipAddress = "";
		DataManager.dataManager.StartCoroutine("Save");
		
		ipEnterPanel.SetActive (true);
		loadingPanel.SetActive (false);
		loginPanel.SetActive (false);
		
		ipEnterPanel.GetComponent<EnterIP>().ipAddressInput.text = oldIP;
	}
}
