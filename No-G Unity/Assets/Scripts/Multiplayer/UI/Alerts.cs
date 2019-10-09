using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Alerts
{
    public IEnumerator CreateNewAlert(string msg)
	{
		yield return SceneManager.LoadSceneAsync("Alerts", LoadSceneMode.Additive); ;
		GameObject.FindObjectOfType<AlertObjects>().AlertText.text = msg;
	}

}
