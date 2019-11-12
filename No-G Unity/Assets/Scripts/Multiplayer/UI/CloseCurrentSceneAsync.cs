using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseCurrentSceneAsync : MonoBehaviour
{
    public void ReloadMaster()
	{
        SceneManager.LoadSceneAsync("Master", LoadSceneMode.Single);
		//SceneManager.UnloadSceneAsync(scene);
	}

    public void CloseScene(string scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }
}
