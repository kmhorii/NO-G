using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
	public InputField username;
	public InputField password;
	public InputField confPassword;
	public InputField email;

    public void CreateAccount()
	{
		if(password.text == confPassword.text)
		{
			RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
			request.Username = username.text;
			request.Password = confPassword.text;
			request.Email = email.text;
			request.DisplayName = username.text;

			PlayFabClientAPI.RegisterPlayFabUser(request, result => {
				Alerts a = new Alerts();
				StartCoroutine(a.CreateNewAlert(result.Username + " has been created"));
			}, error => {
				Alerts a = new Alerts();
				StartCoroutine(a.CreateNewAlert(error.ErrorMessage));
			});
		}
	}
}
