using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayFabAuth : MonoBehaviour
{
	public InputField username;
	public InputField password;
	public string levelToLoad;

    public void Login()
	{
		LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();

		request.Username = username.text;
		request.Password = password.text;

		PlayFabClientAPI.LoginWithPlayFab(request, result => {
			Alerts a = new Alerts();
			StartCoroutine(a.CreateNewAlert(username.text + " You have logged in!"));
			PlayerInfo.Name = username.text;
			SceneManager.LoadScene(levelToLoad);

		}, error => {
			Alerts a = new Alerts();
			StartCoroutine(a.CreateNewAlert(error.ErrorMessage));
		});
	}

    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene(levelToLoad);
    }
}
