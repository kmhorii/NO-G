using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseMenu : MonoBehaviour
{
    public void CloseSettings()
    {
        if (SceneManager.GetSceneByName("Settings").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Settings");
        }
    }
}
