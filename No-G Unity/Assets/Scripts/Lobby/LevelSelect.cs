using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public string[] LevelList;

    public Text LevelText;

    private int i;

    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        LevelText.text = LevelList[i];
    }

    public void LevelCycle()
    {
        Debug.Log(LevelList.Length);
        Debug.Log(i);
        i = (LevelList.Length == i + 1 ? 0 : ++i);

        LevelText.text = LevelList[i];
    }
}
