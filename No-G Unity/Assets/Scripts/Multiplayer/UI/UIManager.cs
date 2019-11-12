using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager: MonoBehaviour
{
   public void Create()
	{
		SceneManager.LoadSceneAsync("Register", LoadSceneMode.Single);
	}

	public void Login()
	{
		SceneManager.LoadScene("Login", LoadSceneMode.Single);
	}
}
